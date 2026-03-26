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

       public async Task<LeaveRequestDetailsDto?> GetLeaveDetailsAsync(int requesId)
        {
            var leave = await _requestRepository.GetByIdAsync(requesId);
            return leave == null ? null : MapToDto(leave);
        }
       public async Task<IEnumerable<LeaveRequestDetailsDto>> GetAllPendingRequestsAsync()
        {
            var leaves = await _requestRepository.GetAllPendingAsync();
            return leaves.Select(MapToDto).ToList();
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

        public async Task RejectLeaveAsync(int requestId, string? managerComment)
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

            var dtos = leaves.Select(MapToDto).ToList();
            return dtos ?? new List<LeaveRequestDetailsDto>();

        }


        private LeaveRequestDetailsDto MapToDto(LeaveRequest leave)
        {
            return new LeaveRequestDetailsDto
            {
                Id = leave.Id,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                DurationInDays = leave.DurationInDays,
                Status = leave.Status,
                Type = leave.Type,
                Reason = leave.Reason,
                RequestedAt = leave.RequestedAt,
                ManagerComment = leave.ManagerComment,
                EmployeeName = leave.Employee?.FullName.ToString() ?? "Unknown"
            };
        }
    }
}
