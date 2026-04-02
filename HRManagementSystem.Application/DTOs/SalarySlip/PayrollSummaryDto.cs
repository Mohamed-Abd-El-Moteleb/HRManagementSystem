using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.SalarySlip
{
    public class PayrollSummaryDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalEmployees { get; set; }

        public decimal TotalGrossSalaries { get; set; } 
        public decimal TotalNetSalaries { get; set; }  

        public decimal TotalTaxLiability { get; set; }      
        public decimal TotalInsuranceLiability { get; set; } 

        public int PaidSlipsCount { get; set; }
        public int PendingSlipsCount { get; set; }

        public string Currency { get; set; } = "EGP";
    }
}
