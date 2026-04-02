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
    public class SalarySlipRepository:ISalarySlipRepository
    {
        private readonly AppDbContext _context;

        public SalarySlipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalarySlip?> GetByIdAsync(int id)
        {
            return await _context.SalarySlips
                .Include(s => s.DetailedAllowances).Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SalarySlip?> GetByEmployeeAndPeriodAsync(int employeeId, int month, int year)
        {
            return await _context.SalarySlips
                .Include(s => s.DetailedAllowances)
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);
        }

        public async Task<IEnumerable<SalarySlip>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.SalarySlips
                .Include(s => s.DetailedAllowances)
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.Year).ThenByDescending(s => s.Month)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<SalarySlip>> GetAllByPeriodAsync(int month, int year)
        {
            return await _context.SalarySlips
                .Include(s => s.DetailedAllowances)
                .Include(s => s.Employee)
                .Where(s => s.Month == month && s.Year == year)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<SalarySlip>> GetUnpaidSlipsAsync(int month, int year)
        {
            return await _context.SalarySlips
                .Where(s => s.Month == month && s.Year == year && !s.IsPaid)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int employeeId, int month, int year)
        {
            return await _context.SalarySlips
                .AnyAsync(s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);
        }

        public async Task AddAsync(SalarySlip salarySlip)
        {
            await _context.SalarySlips.AddAsync(salarySlip);
        }

        public async Task AddRangeAsync(IEnumerable<SalarySlip> salarySlips)
        {
            await _context.SalarySlips.AddRangeAsync(salarySlips);
        }

        public void Update(SalarySlip salarySlip)
        {
            _context.SalarySlips.Update(salarySlip);
        }

        public void Remove(SalarySlip salarySlip)
        {
            _context.SalarySlips.Remove(salarySlip);
        }

    }
}
