using HRManagementSystem.Application.DTOs.LeaveRequest;
using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface ILeaveService
    {
        Task<int> RequestLeaveAsync(CreateLeaveRequestDto requestDto);
        Task ApproveLeaveAsync(int requestId, string? managerComment);
        Task RejectLeaveAsync(int requestId, string? managerComment);
        Task CancelLeaveAsync(int requestId);
        Task<IEnumerable<LeaveRequestDetailsDto>> GetEmployeeLeavesAsync(int employeeId);
        Task<LeaveRequestDetailsDto?> GetLeaveDetailsAsync(int requesId);
        Task<IEnumerable<LeaveRequestDetailsDto?>> GetAllPendingRequestsAsync();
    }
}
