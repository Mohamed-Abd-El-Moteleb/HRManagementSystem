using HRManagementSystem.Application.DTOs.LeaveAllocation;
using HRManagementSystem.Application.DTOs.LeaveRequest;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly ILeaveAllocationService _leaveAllocationService;
        public LeaveController(ILeaveService leaveService, ILeaveAllocationService leaveAllocationService)
        {
            _leaveService = leaveService;
            _leaveAllocationService = leaveAllocationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDetailsDto>> GetLeaveById(int id)
        {
            var leave = await _leaveService.GetLeaveDetailsAsync(id);
            if (leave == null) return NotFound();
            return Ok(leave);
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDetailsDto>>> GetPendingLeaves()
        {
            var leaves = await _leaveService.GetAllPendingRequestsAsync();
            return Ok(leaves);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<LeaveRequestDetailsDto>>> GetEmployeeLeaves(int employeeId)
        {
            var leaves = await _leaveService.GetEmployeeLeavesAsync(employeeId);
            return Ok(leaves);
        }

        [HttpGet("employee/{employeeId}/allocations")]
        public async Task<ActionResult<IEnumerable<AllocationDetailsDto>>> GetEmployeeAllocations(int employeeId)
        {
            var allocations = await _leaveAllocationService.GetEmployeeAllocationsAsync(employeeId);
            return Ok(allocations);
        }

        [HttpPost]
        public async Task<ActionResult<int>> RequestLeave ([FromBody] CreateLeaveRequestDto requestDto)
        {
            var leaveRequestId = await _leaveService.RequestLeaveAsync(requestDto);
            return CreatedAtAction(nameof(GetLeaveById), new { id = leaveRequestId },leaveRequestId);
        }

        [HttpPost("allocate")]
        public async Task<IActionResult> AllocateLeave([FromBody] CreateAllocationDto dto)
        {
            await _leaveAllocationService.AssignAllocationAsync(
                dto.EmployeeId,
                dto.LeaveType,
                dto.TotalDays,
                dto.Year);

            return Ok("Allocation created successfully.");
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveLeave(int id, [FromBody] UpdateLeaveRequestDto updateLeaveRequestDto)
        {
            await _leaveService.ApproveLeaveAsync(id, updateLeaveRequestDto.ManagerComment);
            return NoContent();
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectLeave(int id, [FromBody] UpdateLeaveRequestDto updateLeaveRequestDto)
        {
            await _leaveService.RejectLeaveAsync(id, updateLeaveRequestDto.ManagerComment);
            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelLeave(int id)
        {
            await _leaveService.CancelLeaveAsync(id);
            return NoContent();
        }

       
    }
}
