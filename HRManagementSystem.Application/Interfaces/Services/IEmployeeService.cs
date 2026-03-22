using HRManagementSystem.Application.DTOs.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDetailsDto> GetByIdAsync(int id);
        Task CreateAsync(CreateEmployeeDto dto);
        Task UpdateAsync(int id,UpdateEmployeeDto dto);
        Task DeleteAsync(int id);
    }
}
