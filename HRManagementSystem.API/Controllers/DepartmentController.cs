using HRManagementSystem.Application.DTOs.Department;
using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            return Ok(department);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetByIdWithImportnatDetails(int id)
        {
            var department = await _departmentService.GetByIdWithDetailsAsync(id);
            return Ok(department);
        }

        [HttpGet("{id}/employees")]
        public async Task<IActionResult> GetEmployeesByDepartmentId(int id)
        {
            var departments = await _departmentService.GetEmployeesByDepartmentIdAsync(id);
            return Ok(departments);
        }
        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetDepartmentStats(int id)
        {
            var department = await _departmentService.GetDepartmentStatsAsync(id);
            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Name }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
        {
            await _departmentService.UpdateAsync(id, dto);
            return Ok("Department updated successfully");
        }

        [HttpPut("{id}/assign-manager/{managerId}")]
        public async Task<IActionResult> AssignManager(int id, int managerId)
        {
            await _departmentService.AssignManagerAsync(id, managerId);
            return Ok("Manager Assigned successfully");
        }

        [HttpPut("{id}/remove-manager")]
        public async Task<IActionResult> RemoveManager(int id)
        {
            await _departmentService.RemoveManagerAsync(id);
            return Ok("Manager Remove successfully");
        }

        [HttpPut("{id}/add-employee/{employeeId}")]
        public async Task<IActionResult> AddEmployeeToDepartment(int id, int employeeId)
        {
            await _departmentService.AddEmployeeAsync(id, employeeId);
            return Ok("Employee Added successfully");
        }

        [HttpPut("{id}/remove-employee/{employeeId}")]
        public async Task<IActionResult> RemoveEmployeeFromDepartment(int id, int employeeId)
        {
            await _departmentService.RemoveEmployeeAsync(id, employeeId);
            return Ok("Employee Removed successfully");
        }

        [HttpPut("{id}/activate-department")]
        public async Task<IActionResult> ActivateDepartment(int id)
        {
            await _departmentService.ActivateDepartmentAsync(id);
            return Ok("Department Activated successfully");
        }
        [HttpPut("{id}/deactivate-department")]
        public async Task<IActionResult> DeactivateDepartment(int id)
        {
            await _departmentService.DeactivateDepartmentAsync(id);
            return Ok("Department Deactivated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);
            return Ok("Department deleted successfully");
        }
    }
}
