using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Attendance
{
    public class BulkAdjustmentRequest
    {
        public List<int> AttendanceIds { get; set; } = new();
        public AttendanceStatus NewStatus { get; set; }
        public string? Notes { get; set; }
    }
}
