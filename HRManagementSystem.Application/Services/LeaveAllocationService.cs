using HRManagementSystem.Application.DTOs.LeaveAllocation;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services
{
    public class LeaveAllocationService:ILeaveAllocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LeaveAllocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AssignAllocationAsync(int employeeId, LeaveType leaveType, int days, int year)
        {
            var existing = await _unitOfWork.LeaveAllocations.GetEmployeeAllocationAsync(employeeId, year, leaveType);

            if (existing != null)
                throw new BusinessException("Employee already has allocation for this type and year.");
           var allocation = LeaveAllocation.Create(employeeId, year, leaveType , days);
            await _unitOfWork.LeaveAllocations.AddAsync(allocation);
            await _unitOfWork.SaveChangesAsync();
        }
       public async Task<IEnumerable<AllocationDetailsDto>> GetEmployeeAllocationsAsync(int employeeId)
        {
            var allocations = await _unitOfWork.LeaveAllocations.GetByEmployeeIdAsync(employeeId);
            return allocations.Select(a => new AllocationDetailsDto
            {
                EmployeeId = a.Id,
                Year = a.Year,
                LeaveType = a.LeaveType.ToString(),
                TotalDays = a.TotalDays,
                RemainingDays = a.RemainingDays
            }).ToList();
        }


    }
}
