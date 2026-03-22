using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repositories
{
    public class DepartmentRepository: IDepartmentRepository
    {
        // Implementation of department repository methods would go here
        private readonly AppDbContext _context;
        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .ToListAsync();
        }
        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }
        public void Update(Department department)
        {
            _context.Departments.Update(department);
        }
        public void Delete(Department department)
        {
            _context.Departments.Remove(department);
        }

        public async Task<bool> AnyAsync(Expression<Func<Department, bool>> predicate)
        {
            return await _context.Departments.AnyAsync(predicate);
        }
    }
}
