using HRManagementSystem.Application.DTOs.PublicHoliday;
using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicHolidayController : ControllerBase
    {
        private readonly IPublicHolidayService _publicHolidayService;

        public PublicHolidayController(IPublicHolidayService service)
        {
            _publicHolidayService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicHolidayResponseDto>>> GetAll()
        {
            var result = await _publicHolidayService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublicHolidayResponseDto>> GetById(int id)
        {
            var result = await _publicHolidayService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreatePublicHolidayDto dto)
        {
            var id = await _publicHolidayService.CreateAsync(dto);
            return CreatedAtAction(actionName: nameof(GetById),
                routeValues: new {id},
                value: new { message = "Public Holiday created successfully", data = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePublicHolidayDto dto)
        {
            await _publicHolidayService.UpdateAsync(id, dto);
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _publicHolidayService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("upcoming/{count}")]
        public async Task<ActionResult<IEnumerable<PublicHolidayResponseDto>>> GetUpcoming(int count)
        {
            var result = await _publicHolidayService.GetUpcomingHolidaysAsync(count);
            return Ok(result);
        }

        [HttpGet("month/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<PublicHolidayResponseDto>>> GetByMonth(int year, int month)
        {
            var result = await _publicHolidayService.GetByMonthAsync(month, year);
            return Ok(result);
        }

        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<PublicHolidayResponseDto>>> GetByYear(int year)
        {
            var result = await _publicHolidayService.GetByYearAsync(year);
            return Ok(result);
        }

        [HttpGet("remaining/{year}")]
        public async Task<ActionResult<int>> GetRemaining(int year)
        {
            var count = await _publicHolidayService.GetRemainingHolidaysInYearAsync(year);
            return Ok(count);
        }

        [HttpGet("check-date")]
        public async Task<ActionResult<bool>> IsHoliday([FromQuery] DateTime date)
        {
            var result = await _publicHolidayService.IsDatePublicHolidayAsync(date);
            return Ok(result);
        }

        [HttpGet("check-range")]
        public async Task<ActionResult<bool>> HasHolidayInRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await _publicHolidayService.AnyHolidayInRangeAsync(start, end);
            return Ok(result);
        }
    }
}
