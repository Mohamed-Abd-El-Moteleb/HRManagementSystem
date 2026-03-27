using AutoMapper;
using HRManagementSystem.Application.DTOs.PublicHoliday;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Exceptions;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services
{
    public class PublicHolidayService:IPublicHolidayService
    {

        private readonly IPublicHolidayRepository _publicHolidayRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PublicHolidayService(
            IPublicHolidayRepository publicHolidayRepository , 
            IUnitOfWork unitOfWork , 
            IMapper mapper) 
        {
            _publicHolidayRepository = publicHolidayRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PublicHolidayResponseDto> GetByIdAsync(int id)
        {
            var holiday = await _publicHolidayRepository.GetByIdAsync(id);
            if (holiday == null)
                throw new KeyNotFoundException($"Public holiday with ID {id} not found.");
            return _mapper.Map<PublicHolidayResponseDto>(holiday);
        }

        public async Task<IEnumerable<PublicHolidayResponseDto>> GetAllAsync()
        {
            var holidays = await _publicHolidayRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PublicHolidayResponseDto>>(holidays);
        }

        public async Task<int> CreateAsync(CreatePublicHolidayDto dto)
        {
            var range = new DateRange(dto.StartDate, dto.EndDate);

            if (await _publicHolidayRepository.HasOverlapAsync(range))
                throw new BusinessException("This period overlaps with an existing public holiday.");

            var holiday = PublicHoliday.Create(dto.Name, dto.StartDate, dto.EndDate, dto.Notes, dto.IsPaid);
            await _publicHolidayRepository.AddAsync(holiday);
            await _unitOfWork.SaveChangesAsync();
            return holiday.Id;
        }

        public async Task UpdateAsync(int id, UpdatePublicHolidayDto dto)
        {
            var holiday = await _publicHolidayRepository.GetByIdAsync(id);
            if (holiday == null)
                throw new KeyNotFoundException($"Public holiday with ID {id} not found.");

            var newRange = new DateRange(dto.StartDate, dto.EndDate);

            if (await _publicHolidayRepository.HasOverlapAsync(newRange, id))
                throw new BusinessException("The updated period overlaps with another public holiday.");

            holiday.UpdatePeriod(dto.StartDate, dto.EndDate);
            holiday.UpdateDetails(dto.Name, dto.Notes, dto.IsPaid);

            _publicHolidayRepository.Update(holiday);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var holiday = await _publicHolidayRepository.GetByIdAsync(id);
            if (holiday == null)
                throw new NotFoundException($"Public holiday with ID {id} not found.");

            _publicHolidayRepository.Delete(holiday);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PublicHolidayResponseDto>> GetUpcomingHolidaysAsync(int count)
        {
            var holidays = await _publicHolidayRepository.GetUpcomingHolidaysAsync(count);
            return _mapper.Map<IEnumerable<PublicHolidayResponseDto>>(holidays);
        }

        public async Task<IEnumerable<PublicHolidayResponseDto>> GetByMonthAsync(int month, int year)
        {
            var holidays = await _publicHolidayRepository.GetHolidaysByMonthAsync(month, year);
            return _mapper.Map<IEnumerable<PublicHolidayResponseDto>>(holidays);
        }

        public async Task<IEnumerable<PublicHolidayResponseDto>> GetByYearAsync(int year)
        {
            var allHolidays = await _publicHolidayRepository.GetAllAsync();
            var yearHolidays = allHolidays.Where(h => h.Year == year);
            return _mapper.Map<IEnumerable<PublicHolidayResponseDto>>(yearHolidays);
        }

        public async Task<int> GetRemainingHolidaysInYearAsync(int year)
        {
            var holidays = await _publicHolidayRepository.GetAllAsync();
            return holidays.Count(h => h.Year == year && h.Period.StartDate >= DateTime.Today);
        }

        public async Task<bool> IsDatePublicHolidayAsync(DateTime date)
        {
            return await _publicHolidayRepository.IsPublicHolidayAsync(date);
        }

        public async Task<int> GetTotalHolidayDaysInMonthAsync(int month, int year)
        {
            var holidays = await _publicHolidayRepository.GetHolidaysByMonthAsync(month, year);
            return holidays.Sum(h => h.TotalDays);
        }

        public async Task<bool> AnyHolidayInRangeAsync(DateTime startDate, DateTime endDate)
        {
            var range = new DateRange(startDate, endDate);
            return await _publicHolidayRepository.AnyHolidayInRangeAsync(range);
        }


    }
}
