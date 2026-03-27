using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Attendance
{
    public class AttendanceStatisticsDto
    {
        public int EmployeeId { get; set; }
        public int TotalPresentDays { get; set; }
        public int TotalAbsentDays { get; set; }
        public int TotalLateDays { get; set; }
        public int TotalOnLeavefDays { get; set; }
        public int TotalHasLeftEarlyDays { get; set; }
        public int TotalOvertimeDays { get; set; }
        public int TotalRemoteWorkDays { get; set; }
        public int TotalBusinessTripDays { get; set; }
        public int TotalPublicHolidayWorkDays { get; set; }
        public double TotalWorkingHours { get; set; }
        public double OvertimeHours { get; set; }

    }
}
