using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public LeaveType Type { get; private set; } // Annual, Sick, Casual
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string? Reason { get; private set; }
        public string? ManagerComment { get; private set; }
        public LeaveStatus Status { get; private set; } = LeaveStatus.Pending;
        public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
        public int DurationInDays => (EndDate.Date - StartDate.Date).Days + 1;
        public virtual Employee Employee { get; private set; }
        private LeaveRequest() { }
        public static LeaveRequest Create(int employeeId, LeaveType type, DateTime startDate, DateTime endDate, string? reason)
        {
            if (employeeId <= 0)
                throw new BusinessException("Valid Employee ID is required.");

            if (startDate.Date < DateTime.Today)
                throw new BusinessException("Start date cannot be in the past.");

            if (endDate.Date < startDate.Date)
                throw new BusinessException("End date cannot be before start date.");

            return new LeaveRequest
            {
                EmployeeId = employeeId,
                Type = type,
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                Reason = reason,
                Status = LeaveStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };
        }

        public void Approve() 
        {
            if (Status != LeaveStatus.Pending)
                throw new BusinessException("Only pending requests can be approved.");
            Status = LeaveStatus.Approved;
        }
        public void Reject(string? Comment) 
        {
            if (Status != LeaveStatus.Pending)
                throw new BusinessException("Only pending requests can be rejected.");
            Status = LeaveStatus.Rejected;
            ManagerComment = Comment;
        }
        public void Cancel()
        {
            if (Status == LeaveStatus.Rejected || Status == LeaveStatus.Cancelled)
                throw new BusinessException("Request is already inactive.");

            if (StartDate.Date <= DateTime.Today && Status == LeaveStatus.Approved)
                throw new BusinessException("Cannot cancel an approved leave that has already started or is today.");

            Status = LeaveStatus.Cancelled;
        }

    }
}
