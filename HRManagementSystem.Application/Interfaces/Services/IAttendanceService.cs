using HRManagementSystem.Application.Common;
using HRManagementSystem.Application.DTOs.Attendance;
using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IAttendanceService
    {
        Task<Result> CheckInAsync(int employeeId);
        Task<Result> CheckOutAsync(int employeeId);

        Task<Result<AttendanceStatisticsDto>> GetEmployeeMonthlyStatsAsync(int employeeId, int month, int year);
        Task<IEnumerable<AttendanceDto>> GetEmployeeHistoryAsync(int employeeId, DateTime start, DateTime end);

        Task<Result> UpdateAttendanceAdjustmentAsync(int attendanceId, AttendanceStatus newStatus, string? notes);
        Task<Result> BulkUpdateAttendanceStatusAsync(List<int> attendanceIds, AttendanceStatus newStatus, string? notes);
        Task<Result<IEnumerable<DailyAttendanceReportDto>>> GetDailyReportAsync(DateTime date);
        Task<bool> IsEmployeeOnLeaveAsync(int employeeId, DateTime date);

        Task<Result<IEnumerable<AttendanceDto>>> SearchAttendanceAsync(AttendanceFilterRequest filter);

        Task ProcessDailyAbsenceAsync(DateTime date);
    }
}
