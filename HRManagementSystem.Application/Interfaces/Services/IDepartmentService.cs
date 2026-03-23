using HRManagementSystem.Application.DTOs.Department;
using HRManagementSystem.Application.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDetailsDto> GetByIdAsync(int id);
        Task<DepartmentDetailsDto> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentIdAsync(int departmentId);
        Task CreateAsync(CreateDepartmentDto dto);
        Task UpdateAsync(int id , UpdateDepartmentDto dto);
        Task AssignManagerAsync(int departmentId, int managerId);
        Task RemoveManagerAsync(int departmentId);
        Task AddEmployeeAsync(int departmentId, int employeeId);
        Task RemoveEmployeeAsync(int departmentId, int employeeId);
        Task ActivateDepartmentAsync(int departmentId);
        Task DeactivateDepartmentAsync(int departmentId);
        Task<DepartmentStatsDto> GetDepartmentStatsAsync(int departmentId);
        Task DeleteAsync(int id);

    }

}
