using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Employee
{
    public class EmployeeFilterRequest
    {
        public string? SearchTerm { get; set; } 
        public int? EmployeeCode { get; set; } 
        public int? DepartmentId { get; set; }
        public string? JobTitle { get; set; }
        public EmploymentStatus? Status { get; set; }

        public DateTime? HiredDateFrom { get; set; } 
        public DateTime? HiredDateTo { get; set; }   

        public decimal? MinSalary { get; set; } 
        public decimal? MaxSalary { get; set; } 

        public Gender? Gender { get; set; } 

        // (Ordering)
        public string? SortBy { get; set; } 
        public bool IsAscending { get; set; } = true;

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
