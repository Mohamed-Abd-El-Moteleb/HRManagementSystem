using HRManagementSystem.Application.DTOs.LeaveAllocation;
using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface ILeaveAllocationService
    {
        Task AssignAllocationAsync(int employeeId, LeaveType leaveType, int days, int year);
        Task<IEnumerable<AllocationDetailsDto>> GetEmployeeAllocationsAsync(int employeeId);
    }
}
