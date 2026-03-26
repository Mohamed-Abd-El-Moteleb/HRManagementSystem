using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.LeaveAllocation
{
    public class CreateAllocationDto
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public LeaveType LeaveType { get; set; }
        public int TotalDays { get; set; }
    }
}
