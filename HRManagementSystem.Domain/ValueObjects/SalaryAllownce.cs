using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.ValueObjects
{
    public class SalaryAllowance
    {
        public string Name { get; private set; }
        public Money Amount { get; private set; }
        public bool IsManual { get; private set; }

        private SalaryAllowance() { }
        public SalaryAllowance(string name, Money amount, bool isManual)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new BusinessException("Allowance name is required.");
            Name = name;
            Amount = amount;
            IsManual = isManual;
        }
    }
}
