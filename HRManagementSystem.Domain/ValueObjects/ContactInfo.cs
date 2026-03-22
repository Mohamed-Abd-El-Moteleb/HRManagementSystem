using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.ValueObjects
{
    public class ContactInfo
    {
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? EmergencyContactName { get; private set; }
        public string? EmergencyContactPhone { get; private set; }

        private ContactInfo() { }

        public ContactInfo(string email, string phoneNumber, string? emergencyContactName = null, string? emergencyContactPhone = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

            try
            {
                email = email?.Trim().ToLowerInvariant();

                var addr = new System.Net.Mail.MailAddress(email);

                if (addr.Address != email)
                    throw new Exception();
            }
            catch
            {
                throw new ArgumentException("Invalid email format.", nameof(email));
            }

            if (phoneNumber.Length < 11 || !phoneNumber.All(char.IsDigit)|| !phoneNumber.StartsWith("01"))
                throw new ArgumentException("Phone number must be at least 11 digits.", nameof(phoneNumber));

            if (!string.IsNullOrWhiteSpace(emergencyContactName) && string.IsNullOrWhiteSpace(emergencyContactPhone))
                throw new ArgumentException("Emergency contact phone is required if name is provided.", nameof(emergencyContactPhone));

            if (string.IsNullOrWhiteSpace(emergencyContactName) && !string.IsNullOrWhiteSpace(emergencyContactPhone))
                throw new ArgumentException("Emergency contact name is required if phone is provided.", nameof(emergencyContactName));

            if (!string.IsNullOrWhiteSpace(emergencyContactPhone))
            {
                if (emergencyContactPhone.Length != 11 || !emergencyContactPhone.All(char.IsDigit))
                {
                    throw new ArgumentException("Emergency contact phone must be exactly 11 digits and contain only numbers.", nameof(emergencyContactPhone));
                }

                if (!emergencyContactPhone.StartsWith("01"))
                {
                    throw new ArgumentException("Emergency contact phone must be a valid Egyptian mobile number starting with 01.", nameof(emergencyContactPhone));
                }
            }
            Email = email;
            PhoneNumber = phoneNumber;
            EmergencyContactName = emergencyContactName;
            EmergencyContactPhone = emergencyContactPhone;
        }
    }
}
