using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class Employee
    {
        public int Id { get; private set; }

        // Basic Info
        public FullName FullName { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public Address Address { get; private set; }
        public NationalIdentity NationalId { get;private set; }

        public Gender Gender { get; private set; }

        // Employment Details
        public DateTime DateOfBirth { get; private set; }
        public DateTime HireDate { get; private set; }
        public string JobTitle { get; private set; } = "Unknown";
        public Money Salary { get; private set; }
        public EmploymentStatus Status { get; private set; } = EmploymentStatus.Active;
        public JobLevel JobLevel { get; private set; }
        public ContractDetails ContractDetails { get; private set; } 
        public BankAccount BankAccount { get; private set; }

        // Relations
        public int DepartmentId { get; private set; }
        public Department? Department { get; private set; }

        // Extras
        public string? ProfileImagePath { get; private set; }

        public int Age => CalculateAge(DateOfBirth);
        // Business logic
        public void Activate()
        {
            if (Status == EmploymentStatus.Active)
                throw new InvalidOperationException("Employee is already active.");

            Status = EmploymentStatus.Active;
        }

        public void SetOnLeave()
        {
            if (Status != EmploymentStatus.Active)
                throw new InvalidOperationException("Only active employees can be set on leave.");

            Status = EmploymentStatus.OnLeave;
        }

        public void Terminate()
        {
            if (Status == EmploymentStatus.Terminated)
                throw new InvalidOperationException("Employee is already terminated.");

            if (Status == EmploymentStatus.Resigned)
                throw new InvalidOperationException("Resigned employees cannot be terminated.");

            Status = EmploymentStatus.Terminated;
        }

        public void Resign()
        {
            if (Status == EmploymentStatus.Terminated)
                throw new InvalidOperationException("Terminated employees cannot resign.");

            if (Status == EmploymentStatus.Resigned)
                throw new InvalidOperationException("Employee is already resigned.");

            Status = EmploymentStatus.Resigned;
        }

        public void UpdateFullName(FullName newFullName)
        {
            if (newFullName is null)
                throw new ArgumentNullException(nameof(newFullName), "Full name cannot be null.");
            FullName = newFullName;
        }
        public void ChangeJobTitle(string newTitle)
        {
            if (newTitle.Length < 3)
                throw new ArgumentException("Job title must be at least 3 characters long.", nameof(newTitle));

            JobTitle = newTitle;
        }

        public void AssignToDepartment(int departmentId)
        {
            if (departmentId <= 0)
                throw new ArgumentException("Department ID must be greater than zero.", nameof(departmentId));

            DepartmentId = departmentId;
        }

        public void UnassignFromDepartment()
        {
            if (DepartmentId == 0)
                throw new InvalidOperationException("Employee is not assigned to any department.");
            DepartmentId = 2;
        }


        public void UpdateAddress(Address newAddress)
        {
            if (newAddress is null)
                throw new ArgumentNullException(nameof(newAddress), "Address cannot be null.");

            Address = newAddress;
        }
        public void UpdateNationalId(NationalIdentity newNationalId)
        {
            if (newNationalId is null)
                throw new ArgumentNullException(nameof(newNationalId), "National ID cannot be null.");
            NationalId = newNationalId;
        }

        public void UpdateContactInfo(ContactInfo newContactInfo)
        {
            if (newContactInfo == null)
                throw new ArgumentNullException(nameof(newContactInfo));

            ContactInfo = newContactInfo;
        }
        public void SetSalary(Money newSalary)
        {
            if (newSalary == null)
                throw new ArgumentNullException(nameof(newSalary));
            if (newSalary.Amount <= 0)
                throw new ArgumentException("Salary must be greater than zero.", nameof(newSalary));

            Salary = newSalary;
        }
        public void AdjustSalary(Money amount, bool increase = true)
        {
            if (amount == null)
                throw new ArgumentNullException(nameof(amount));
            if (amount.Amount <= 0)
                throw new ArgumentException("Salary adjustment amount must be greater than zero.", nameof(amount));

            Salary = increase ? Salary.Add(amount) : Salary.Subtract(amount);
        }

        public void UpdateContractDetails(ContractDetails newContract)
        {
            if (newContract == null) throw new ArgumentNullException(nameof(newContract));
            ContractDetails = newContract;
        }

        public void UpdateBankAccount(BankAccount newAccount)
        {
            if (newAccount == null) throw new ArgumentNullException(nameof(newAccount));
            BankAccount = newAccount;
        }
        public void UpdateProfileImagePath(string profileImagePath)
        {
            if (profileImagePath == null) throw new ArgumentNullException(nameof(profileImagePath));
            ProfileImagePath = profileImagePath;
        }
        public void Promote()
        {
            if (JobLevel == JobLevel.Director)
                throw new InvalidOperationException("Employee is already at the highest level.");
            JobLevel++;
        }

        public void Demote()
        {
            if (JobLevel == JobLevel.Intern)
                throw new InvalidOperationException("Employee is already at the lowest level.");
            JobLevel--;
        }

        private static int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            int age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }

        public static Employee CreateNew(
    FullName fullName,
    ContactInfo contactInfo,
    Address address,
    NationalIdentity nationalId,
    Gender gender,
    DateTime dateOfBirth,
    Money startingSalary,
    ContractDetails contractDetails,
    BankAccount bankAccount,
    string jobTitle,
    JobLevel jobLevel)
        {
            if (dateOfBirth > DateTime.Today)
                throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

            if (CalculateAge(dateOfBirth) < 18)
                throw new ArgumentException("Employee must be at least 18 years old.", nameof(dateOfBirth));

            if (startingSalary.Amount <= 0)
                throw new ArgumentException("Salary must be positive.", nameof(startingSalary));

            if (contractDetails is null)
                throw new ArgumentNullException(nameof(contractDetails));

            if (bankAccount is null)
                throw new ArgumentNullException(nameof(bankAccount));

            return new Employee
            {
                FullName = fullName,
                ContactInfo = contactInfo,
                Address = address,
                NationalId = nationalId,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                HireDate = DateTime.UtcNow,
                Salary = startingSalary,
                ContractDetails = contractDetails,
                BankAccount = bankAccount,
                JobTitle = jobTitle,
                Status = EmploymentStatus.Active,
                JobLevel = jobLevel
            };
        }
        private Employee() { }


    }
}
