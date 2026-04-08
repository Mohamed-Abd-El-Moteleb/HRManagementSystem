using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.DTOs.SalarySlip;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("generate/{employeeId}")]
        public async Task<ActionResult<SalarySlipDto>> Generate(int employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            var result = await _payrollService.GenerateSalarySlipAsync(employeeId, month, year);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("process-all")]
        public async Task<ActionResult<IEnumerable<SalarySlipDto>>> ProcessAll([FromQuery] int month, [FromQuery] int year)
        {
            var results = await _payrollService.ProcessCompanyPayrollAsync(month, year);
            return Ok(results);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("slips/{id}/allowances")]
        public async Task<IActionResult> AddOneMonthAllowance(int slipId, [FromBody] AllowanceRequest request)
        {
            var amount = new Money(request.Amount, request.Currency);
            await _payrollService.AddManualAllowanceAsync(slipId, amount, request.Reason);
            return NoContent();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("slips/{id}/bonus")]
        public async Task<IActionResult> AddBonus(int id, [FromBody] BonusRequest request)
        {
            var bonusMoney = new Money(request.Amount, request.Currency);

            await _payrollService.AddMonthlyBonusAsync(id, bonusMoney, request.Reason);

            return Ok(new { message = "Bonus Added Succesfully" });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("slips/{id}/deductions")]
        public async Task<IActionResult> AddManualDeduction(int id, [FromBody] DeductionRequest request)
        {
            var amount = new Money(request.Amount, request.Currency);
            await _payrollService.AddManualDeductionAsync(id, amount, request.Reason);
            return NoContent();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPut("{id}/recalculate")]
        public async Task<ActionResult<SalarySlipDto>> Recalculate(int id)
        {
            var result = await _payrollService.RecalculateAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/finalize")]
        public async Task<IActionResult> Finalize(int id)
        {
            await _payrollService.FinalizeSalarySlipAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/pay")]
        public async Task<IActionResult> MarkAsPaid(int id, [FromQuery] DateTime? paymentDate)
        {
            await _payrollService.MarkAsPaidAsync(id, paymentDate ?? DateTime.UtcNow);
            return NoContent();
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("summary")]
        public async Task<ActionResult<PayrollSummaryDto>> GetSummary([FromQuery] int month, [FromQuery] int year)
        {
            var summary = await _payrollService.GetMonthlySummaryAsync(month, year);
            return Ok(summary);
        }

        [HttpGet("slips")]
        public async Task<ActionResult<IEnumerable<SalarySlipDto>>> GetSlips([FromQuery] int month,[FromQuery] int year,[FromQuery] bool? isPaid,[FromQuery] int? employeeId)
        {
            var results = await _payrollService.GetSlipsAsync(month, year, isPaid, employeeId);
            return Ok(results);
        }

        [HttpGet("slips/{id}")]
        public async Task<ActionResult<SalarySlipDto>> GetById(int id)
        {
            var result = await _payrollService.GetSlipByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpGet("missing-employees")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetMissing([FromQuery] int month,[FromQuery] int year)
        {
            var results = await _payrollService.GetMissingEmployeesForPayrollAsync(month, year);
            return Ok(results);
        }

        public record BonusRequest(decimal Amount, string Currency, string Reason);
        public record DeductionRequest(decimal Amount, string Currency, string Reason);
    }
}
