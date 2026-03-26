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
    public class LeaveRequestRepository: ILeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }
       public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests.Where(lr => lr.EmployeeId == employeeId).Include(lr=>lr.Employee).OrderByDescending(lr=>lr.RequestedAt).ToListAsync();
        }
       public async Task<LeaveRequest?> GetWithEmployeeAsync(int id)
        {
            return await _context.LeaveRequests.Include(lr => lr.Employee).FirstOrDefaultAsync(lr => lr.Id == id);
        }
        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests.FindAsync(id);
        }
        public async Task<IEnumerable<LeaveRequest?>> GetAllPendingAsync()
        {
            return await _context.LeaveRequests.Where(lr => lr.Status == LeaveStatus.Pending).Include(lr => lr.Employee).OrderByDescending(lr => lr.RequestedAt).ToListAsync();
        }
        public async Task AddAsync(LeaveRequest leaveRequest)
        {
            await _context.LeaveRequests.AddAsync(leaveRequest);
        }

        

        public void Update(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Update(leaveRequest);
        }

        public void Delete(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Remove(leaveRequest);
        }
    }
}
