using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.LeaveRequest
{
    public class LeaveRequestDetailsDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public LeaveType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInDays { get; set; }
        public LeaveStatus Status { get; set; }
        public string? Reason { get; set; }
        public string? ManagerComment { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
