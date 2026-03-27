using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceFilterRequest
    {
            public int? EmployeeId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public AttendanceStatus? Status { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 20;
        
    }
}
