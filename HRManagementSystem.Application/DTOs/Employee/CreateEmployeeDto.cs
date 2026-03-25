using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        // Basic info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;

        // Identity info
        public string NationalId { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        // Address info
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;

        // Employment info
        public string JobTitle { get; set; }
        public string JobLevel { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Salary { get; set; }
        public string SalaryCurrancy { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        // Contract & Bank 
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string? Iban { get; set; } = string.Empty;
        public string ContractType { get; set; } = "FullTime";
        public DateTime? ContractEndDate { get; set; }
        public DateTime ContractStartDate { get; set; }
    }
}
