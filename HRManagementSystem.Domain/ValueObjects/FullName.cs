using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.ValueObjects
{
    public class FullName
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private FullName() { } 

        public FullName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2)
                throw new ArgumentException("First name must be at least 2 characters.");
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2)
                throw new ArgumentException("Last name must be at least 2 characters.");
            FirstName = firstName;
            LastName = lastName;
        }
        public override string ToString() => $"{FirstName} {LastName}";
    }
}
