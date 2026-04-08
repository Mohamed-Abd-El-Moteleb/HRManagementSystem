using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HRManagementSystem.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
