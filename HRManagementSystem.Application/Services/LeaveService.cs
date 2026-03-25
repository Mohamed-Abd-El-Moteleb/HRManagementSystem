using HRManagementSystem.Application.DTOs.LeaveRequest;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services
{
    public class LeaveService:ILeaveService
    {
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly ILeaveAllocationRepository _allocationRepository;
        private readonly IUnitOfWork _unitOfWork;
        public LeaveService(
        ILeaveRequestRepository requestRepository,
        ILeaveAllocationRepository allocationRepository,
        IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _allocationRepository = allocationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> RequestLeaveAsync(CreateLeaveRequestDto dto)
        {
            var currentYear = DateTime.Now.Year;
            var allocation = await _allocationRepository.GetEmployeeAllocationAsync(dto.EmployeeId, currentYear, dto.LeaveType);

            if (allocation == null)
                throw new BusinessException("No leave allocation found for this employee for the current year.");

            var leaveRequest = LeaveRequest.Create(
            dto.EmployeeId,
            dto.LeaveType,
            dto.StartDate,
            dto.EndDate,
            dto.Reason);

            allocation.DeductDays(leaveRequest.DurationInDays);

            await _unitOfWork.LeaveRequests.AddAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return leaveRequest.Id;
        }
        public async Task ApproveLeaveAsync(int requestId, string? managerComment)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null) throw new BusinessException("Leave request not found.");

            request.Approve();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectLeaveAsync(int requestId, string managerComment)
        {
            var request = await _unitOfWork.LeaveRequests.GetByIdAsync(requestId);
            if (request == null) throw new BusinessException("Leave request not found.");

            request.Reject(managerComment);

            var allocation = await _allocationRepository.GetEmployeeAllocationAsync(
            request.EmployeeId,
            request.StartDate.Year,
            request.Type);

            if (allocation != null)
            {
                allocation.RestoreDays(request.DurationInDays);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task CancelLeaveAsync(int requestId) {
            {
                var request = await _unitOfWork.LeaveRequests.GetByIdAsync(requestId);
                if (request == null) throw new BusinessException("Leave request not found.");

                request.Cancel();

                var allocation = await _unitOfWork.LeaveAllocations.GetEmployeeAllocationAsync(
                request.EmployeeId,
                request.StartDate.Year,
                request.Type);

                if (allocation != null)
                {
                    allocation.RestoreDays(request.DurationInDays);
                }

                await _unitOfWork.SaveChangesAsync();

            }
        }
        public async Task<IEnumerable<LeaveRequestDetailsDto>> GetEmployeeLeavesAsync(int employeeId)
        {
            var leaves = await _unitOfWork.LeaveRequests.GetByEmployeeIdAsync(employeeId);

            var dtos = leaves.Select(l => new LeaveRequestDetailsDto
            {
                Id = l.Id,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                DurationInDays = l.DurationInDays,
                Status = l.Status,
                Type = l.Type,
                Reason = l.Reason,
                RequestedAt = l.RequestedAt,
                ManagerComment = l.ManagerComment,
                EmployeeName = l.Employee?.FullName.ToString()?? "Uknown"
            }).ToList();
            return dtos ?? new List<LeaveRequestDetailsDto>();

        }
    }
}
