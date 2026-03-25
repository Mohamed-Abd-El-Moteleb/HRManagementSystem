using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface ILeaveAllocationRepository
    {
        Task<LeaveAllocation?> GetEmployeeAllocationAsync(int employeeId, int year, LeaveType type);
        Task AddAsync(LeaveAllocation allocation);
        void Update(LeaveAllocation allocation);
    }
}
