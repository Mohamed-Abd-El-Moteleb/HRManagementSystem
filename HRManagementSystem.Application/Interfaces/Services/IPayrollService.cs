using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.DTOs.SalarySlip;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<SalarySlipDto> GenerateSalarySlipAsync(int employeeId, int month, int year);

        Task<IEnumerable<SalarySlipDto>> ProcessCompanyPayrollAsync(int month, int year);


        Task AddManualAllowanceAsync(int salarySlipId, Money amount, string reason);

        Task AddMonthlyBonusAsync(int salarySlipId, Money amount, string reason);

        Task AddManualDeductionAsync(int salarySlipId, Money amount, string reason);

        Task<SalarySlipDto> RecalculateAsync(int salarySlipId);


        Task FinalizeSalarySlipAsync(int salarySlipId);

        Task MarkAsPaidAsync(int salarySlipId, DateTime paymentDate);

        Task<IEnumerable<SalarySlipDto>> GetSlipsAsync(int month, int year, bool? isPaid = null, int? employeeId = null);
        Task<SalarySlipDto> GetSlipByIdAsync(int id);
        Task<IEnumerable<EmployeeDto>> GetMissingEmployeesForPayrollAsync(int month, int year);

        Task<PayrollSummaryDto> GetMonthlySummaryAsync(int month, int year);

    }
}
