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
    public class LeaveAllocationRepository:ILeaveAllocationRepository
    {
        private readonly AppDbContext _context;

        public LeaveAllocationRepository(AppDbContext context)
        {
            _context = context;
        }

       public async Task<LeaveAllocation?> GetEmployeeAllocationAsync(int employeeId, int year, LeaveType type)
        {
            return await _context.LeaveAllocations.FirstOrDefaultAsync(la => la.EmployeeId == employeeId && la.Year == year && la.LeaveType == type);
        }
        public async Task<IEnumerable<LeaveAllocation?>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveAllocations.Where(la => la.EmployeeId == employeeId).ToListAsync();
        }

        public async Task AddAsync(LeaveAllocation allocation)
        {
            await _context.LeaveAllocations.AddAsync(allocation);
        }

        public void Update(LeaveAllocation allocation)
        {
            _context.LeaveAllocations.Update(allocation);
        }
        public void Delete(LeaveAllocation allocation)
        {
            _context.LeaveAllocations.Remove(allocation);
        }
    }
}
