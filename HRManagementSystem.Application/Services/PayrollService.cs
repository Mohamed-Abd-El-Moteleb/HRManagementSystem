using AutoMapper;
using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.DTOs.SalarySlip;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Exceptions;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services
{
    public class PayrollService:IPayrollService
    {
        private readonly ISalarySlipRepository _salarySlipRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IAttendanceService _attendanceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PayrollService(
            ISalarySlipRepository salarySlipRepo,
            IEmployeeRepository employeeRepo,
            IAttendanceService attendanceService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _salarySlipRepo = salarySlipRepo;
            _employeeRepo = employeeRepo;
            _attendanceService = attendanceService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SalarySlipDto> GenerateSalarySlipAsync(int employeeId, int month, int year)
        {
            var employee = await _employeeRepo.GetByIdAsync(employeeId);
            if (employee == null) throw new NotFoundException("Employee not found");

            if (await _salarySlipRepo.ExistsAsync(employeeId, month, year))
                throw new BusinessException("Salary slip already exists for this period.");

            var attendanceResult = await _attendanceService.GetEmployeeMonthlyStatsAsync(employeeId, month, year);

            if (!attendanceResult.IsSuccess)
                throw new BusinessException($"Could not fetch attendance data: {attendanceResult.Error}");

            var stats = attendanceResult.Value;

            var absenceDeduction = CalculateAbsenceDeduction(employee.Salary, stats.TotalAbsentDays);
            var overtimeAmount = CalculateOvertimeAmount(employee.Salary, stats.OvertimeHours);

            var holidayWorkAmount = CalculateHolidayWorkAmount(employee.Salary, stats.TotalPublicHolidayWorkHours);

            var salarySlip = new SalarySlip(employeeId, month, year, employee.Salary);

            salarySlip.ApplyAttendanceMetrics(
            stats.TotalAbsentDays,
            absenceDeduction,
            stats.OvertimeHours,
            overtimeAmount);

            salarySlip.SetHolidayWork(stats.TotalPublicHolidayWorkDays, holidayWorkAmount);

            foreach (var allowance in employee.FixedAllowances)
            {
                salarySlip.AddAllowances(allowance.Name, allowance.Amount);
            }

            await _salarySlipRepo.AddAsync(salarySlip);
            await _unitOfWork.SaveChangesAsync(); 

            return _mapper.Map<SalarySlipDto>(salarySlip);
        }

        private Money CalculateAbsenceDeduction(Money baseSalary, int absentDays)
        {
            var dayRate = baseSalary.Amount / 30;
            return new Money(dayRate * absentDays, baseSalary.Currency);
        }

        private Money CalculateOvertimeAmount(Money baseSalary, double overtimeHours)
        {
            var hourRate = baseSalary.Amount / 240;
            return new Money(hourRate * 1.5m * (decimal)overtimeHours, baseSalary.Currency);
        }

        private Money CalculateHolidayWorkAmount(Money baseSalary, double holidayHours)
        {
            if (holidayHours <= 0) return new Money(0, baseSalary.Currency);

            var hourRate = baseSalary.Amount / 240;

            var amount = hourRate * 2 * (decimal)holidayHours;

            return new Money(Math.Round(amount, 2), baseSalary.Currency);
        }


        public async Task<IEnumerable<SalarySlipDto>> ProcessCompanyPayrollAsync(int month, int year)
        {
            var activeEmployees = await _employeeRepo.GetAllActiveAsync();
            var results = new List<SalarySlipDto>();

            foreach (var employee in activeEmployees)
            {
                var existingSlip = await _salarySlipRepo.GetByEmployeeAndPeriodAsync(employee.Id, month, year);

                if (existingSlip != null)
                {
                    if (existingSlip.IsPaid) continue;

                    var updated = await RecalculateAsync(existingSlip.Id);
                    results.Add(updated);
                }
                else
                {
                    var newSlip = await GenerateSalarySlipAsync(employee.Id, month, year);
                    results.Add(newSlip);
                }
            }
            return results;
        }

        public async Task<SalarySlipDto> RecalculateAsync(int salarySlipId)
        {
            var existingSlip = await _salarySlipRepo.GetByIdAsync(salarySlipId);
            if (existingSlip == null) throw new NotFoundException("Salary slip not found");

            var employee = await _employeeRepo.GetByIdAsync(existingSlip.EmployeeId);
            var attendanceResult = await _attendanceService.GetEmployeeMonthlyStatsAsync(existingSlip.EmployeeId, existingSlip.Month, existingSlip.Year);
            var stats = attendanceResult.Value;

            var absenceDeduction = CalculateAbsenceDeduction(employee.Salary, stats.TotalAbsentDays);
            var overtimeAmount = CalculateOvertimeAmount(employee.Salary, stats.OvertimeHours);
            var holidayWorkAmount = CalculateHolidayWorkAmount(employee.Salary, stats.TotalPublicHolidayWorkHours);
            existingSlip.Recalculate(
                stats.TotalAbsentDays, absenceDeduction,
                stats.OvertimeHours, overtimeAmount,
                (int)stats.TotalPublicHolidayWorkHours, holidayWorkAmount);

            existingSlip.ClearFixedAllowancesOnly();
            foreach (var allowance in employee.FixedAllowances)
            {
                existingSlip.AddAllowances(allowance.Name, allowance.Amount);
            }
            _salarySlipRepo.Update(existingSlip);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SalarySlipDto>(existingSlip);
        }

        public async Task AddManualAllowanceAsync(int salarySlipId, Money amount, string reason)
        {
            var slip = await _salarySlipRepo.GetByIdAsync(salarySlipId);
            if (slip == null) throw new NotFoundException("Salary slip not found");
            if (slip.IsFinalized) throw new BusinessException("Cannot modify a finalized slip");

            slip.AddAllowances(reason, amount, isManual: true);

            _salarySlipRepo.Update(slip);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddMonthlyBonusAsync(int salarySlipId, Money amount, string reason)
        {
            var slip = await _salarySlipRepo.GetByIdAsync(salarySlipId);
            if (slip == null) throw new NotFoundException("Salary slip not found");

            if (slip.IsFinalized || slip.IsPaid)
                throw new BusinessException("Cannot add bonus to a finalized or paid salary slip");

            slip.AddBonus(amount, reason);

            _salarySlipRepo.Update(slip);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddManualDeductionAsync(int salarySlipId, Money amount, string reason)
        {
            var slip = await _salarySlipRepo.GetByIdAsync(salarySlipId);
            if (slip == null)
                throw new NotFoundException("Salary slip not found");

            if (slip.IsFinalized || slip.IsPaid)
                throw new BusinessException("Cannot add deductions to a finalized or paid salary slip");

            slip.AddManualDeduction(amount, reason);

            _salarySlipRepo.Update(slip);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task FinalizeSalarySlipAsync(int salarySlipId)
        {
            var slip = await _salarySlipRepo.GetByIdAsync(salarySlipId);
            if (slip == null) throw new NotFoundException("Salary slip not found");

            slip.FinalizeSlip(); 

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int salarySlipId, DateTime paymentDate)
        {
            var slip = await _salarySlipRepo.GetByIdAsync(salarySlipId);
            if (slip == null) throw new NotFoundException("Salary slip not found");

            slip.MarkAsPaid(paymentDate);

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<PayrollSummaryDto> GetMonthlySummaryAsync(int month, int year)
        {
            var slips = await _salarySlipRepo.GetAllByPeriodAsync(month, year);

            return new PayrollSummaryDto
            {
                Month = month,
                Year = year,
                TotalEmployees = slips.Count(),
                TotalGrossSalaries = slips.Sum(s => s.GrossSalary.Amount),
                TotalNetSalaries = slips.Sum(s => s.NetSalary.Amount),
                PaidSlipsCount = slips.Count(s => s.IsPaid),
                PendingSlipsCount = slips.Count(s => !s.IsPaid),
                Currency = slips.FirstOrDefault()?.BaseSalary.Currency ?? "EGP"
            };
        }

        

        public async Task<IEnumerable<SalarySlipDto>> GetSlipsAsync(int month, int year, bool? isPaid = null, int? employeeId = null)
        {
            var slips = await _salarySlipRepo.GetAllByPeriodAsync(month, year);

            if (isPaid.HasValue)
                slips = slips.Where(s => s.IsPaid == isPaid.Value);

            if (employeeId.HasValue)
                slips = slips.Where(s => s.EmployeeId == employeeId.Value);

            return _mapper.Map<IEnumerable<SalarySlipDto>>(slips);
        }

        public async Task<SalarySlipDto> GetSlipByIdAsync(int id)
        {
            var slip = await _salarySlipRepo.GetByIdAsync(id);
            if (slip == null) throw new NotFoundException("Salary slip not found");

            return _mapper.Map<SalarySlipDto>(slip);
        }

        public async Task<IEnumerable<EmployeeDto>> GetMissingEmployeesForPayrollAsync(int month, int year)
        {
            var allActive = await _employeeRepo.GetAllActiveAsync();

            var processedIds = (await _salarySlipRepo.GetAllByPeriodAsync(month, year))
                                .Select(s => s.EmployeeId);

            var missing = allActive.Where(e => !processedIds.Contains(e.Id));

            return _mapper.Map<IEnumerable<EmployeeDto>>(missing);
        }
    }
}
