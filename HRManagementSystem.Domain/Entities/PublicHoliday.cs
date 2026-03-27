using HRManagementSystem.Domain.Exceptions;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class PublicHoliday
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateRange Period { get; private set; }
        public string? Notes { get; set; }
        public int TotalDays => Period.TotalDays;
        public int Year => Period.StartDate.Year;
        public bool Includes(DateTime date) => Period.Includes(date);
        public bool IsPaid { get; set; } = true;

        private PublicHoliday() { }

        public static PublicHoliday Create(string name, DateTime startDate, DateTime endDate, string? notes, bool isPaid = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Holiday name is required.");

           var period = new DateRange(startDate, endDate);

            return new PublicHoliday
            {
                Name = name,
                Period = period,
                Notes = notes,
                IsPaid = isPaid
            };
        }

        public void UpdatePeriod(DateTime newStart, DateTime newEnd)
        {
            Period = new DateRange(newStart, newEnd);
        }

        public void UpdateDetails(string name, string? notes, bool isPaid)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Holiday name is required.");

            Name = name;
            Notes = notes;
            IsPaid = isPaid;
        }
    }
}
