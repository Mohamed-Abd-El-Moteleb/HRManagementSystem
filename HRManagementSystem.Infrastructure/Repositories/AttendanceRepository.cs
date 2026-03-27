using HRManagementSystem.Application.DTOs.Attendance;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repositories
{
    public class AttendanceRepository:IAttendanceRepository
    {
        private readonly AppDbContext _context;
        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Attendance?> GetAttendanceByIdAsync(int attendanceId)
        {
            return await _context.Attendances.FindAsync(attendanceId);
        }

        public async Task<Attendance?> GetAttendanceByDateAsync(int employeeId, DateTime date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date);
        }
        public async Task<bool> HasCheckedInAsync(int employeeId, DateTime date)
        {
            return await _context.Attendances
                .AnyAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date);
        }
        
        public async Task<IEnumerable<Attendance>> GetEmployeeAttendanceHistoryAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            var Attendances = await _context.Attendances.Where(a=> a.EmployeeId == employeeId && a.Date.Date >= startDate.Date && a.Date.Date <= endDate.Date).Include(e=>e.Employee).AsNoTracking()
                .ToListAsync();
            return Attendances;
        }
        public async Task<IEnumerable<Attendance>> GetAbsentEmployeesAsync(DateTime date)
        {
            var absentAttendances= await _context.Attendances.Where(a => a.Date.Date == date.Date && a.Status == AttendanceStatus.Absent)
                .Include(a => a.Employee).AsNoTracking()
                .ToListAsync();
            return absentAttendances;
        }

        public async Task<IEnumerable<Attendance>> GetDepartmentAttendanceReportAsync(int departmentId, DateTime date)
        {
            var report = await _context.Attendances
                .Where(a => a.Date.Date == date.Date && a.Employee.DepartmentId == departmentId)
                .Include(a => a.Employee).AsNoTracking()
                .ToListAsync();
            return report;
        }

        public async Task<IEnumerable<Attendance>> GetDailyReportAsync(DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date.Date == date.Date)
                .ToListAsync();
        }
        public async Task<bool> HasApprovedLeaveAsync(int employeeId, DateTime date)
        {
            return await _context.LeaveRequests.AnyAsync(l => l.EmployeeId == employeeId && l.StartDate.Date <= date.Date && l.EndDate.Date >= date.Date && l.Status == LeaveStatus.Approved);

        }

        public async Task< List<Attendance>> GetAttendancesForStatsAsync(int employeeId, int month, int year)
        {
                            return await _context.Attendances
                        .Where(a => a.EmployeeId == employeeId && a.Date.Month == month && a.Date.Year == year)
                        .ToListAsync();
        }

        public async Task UpdateAttendanceStatusAsync(int attendanceId, AttendanceStatus newStatus, string? notes)
        {
            var attendance = await _context.Attendances.FindAsync(attendanceId);
            if (attendance != null)
            {
                attendance.Status = newStatus;
                attendance.Notes = notes;
            }
        }

        public async Task BulkUpdateStatusAsync(List<int> attendanceIds, AttendanceStatus newStatus, string? notes)
        {
            var records = await _context.Attendances.Where(a => attendanceIds.Contains(a.Id))
                .ExecuteUpdateAsync(s => s
                .SetProperty(a => a.Status, newStatus)
                .SetProperty(a => a.Notes, notes));

        }

        public async Task<double> GetTotalWorkHoursByMonthAsync(int employeeId, DateTime monthDate)
        {
            return await _context.Attendances
                 .Where(a => a.EmployeeId == employeeId
                          && a.Date.Month == monthDate.Month
                          && a.Date.Year == monthDate.Year
                          && a.CheckIn != null
                          && a.CheckOut != null)
                 .SumAsync(a => EF.Functions.DateDiffMinute(a.CheckIn, a.CheckOut) ?? 0) / 60.0;
        }
        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
        }


        public async Task<IEnumerable<Attendance>> SearchAttendanceAsync(AttendanceFilterRequest filter)
        {
            var query = _context.Attendances.Include(a => a.Employee).AsQueryable();

            if (filter.EmployeeId.HasValue)
                query = query.Where(a => a.EmployeeId == filter.EmployeeId);

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Date >= filter.StartDate);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Date <= filter.EndDate);

            if (filter.Status.HasValue)
                query = query.Where(a => a.Status == filter.Status);

            // Pagination
            return await query
                .OrderByDescending(a => a.Date)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }


    }
}
