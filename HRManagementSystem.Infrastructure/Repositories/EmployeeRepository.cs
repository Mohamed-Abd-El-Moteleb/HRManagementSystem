using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repositories
{
    public class EmployeeRepository:IEmployeeRepository
    {
        // Implementation of employee repository methods would go here
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
           return await _context.Employees.Include(e=>e.Department).FirstOrDefaultAsync(e=>e.Id==id);
        }
        public async Task<IEnumerable<Employee>> GetAllAsync() 
        {
            return await _context.Employees.Include(e=>e.Department).ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .ToListAsync();
        }

        public async Task AddAsync(Employee employee) 
        {
            await _context.Employees.AddAsync(employee);
        }

        public void Update(Employee employee)
        {
             _context.Employees.Update(employee);
        }
        public void Delete(Employee employee)
        {
            _context.Employees.Remove(employee);
        }

        public async Task<bool> ExistsByEmailOrNationalIdAsync(string email, string nationalId)
        {
            bool exists = await _context.Employees.AnyAsync(e => e.ContactInfo.Email == email || e.NationalId.NationalId == nationalId);
            return exists;
        }

    }
}
