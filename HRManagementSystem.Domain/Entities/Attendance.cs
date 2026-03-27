using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRManagementSystem.Domain.Entities
{
    public class Attendance
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public Employee Employee { get; private set; }

        public DateTime Date { get; private set; }
        public DateTime? CheckIn { get; private set; }
        public DateTime? CheckOut { get; private set; }

        public AttendanceStatus Status { get; set; } // for manual adjustments by HR or managers
        public string? Notes { get; set; }

        public Attendance(int employeeId, DateTime date)
        {
            EmployeeId = employeeId;
            Date = date.Date;
            Status = AttendanceStatus.Absent;
        }

        public void RecordCheckIn(DateTime checkInTime, TimeSpan shiftStartTime, int gracePeriodMinutes)
        {
            CheckIn = checkInTime;

            TimeSpan allowedArrivalTime = shiftStartTime.Add(TimeSpan.FromMinutes(gracePeriodMinutes));
            TimeSpan earlyArrivalThreshold = shiftStartTime.Subtract(TimeSpan.FromMinutes(30));

            if (checkInTime.TimeOfDay > allowedArrivalTime)
            {
                Status = AttendanceStatus.Late;
                Notes = $"Checked in late at {checkInTime.TimeOfDay:hh\\:mm}";
            }
            else if (checkInTime.TimeOfDay < earlyArrivalThreshold)
            {
                Status = AttendanceStatus.Present; 
                Notes = $"Checked in early at {checkInTime.TimeOfDay:hh\\:mm}"; 
            }
            else
            {
                Status = AttendanceStatus.Present;
            }
        }

        public void RecordCheckOut(DateTime checkOutTime, TimeSpan shiftEndTime, TimeSpan shiftDuration)
        {
            if (CheckIn == null)
                throw new InvalidOperationException("Cannot record checkout: No check-in record found.");

            CheckOut = checkOutTime;
            var actualDuration = checkOutTime - CheckIn.Value;

            string baseNotes = string.IsNullOrEmpty(Notes) ? "" : Notes + " | ";

            bool wasLate = Status == AttendanceStatus.Late;
            string lateStatus = wasLate ? "Yes" : "No";

            if (checkOutTime.TimeOfDay < shiftEndTime)
            {
                Status = AttendanceStatus.HasLeftEarly;
                Notes = $"{baseNotes}Left early at {checkOutTime.TimeOfDay:hh\\:mm}";
            }
            else if (actualDuration > shiftDuration)
            {
                Status = AttendanceStatus.Overtime;
                Notes = $"{baseNotes}Worked overtime until {checkOutTime.TimeOfDay:hh\\:mm}. (Late arrival: {lateStatus})";
            }
            else
            {
                Status = wasLate ? AttendanceStatus.Late : AttendanceStatus.Present;
                Notes = $"{baseNotes}Checked out at {checkOutTime.TimeOfDay:hh\\:mm}";
            }
        }
    }

    
}
