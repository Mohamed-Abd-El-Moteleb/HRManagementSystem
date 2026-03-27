using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task AddAsync(Employee employee);
        void Update(Employee employee);
        void Delete(Employee employee);
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int departmentId);
        Task<bool> ExistsByEmailOrNationalIdAsync(string? email, string? nationalId);

        Task<(IEnumerable<Employee> Data, int TotalCount)> SearchEmployeesAsync(EmployeeFilterRequest filter);
    }
}
