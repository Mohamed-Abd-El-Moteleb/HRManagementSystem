using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Employee
{
    public class EmployeeDetailsDto
    {
        public int Id { get; set; }

        // Basic Info
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }

        // Address & Identity
        public string Address { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;

        // Employment Details
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public DateTime HireDate { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string JobLevel { get; set; } = string.Empty;

        // Contract Details
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContractType { get; set; } = string.Empty;

        // Bank Info
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;

        // Organization Info
        public string DepartmentName { get; set; } = string.Empty;

        // Financial Info
        public decimal Salary { get; set; }
        public string SalaryCurrancy { get; set; } = string.Empty;


        // Extra
        public string? ProfileImagePath { get; set; }

    }
}
