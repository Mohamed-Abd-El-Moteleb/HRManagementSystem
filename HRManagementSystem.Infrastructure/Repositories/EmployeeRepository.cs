using HRManagementSystem.Application.DTOs.Employee;
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

        public async Task<bool> ExistsByEmailOrNationalIdAsync(string? email, string? nationalId)
        {
            return await _context.Employees.AnyAsync(e =>
                (!string.IsNullOrEmpty(email) && e.ContactInfo.Email == email) ||
                (!string.IsNullOrEmpty(nationalId) && e.NationalId.NationalId == nationalId));
        }

        public async Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeFilterDto filter)
        {
            var query = _context.Employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(e =>
                    e.FullName.FirstName.Contains(filter.SearchTerm) ||
                    e.FullName.LastName.Contains(filter.SearchTerm) ||
                    e.ContactInfo.Email.Contains(filter.SearchTerm) ||
                    e.NationalId.NationalId.Contains(filter.SearchTerm));
            }

            if (filter.DepartmentId.HasValue)
                query = query.Where(e => e.DepartmentId == filter.DepartmentId);

            if (filter.Gender.HasValue)
                query = query.Where(e => e.Gender == filter.Gender);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.FullName.FirstName)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
