using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Department
{
    public class DepartmentStatsDto
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int EmployeesCount { get; set; }
        public decimal TotalSalary { get; set; }
        public string? ManagerName { get; set; }
    }
}
