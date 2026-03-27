using HRManagementSystem.Application.Common;
using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<Result<IEnumerable<EmployeeDto>>> SearchEmployeesAsync(EmployeeFilterRequest filter);
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDetailsDto> GetByIdAsync(int id);
        Task<Employee> GetEmployeeOrThrowAsync(int id);
        Task<int> CreateAsync(CreateEmployeeDto dto);
        Task UpdateAsync(int id, UpdateEmployeeDto dto);
        Task DeleteAsync(int id);
        Task PromoteAsync(int id);
        Task DemoteAsync(int id);
        Task TerminateAsync(int id);
        Task ActivateAsync(int id);
        Task SetOnLeaveAsync(int id);
        Task ResignAsync(int id);
        Task ChangeJobTitleAsync(int id, string jobTitle);
        Task AssignToDepartmentAsync(int id, int departmentId);
        Task UnassignFromDepartmentAsync(int id);
        Task UpdateAddressAsync(int id, Address address);
        Task UpdateContactInfoAsync(int id, ContactInfo contactInfo);
        Task AdjustSalaryAsync(int id, Money money, bool increase = true);
    }
        
}
