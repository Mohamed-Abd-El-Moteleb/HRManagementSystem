using HRManagementSystem.Application.DTOs.Attendance;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetAttendanceByIdAsync(int attendanceId);
        Task<Attendance?> GetAttendanceByDateAsync(int employeeId, DateTime date);
        Task<bool> HasCheckedInAsync(int employeeId, DateTime date);
        Task<IEnumerable<Attendance>> GetEmployeeAttendanceHistoryAsync(int employeeId, DateTime startDate, DateTime endDate);

        //Dashboards
        Task<IEnumerable<Attendance>> SearchAttendanceAsync(AttendanceFilterRequest filter);
        Task<List<Attendance>> GetAttendancesForStatsAsync(int employeeId, int month, int year);
        Task<IEnumerable<Attendance>> GetDepartmentAttendanceReportAsync(int departmentId, DateTime date);
        Task<IEnumerable<Attendance>> GetDailyReportAsync(DateTime date);
        Task<double> GetTotalWorkHoursByMonthAsync(int employeeId, DateTime monthDate);
        Task UpdateAttendanceStatusAsync(int attendanceId, AttendanceStatus newStatus, string? comment);
        Task BulkUpdateStatusAsync(List<int> attendanceIds, AttendanceStatus newStatus, string? comment);
        //(Integration & Logic Hooks)
        Task<bool> HasApprovedLeaveAsync(int employeeId, DateTime date); 

        Task AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
    }

}
