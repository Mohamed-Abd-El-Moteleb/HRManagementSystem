using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

        // Basic Info
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }

        // Address & Identity
        public string? City { get; set; }
        public string? Street { get; private set; }
        public string? BuildingNumber { get; private set; }
        public string? NationalId { get; set; }

        // Employment Details
        public DateTime? DateOfBirth { get; set; }
        public string? JobTitle { get; set; }
        public string? Status { get; set; }
        public string? JobLevel { get; set; }

        // Contract Details
        public DateTime? ContractEndDate { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public string? ContractType { get; set; }

        // Bank Info
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IBAN { get; set; }

        // Organization Info
        public int DepartmentId { get; set; }

        // Financial Info
        public decimal? Salary { get; set; }
        public string? SalaryCurrancy { get; set; } = string.Empty;


        // Extra
        public string? ProfileImagePath { get; set; }
    }
}
