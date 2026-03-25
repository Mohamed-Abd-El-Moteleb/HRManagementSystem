using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        ILeaveRequestRepository LeaveRequests { get; }
        ILeaveAllocationRepository LeaveAllocations { get; }
        Task<int> SaveChangesAsync();
    }
}
