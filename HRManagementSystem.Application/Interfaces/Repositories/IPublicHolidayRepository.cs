using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface IPublicHolidayRepository
    {
        Task<IEnumerable<PublicHoliday>> GetAllAsync();
        Task<PublicHoliday?> GetByIdAsync(int id);
        Task AddAsync(PublicHoliday holiday);
        void Update(PublicHoliday holiday);
        void Delete(PublicHoliday holiday);
        Task<bool> IsPublicHolidayAsync(DateTime date);
        Task<bool> HasOverlapAsync(DateRange dateRange, int? excludeId = null);
        Task<IEnumerable<PublicHoliday>> GetHolidaysByMonthAsync(int month, int year);
        Task<IEnumerable<PublicHoliday>> GetUpcomingHolidaysAsync(int count);
        Task<bool> AnyHolidayInRangeAsync(DateRange dateRange);
    }
}
