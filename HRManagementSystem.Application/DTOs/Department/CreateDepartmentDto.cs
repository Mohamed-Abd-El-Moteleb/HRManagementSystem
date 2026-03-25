using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Department
{
    public class CreateDepartmentDto
    {
    
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Code { get; set; }


    }
}
