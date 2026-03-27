using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Infrastructure.Data.Context;
using System;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        public IEmployeeRepository Employees { get; }
        public IDepartmentRepository Departments { get; }
        public ILeaveRequestRepository LeaveRequests { get; }
        public ILeaveAllocationRepository LeaveAllocations { get; }
        public IAttendanceRepository AttendanceRepository { get; }
        public IPublicHolidayRepository PublicHolidayRepository { get; }


        public UnitOfWork(
            AppDbContext context,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            ILeaveRequestRepository leaveRequests,
            ILeaveAllocationRepository leaveAllocations,
            IAttendanceRepository attendanceRepository,
            IPublicHolidayRepository publicHolidayRepository)
        {
            _context = context;
            Employees = employeeRepository;
            Departments = departmentRepository;
            LeaveRequests = leaveRequests;
            LeaveAllocations = leaveAllocations;
            AttendanceRepository = attendanceRepository;
            PublicHolidayRepository = publicHolidayRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
