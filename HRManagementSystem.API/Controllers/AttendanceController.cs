using HRManagementSystem.Application.DTOs.Attendance;
using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAttendance([FromQuery] AttendanceFilterRequest filter)
        {
            var result = await _attendanceService.SearchAttendanceAsync(filter);
            return Ok(result);
        }

        [HttpPost("check-in/{employeeId}")]
        public async Task<IActionResult> CheckIn(int employeeId)
        {
            var result = await _attendanceService.CheckInAsync(employeeId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok("Checked in successfully.");
        }

        [HttpPost("check-out/{employeeId}")]
        public async Task<IActionResult> CheckOut(int employeeId)
        {
            var result = await _attendanceService.CheckOutAsync(employeeId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok("Checked out successfully.");
        }

        [HttpGet("stats/{employeeId}/{month}/{year}")]
        public async Task<IActionResult> GetMonthlyStats(int employeeId, int month, int year)
        {
            var result = await _attendanceService.GetEmployeeMonthlyStatsAsync(employeeId, month, year);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value); 
        }

        [HttpGet("history/{employeeId}")]
        public async Task<IActionResult> GetHistory(int employeeId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var history = await _attendanceService.GetEmployeeHistoryAsync(employeeId, start, end);
            return Ok(history);
        }

        [HttpPut("adjust/{attendanceId}")]
        public async Task<IActionResult> AdjustAttendance(int attendanceId, [FromBody] AdjustmentRequest request)
        {
            var result = await _attendanceService.UpdateAttendanceAdjustmentAsync(attendanceId, request.NewStatus, request.Notes);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok("Adjustment applied successfully.");
        }

        [HttpPost("bulk-update")]
        public async Task<IActionResult> BulkUpdate([FromBody] BulkAdjustmentRequest request)
        {
            var result = await _attendanceService.BulkUpdateAttendanceStatusAsync(request.AttendanceIds, request.NewStatus, request.Notes);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok("Bulk update applied successfully.");
        }

        [HttpGet("daily-report/{date}")]
        public async Task<IActionResult> GetDailyReport(DateTime date)
        {
            var result = await _attendanceService.GetDailyReportAsync(date);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(result.Value);
        }


    }
}

