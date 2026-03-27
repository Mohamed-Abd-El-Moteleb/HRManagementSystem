using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
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

        public async Task<(IEnumerable<Employee> Data, int TotalCount)> SearchEmployeesAsync(EmployeeFilterRequest filter)
        {
            var query = _context.Employees.Include(e => e.Department).AsQueryable();

            // (SearchTerm)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(e => e.FullName.FirstName.Contains(filter.SearchTerm) ||
                                         e.FullName.LastName.Contains(filter.SearchTerm) ||
                                         e.ContactInfo.Email.Contains(filter.SearchTerm) ||
                                         e.Id==filter.EmployeeCode);
            }

            if (filter.HiredDateFrom.HasValue)
                query = query.Where(e => e.HireDate >= filter.HiredDateFrom.Value);

            if (filter.HiredDateTo.HasValue)
                query = query.Where(e => e.HireDate <= filter.HiredDateTo.Value);

            if (filter.MinSalary.HasValue)
                query = query.Where(e => e.Salary.Amount >= filter.MinSalary.Value);

            if (filter.MaxSalary.HasValue)
                query = query.Where(e => e.Salary.Amount <= filter.MaxSalary.Value);

            if (filter.Gender.HasValue)
                query = query.Where(e => e.Gender == filter.Gender.Value);

            if (filter.Status.HasValue)
            {
                query = query.Where(e => e.Status == filter.Status.Value);
            }

            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.IsAscending ? query.OrderBy(e => e.FullName.FirstName) : query.OrderByDescending(e => e.FullName.FirstName),
                "hiredate" => filter.IsAscending ? query.OrderBy(e => e.HireDate) : query.OrderByDescending(e => e.HireDate),
                "salary" => filter.IsAscending ? query.OrderBy(e => e.Salary) : query.OrderByDescending(e => e.Salary),
                _ => query.OrderBy(e => e.Id) 
            };

            int totalCount = await query.CountAsync();

            var data = await query.Skip((filter.Page - 1) * filter.PageSize)
                                  .Take(filter.PageSize)
                                  .ToListAsync();

            return (data, totalCount);
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

       
    }
}
