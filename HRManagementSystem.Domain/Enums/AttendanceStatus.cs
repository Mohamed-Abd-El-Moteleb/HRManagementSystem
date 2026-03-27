using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Enums
{
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3,
        OnLeave = 4,
        HasLeftEarly = 5,
        Overtime = 6,
        RemoteWork = 7,
        BusinessTrip = 8
    }
}
