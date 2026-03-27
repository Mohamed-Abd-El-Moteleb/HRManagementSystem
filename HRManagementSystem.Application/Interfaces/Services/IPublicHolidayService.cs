using HRManagementSystem.Application.DTOs.PublicHoliday;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IPublicHolidayService
    {
        Task<PublicHolidayResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<PublicHolidayResponseDto>> GetAllAsync();
        Task<int> CreateAsync(CreatePublicHolidayDto dto);
        Task UpdateAsync(int id, UpdatePublicHolidayDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<PublicHolidayResponseDto>> GetUpcomingHolidaysAsync(int count);
        Task<IEnumerable<PublicHolidayResponseDto>> GetByMonthAsync(int month, int year);
        Task<IEnumerable<PublicHolidayResponseDto>> GetByYearAsync(int year);
        Task<int> GetRemainingHolidaysInYearAsync(int year);
        Task<bool> IsDatePublicHolidayAsync(DateTime date);
        Task<int> GetTotalHolidayDaysInMonthAsync(int month, int year);
        Task<bool> AnyHolidayInRangeAsync(DateTime startDate, DateTime endDate);
    }
}
