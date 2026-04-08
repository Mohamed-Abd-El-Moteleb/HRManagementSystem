using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Application.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = "Admin,HR,Manager")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] EmployeeFilterRequest filter)
        {
            var result = await _employeeService.SearchEmployeesAsync(filter);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin,HR,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return Ok(employee);
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            var employeeId= await _employeeService.CreateAsync(dto);
            return CreatedAtAction(actionName: nameof(GetById),
            routeValues: new { id = employeeId }, 
            value: new { message = "Employee created successfully", data = dto });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPost("{empId}/fixed-allowances")]
        public async Task<IActionResult> AddFixedAllowance(int empId, [FromBody] FixedAllowanceRequest request)
        {
            var amount = new Money(request.Amount, request.Currency);
            await _employeeService.AddPermanentAllowanceAsync(empId, amount, request.Name);
            return Ok(new { message = "Fixed Allowance added successfully " });
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)
        {
            await _employeeService.UpdateAsync(id, dto);
            return Ok("Employee updated successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/promote")]
        public async Task<IActionResult> Promote(int id)
        {
            await _employeeService.PromoteAsync(id);
            return Ok("Employee Promoted successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/demote")]
        public async Task<IActionResult> Demote(int id)
        {
            await _employeeService.DemoteAsync(id);
            return Ok("Employee Demoted successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/terminate")]
        public async Task<IActionResult> Terminate(int id)
        {
            await _employeeService.TerminateAsync(id);
            return Ok("Employee Terminated successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            await _employeeService.ActivateAsync(id);
            return Ok("Employee Activated successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/setOnLeave")]
        public async Task<IActionResult> SetOnLeave(int id)
        {
            await _employeeService.SetOnLeaveAsync(id);
            return Ok("Employee Set On Leave successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/resign")]
        public async Task<IActionResult> Resign(int id)
        {
            await _employeeService.ResignAsync(id);
            return Ok("Employee Resigned successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/changeJobTitle")]
        public async Task<IActionResult> ChangeJobTitle(int id, [FromQuery] string jobTitle)
        {
            await _employeeService.ChangeJobTitleAsync(id,jobTitle);
            return Ok("Employee Job Title Changed successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/assignToDepartment/{departmentId}")]
        public async Task<IActionResult> AssignToDepartment(int id, int departmentId)
        {
            await _employeeService.AssignToDepartmentAsync(id,departmentId);
            return Ok("Employee Department Changed successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/unassignFromDepartment")]
        public async Task<IActionResult> UnassignFromDepartment(int id)
        {
            await _employeeService.UnassignFromDepartmentAsync(id);
            return Ok("Employee Department Changed successfully");
        }

        [HttpPatch("{id}/updateAddress")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] Address address)
        {
            await _employeeService.UpdateAddressAsync(id, address);
            return Ok("Employee Address Changed successfully");
        }

        [HttpPatch("{id}/updateContactInfo")]
        public async Task<IActionResult> UpdateContactInfo(int id, [FromBody] ContactInfo contactInfo)
        {
            await _employeeService.UpdateContactInfoAsync( id, contactInfo);
            return Ok("Employee Contact Info Changed successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpPatch("{id}/AdjustSalary")]
        public async Task<IActionResult> AdjustSalary(int id, [FromBody] Money money, bool increase = true)
        {
            await _employeeService.AdjustSalaryAsync(id, money, increase);
            return Ok("Employee Salary Adjusted successfully");
        }

        [Authorize(Roles = "Admin,HR")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }

    }
}
