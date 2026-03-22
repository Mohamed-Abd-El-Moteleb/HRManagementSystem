using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.ValueObjects
{
    public class BankAccount
    {
        public string BankName { get; private set; }
        public string AccountNumber { get; private set; }
        public string? IBAN { get; private set; }

        private BankAccount() { }

        public BankAccount(string accountNumber, string bankName,  string iban)
        {
            if (string.IsNullOrWhiteSpace(bankName))
                throw new ArgumentException("Bank name is required.", nameof(bankName));
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.", nameof(accountNumber));
       

            BankName = bankName;
            AccountNumber = accountNumber;
            IBAN = iban;
        }
    }
}
