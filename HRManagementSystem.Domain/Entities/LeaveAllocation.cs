using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class LeaveAllocation
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public virtual Employee Employee { get; private set; }

        public int Year { get; private set; }
        public LeaveType LeaveType { get; private set; }

        public int TotalDays { get; private set; } 
        public int UsedDays { get; private set; }  

        public int RemainingDays => TotalDays - UsedDays;

        private LeaveAllocation() { }

        public static LeaveAllocation Create(int employeeId, int year, LeaveType type, int totalDays)
        {
            if (employeeId <= 0)
                throw new BusinessException("Valid Employee ID is required.");

            if (year < DateTime.Now.Year)
                throw new BusinessException("Cannot allocate leaves for past years.");

            if (totalDays <= 0)
                throw new BusinessException("Total days must be greater than zero.");

            return new LeaveAllocation
            {
                EmployeeId = employeeId,
                Year = year,
                LeaveType = type,
                TotalDays = totalDays,
                UsedDays = 0
            };
        }

        public void DeductDays(int days)
        {
            if (days <= 0)
                throw new BusinessException("Days to deduct must be positive.");

            if (days > RemainingDays)
                throw new BusinessException($"Insufficient leave balance. Remaining: {RemainingDays}, Requested: {days}");

            UsedDays += days;

        }
        public void RestoreDays(int days)
        {
            if (days <= 0)
                throw new BusinessException("Days to restore must be positive.");

            if (UsedDays - days < 0)
                UsedDays = 0;
            else
                UsedDays -= days;
        }

        public void UpdateTotalDays(int newTotal)
        {
            if (newTotal < UsedDays)
                throw new BusinessException("New total cannot be less than days already used.");

            TotalDays = newTotal;
        }
    }
}
