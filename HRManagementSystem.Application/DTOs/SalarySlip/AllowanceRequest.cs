using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.SalarySlip
{
    public class AllowanceRequest
    {
        [Required]
        public string Reason { get; set; } 

        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "EGP";
    }
}
