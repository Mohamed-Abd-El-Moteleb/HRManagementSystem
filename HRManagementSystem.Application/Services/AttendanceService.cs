using HRManagementSystem.Application.Common;
using HRManagementSystem.Application.DTOs.Attendance;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services
{
    public class AttendanceService:IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPublicHolidayService _publicHolidayService;
        private readonly IUnitOfWork _unitOfWork;

        private readonly TimeSpan _defaultShiftStart = new TimeSpan(9, 0, 0);
        private readonly TimeSpan _defaultShiftEnd = new TimeSpan(17, 0, 0);
        private const int _gracePeriodMinutes = 15;

        public AttendanceService(IAttendanceRepository attendanceRepository, 
            IUnitOfWork unitOfWork,
            IEmployeeRepository employeeRepository,
            IPublicHolidayService publicHolidayService)
        {
            _attendanceRepository = attendanceRepository;
            _unitOfWork = unitOfWork;
            _employeeRepository = employeeRepository;
            _publicHolidayService = publicHolidayService;
        }
        public async Task<Result<IEnumerable<AttendanceDto>>> SearchAttendanceAsync(AttendanceFilterRequest filter)
        {
            if (filter.StartDate > filter.EndDate)
                return Result<IEnumerable<AttendanceDto>>.Failure("Start date cannot be after end date.");

            var attendances = await _attendanceRepository.SearchAttendanceAsync(filter);

            var result = attendances.Select(a => new AttendanceDto
            {
                Id = a.Id,
                Date = a.Date,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName.ToString(),
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                Status = a.Status.ToString(),
                Notes = a.Notes
            });

            return Result<IEnumerable<AttendanceDto>>.Success(result);
        }
        public async Task<Result<AttendanceStatisticsDto>> GetEmployeeMonthlyStatsAsync(int employeeId, int month, int year)
        {
            var attendances = await _attendanceRepository.GetAttendancesForStatsAsync(employeeId, month, year);

            if (attendances == null || !attendances.Any())
                return Result<AttendanceStatisticsDto>.Failure("No statistics found for this period.");

            var publicHolidays = await _publicHolidayService.GetByMonthAsync(month, year);
            
            var holidayDates = publicHolidays
                .SelectMany(h => {
                    var dates = new List<DateTime>();
                    for (var dt = h.StartDate.Date; dt <= h.EndDate.Date; dt = dt.AddDays(1))
                        dates.Add(dt);
                    return dates;
                }).ToHashSet();

            double shiftDurationMinutes = (_defaultShiftEnd - _defaultShiftStart).TotalMinutes;

            var statsDto = new AttendanceStatisticsDto
            {
                EmployeeId = employeeId,
                TotalPresentDays = attendances.Count(a => a.Status == AttendanceStatus.Present),
                TotalAbsentDays = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                TotalLateDays = attendances.Count(a => a.Status == AttendanceStatus.Late),
                TotalBusinessTripDays = attendances.Count(a => a.Status == AttendanceStatus.BusinessTrip),
                TotalHasLeftEarlyDays= attendances.Count(a => a.Status == AttendanceStatus.HasLeftEarly),
                TotalOnLeavefDays = attendances.Count(a => a.Status == AttendanceStatus.OnLeave),
                TotalOvertimeDays = attendances.Count(a => a.Status == AttendanceStatus.Overtime),
                TotalRemoteWorkDays = attendances.Count(a => a.Status == AttendanceStatus.RemoteWork),
                TotalPublicHolidayWorkDays = attendances.Count(a =>
                a.CheckIn.HasValue && holidayDates.Contains(a.Date.Date)),
                TotalWorkingHours = attendances
                    .Where(a => a.CheckIn.HasValue && a.CheckOut.HasValue)
                    .Sum(a => (a.CheckOut.Value - a.CheckIn.Value).TotalHours),
                TotalPublicHolidayWorkHours = attendances
                    .Where(a => a.CheckIn.HasValue && a.CheckOut.HasValue && holidayDates.Contains(a.Date.Date))
                    .Sum(a => (a.CheckOut.Value - a.CheckIn.Value).TotalHours),
                OvertimeHours = attendances
                    .Where(a => a.Status == AttendanceStatus.Overtime && a.CheckIn.HasValue && a.CheckOut.HasValue)
                    .Sum(a => Math.Max(0, (a.CheckOut.Value - a.CheckIn.Value).TotalMinutes - shiftDurationMinutes)) / 60.0
            };


            return Result<AttendanceStatisticsDto>.Success(statsDto);
        }
        public async Task<IEnumerable<AttendanceDto>> GetEmployeeHistoryAsync(int employeeId, DateTime start, DateTime end)
        {
            var attendances = await _attendanceRepository.GetEmployeeAttendanceHistoryAsync(employeeId, start, end);

            return attendances.Select(a => new AttendanceDto
            {
                Id = a.Id,
                Date = a.Date,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName.ToString(),
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                Status = a.Status.ToString(),
                Notes = a.Notes
            });
        }
        public async Task<Result> CheckInAsync(int employeeId)
        {
            var employee=await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                return Result.Failure("Employee not found.");

            if (await IsEmployeeOnLeaveAsync(employeeId, DateTime.Now))
                return Result.Failure("Cannot check in: Employee has an approved leave today.");

            if (await _attendanceRepository.HasCheckedInAsync(employeeId, DateTime.Now))
                return Result.Failure("Employee has already checked in for today.");

            var attendance = new Attendance(employeeId, DateTime.Now);

            attendance.RecordCheckIn(DateTime.Now, _defaultShiftStart, _gracePeriodMinutes);

            await _attendanceRepository.AddAsync(attendance);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> CheckOutAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) return Result.Failure("Employee not found.");

            var now = DateTime.Now;
            var attendance = await _attendanceRepository.GetAttendanceByDateAsync(employeeId, now.Date);

            if (attendance == null)
                return Result.Failure("No check-in record found for today.");

            var isPublicHoliday = await _publicHolidayService.IsDatePublicHolidayAsync(now.Date);

            TimeSpan shiftDuration = _defaultShiftEnd - _defaultShiftStart;

            try
            {
                attendance.RecordCheckOut(now, _defaultShiftEnd, shiftDuration, isPublicHoliday);

                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> UpdateAttendanceAdjustmentAsync(int attendanceId, AttendanceStatus newStatus, string? notes)
        {
            var attendance = await _attendanceRepository.GetAttendanceByIdAsync(attendanceId); 
            if (attendance == null)
            {
                return Result.Failure("Attendance record not found.");
            }
            await _attendanceRepository.UpdateAttendanceStatusAsync(attendanceId, newStatus, notes);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();

        }


        public async Task<Result> BulkUpdateAttendanceStatusAsync(List<int> attendanceIds, AttendanceStatus newStatus, string? notes)
        {
            await _attendanceRepository.BulkUpdateStatusAsync(attendanceIds, newStatus, notes);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<DailyAttendanceReportDto>>> GetDailyReportAsync(DateTime date)
        {
            var attendances = await _attendanceRepository.GetDailyReportAsync(date);

            var report = attendances.Select(a => new DailyAttendanceReportDto
            {
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName.ToString(),
                Status = a.Status.ToString(),
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut
            });

            return Result<IEnumerable<DailyAttendanceReportDto>>.Success(report);
        }

        public async Task<bool> IsEmployeeOnLeaveAsync(int employeeId, DateTime date)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                throw new BusinessException("Employee not found.");

            return await _attendanceRepository.HasApprovedLeaveAsync(employeeId, date);
        }

        public async Task ProcessDailyAbsenceAsync(DateTime date)
        {
            if (await _publicHolidayService.IsDatePublicHolidayAsync(date))
                return;

            var allEmployees = await _employeeRepository.GetAllAsync();

            foreach (var employee in allEmployees)
            {
                var hasAttendance = await _attendanceRepository.HasCheckedInAsync(employee.Id, date);
                if (hasAttendance) continue;

                var isOnLeave = await IsEmployeeOnLeaveAsync(employee.Id, date);
                if (isOnLeave) continue;

                var absenceRecord = new Attendance(employee.Id, date)
                {
                    Status = AttendanceStatus.Absent,
                    Notes = "Auto-generated: No check-in recorded."
                };

                await _attendanceRepository.AddAsync(absenceRecord);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
        
}
