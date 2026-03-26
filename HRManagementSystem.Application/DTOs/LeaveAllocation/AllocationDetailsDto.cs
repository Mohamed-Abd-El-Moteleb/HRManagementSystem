using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.LeaveAllocation
{
    public class AllocationDetailsDto
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public string LeaveType { get; set; }
        public int TotalDays { get; set; }
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
    }
}
