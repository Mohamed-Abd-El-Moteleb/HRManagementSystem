using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.ValueObjects;
using HRManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repositories
{
    public class PublicHolidayRepository:IPublicHolidayRepository
    {
        private readonly AppDbContext _context;
        public PublicHolidayRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PublicHoliday>> GetAllAsync()
        {
             return await _context.PublicHolidays.AsNoTracking().ToListAsync();
        }

        public async Task<PublicHoliday?> GetByIdAsync(int id)
        {
             return await _context.PublicHolidays.FindAsync(id);
        }

        public async Task AddAsync(PublicHoliday holiday)
        {
             await _context.PublicHolidays.AddAsync(holiday);
        }
        public void Update(PublicHoliday holiday)
        {
            _context.PublicHolidays.Update(holiday);
        }
        public void Delete(PublicHoliday holiday)
        {
            _context.PublicHolidays.Remove(holiday);
        }

        public async Task<bool> IsPublicHolidayAsync(DateTime date)
        {
            return await _context.PublicHolidays.AnyAsync(h => date.Date >= h.Period.StartDate && date.Date <= h.Period.EndDate);
        }

        public async Task<bool> HasOverlapAsync(DateRange range, int? excludeId = null)
        {
            var query = _context.PublicHolidays.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(h => h.Id != excludeId.Value);

            return await query.AnyAsync(h =>
                range.StartDate <= h.Period.EndDate && range.EndDate >= h.Period.StartDate);
        }
        public async Task<IEnumerable<PublicHoliday>> GetHolidaysByMonthAsync(int month, int year)
        {
            return await _context.PublicHolidays.AsNoTracking()
                .Where(h => (h.Period.StartDate.Month == month && h.Period.StartDate.Year == year) ||
                            (h.Period.EndDate.Month == month && h.Period.EndDate.Year == year))
                .OrderBy(h => h.Period.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PublicHoliday>> GetUpcomingHolidaysAsync(int count)
        {
            return await _context.PublicHolidays.AsNoTracking()
                .Where(h => h.Period.StartDate >= DateTime.Today)
                .OrderBy(h => h.Period.StartDate)
                .Take(count)
                .ToListAsync();
        }
        public async Task<bool> AnyHolidayInRangeAsync(DateRange range)
        {
            return await _context.PublicHolidays.AnyAsync(h =>
                range.StartDate <= h.Period.EndDate && range.EndDate >= h.Period.StartDate);
        }

    }
}
