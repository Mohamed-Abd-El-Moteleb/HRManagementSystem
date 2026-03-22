using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.ValueObjects
{
    public class ContractDetails
    {
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ContractType ContractType { get; private set; } 

        private ContractDetails() { }

        public ContractDetails(DateTime startDate, DateTime? endDate, ContractType contractType)
        {
            if (endDate.HasValue && endDate <= startDate)
                throw new ArgumentException("End date must be after start date.");

            if (!Enum.IsDefined(typeof(ContractType), contractType))
                throw new ArgumentException("Invalid contract type.", nameof(contractType));

            StartDate = startDate;
            EndDate = endDate;
            ContractType = contractType;
        }

        public bool IsActive() => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
    }
}
