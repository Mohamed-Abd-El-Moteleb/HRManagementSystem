using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {

        // Department Basic Info
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }

        // Status
        public bool? IsActive { get; set; }

        // Manager Info
        public int? ManagerId { get; set; }
    }
}

