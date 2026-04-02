using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.DTOs.Employee
{
    public class FixedAllowanceRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        public decimal Amount { get; set; } 

        [Required]
        [RegularExpression("^[A-Z]{3}$")]
        public string Currency { get; set; } = "EGP"; 
    }
}
