using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
        Task<LeaveRequest?> GetWithEmployeeAsync(int id);
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<IEnumerable<LeaveRequest?>> GetAllPendingAsync();
        Task AddAsync(LeaveRequest leaveRequest);
        void Update(LeaveRequest leaveRequest);
        void Delete(LeaveRequest leaveRequest);
    }
}
