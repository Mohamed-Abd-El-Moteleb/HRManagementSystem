using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.SalarySlip
{
    public class SalarySlipDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal BaseSalary { get; set; }
        public List<AllowanceDto> Allowances { get; set; } = new();
        public decimal TotalAllowances { get; set; }
        public decimal OvertimeAmount { get; set; }
        public decimal HolidayWorkAmount { get; set; }
        public decimal Bonuses { get; set; }

        public decimal AbsenceDeduction { get; set; }
        public decimal LateDeduction { get; set; } 
        public decimal InsuranceDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal ManualDeductions { get; set; }
        public decimal TotalDeductions { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public string Currency { get; set; } = "EGP";

        public bool IsPaid { get; set; }
        public DateTime CalculationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }
    }
}
