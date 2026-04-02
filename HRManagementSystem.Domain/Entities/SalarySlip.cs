using HRManagementSystem.Domain.Exceptions;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HRManagementSystem.Domain.Entities
{
    public class SalarySlip
    {
        public int Id { get; private set; }
        public int EmployeeId { get; private set; }
        public Employee Employee { get; private set; }

        public int Month { get; private set; }
        public int Year { get; private set; }

        
        public Money BaseSalary { get; private set; }

        private readonly List<SalaryAllowance> _detailedAllowances = new(); 
        public virtual IReadOnlyCollection<SalaryAllowance> DetailedAllowances => _detailedAllowances.AsReadOnly();
        public Money TotalAllowances
        {
            get
            {
                if (!_detailedAllowances.Any())
                {
                    var currency = BaseSalary?.Currency ?? "EGP";
                    return Money.Zero(currency);
                }
                return _detailedAllowances.Select(a => a.Amount).Aggregate((sum, next) => sum.Add(next));
            }
        }

        // (Variables)
        public double OvertimeHours { get; private set; }
        public Money OvertimeAmount { get; private set; }
        public int HolidayWorkDays { get; private set; } // Public Holidays
        public double HolidayWorkHours { get; private set; }
        public Money HolidayWorkAmount { get; private set; }
        public Money Bonuses { get; private set; }

        // (Deductions)
        public int AbsentDays { get; private set; }
        public Money AbsenceDeduction { get; private set; }
        public Money LateDeduction { get; private set; }
        public Money InsuranceDeduction { get; private set; }
        public Money TaxDeduction { get; private set; }
        public Money ManualDeductions { get; private set; }

        //(Calculated Fields)
        public Money GrossSalary => BaseSalary.Add(TotalAllowances).Add(OvertimeAmount).Add(HolidayWorkAmount).Add(Bonuses);
        public Money TotalDeductions => AbsenceDeduction.Add(LateDeduction).Add(InsuranceDeduction).Add(TaxDeduction).Add(ManualDeductions);
        public Money NetSalary => GrossSalary.Subtract(TotalDeductions);


        public DateTime CalculationDate { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public bool IsFinalized { get; private set; }
        public bool IsPaid { get; private set; }
        public string? Notes { get; private set; }

        private SalarySlip() { }

        public SalarySlip(int employeeId, int month, int year, Money baseSalary)
        {
            if (month < 1 || month > 12) throw new BusinessException("Invalid month.");
            if (year < DateTime.Now.Year-1) throw new BusinessException("Invalid year.");
            EmployeeId = employeeId;
            Month = month;
            Year = year;
            BaseSalary = baseSalary;
            CalculationDate = DateTime.UtcNow;
            IsPaid = false;

            OvertimeAmount = Money.Zero(baseSalary.Currency);
            HolidayWorkAmount = Money.Zero(baseSalary.Currency);
            Bonuses = Money.Zero(baseSalary.Currency);
            AbsenceDeduction = Money.Zero(baseSalary.Currency);
            LateDeduction = Money.Zero(baseSalary.Currency);
            InsuranceDeduction = Money.Zero(baseSalary.Currency);
            TaxDeduction = Money.Zero(baseSalary.Currency);
            ManualDeductions = Money.Zero(baseSalary.Currency);
        }

        public void Recalculate(
            int absentDays,
            Money absenceDeduction,
            double overtimeHours,
            Money overtimeAmount,
            int holidayDays,
            Money holidayAmount)
        {
            if (IsPaid) throw new InvalidOperationException("Cannot update a paid slip.");

            this.AbsentDays = absentDays;
            this.AbsenceDeduction = absenceDeduction;
            this.OvertimeHours = overtimeHours;
            this.OvertimeAmount = overtimeAmount;
            this.HolidayWorkDays = holidayDays;
            this.HolidayWorkAmount = holidayAmount;

            this.CalculationDate = DateTime.UtcNow;
        }

        public void ClearFixedAllowancesOnly()
        {
            if (IsPaid) throw new InvalidOperationException("Cannot clear allowances of a paid slip.");

            _detailedAllowances.RemoveAll(a => !a.IsManual);
        }

        public void ApplyAttendanceMetrics(int absentDays, Money absenceDeduction, double overtimeHours, Money overtimeAmount)
        {
            if (IsPaid) throw new BusinessException("Cannot update a paid salary slip.");

            AbsentDays = absentDays;
            AbsenceDeduction = absenceDeduction;
            OvertimeHours = overtimeHours;
            OvertimeAmount = overtimeAmount;
        }

        public void ApplyDeductions(Money taxes, Money insurance, Money lateDeduction)
        {
            if (IsPaid) throw new BusinessException("Cannot update a paid salary slip.");

            TaxDeduction = taxes;
            InsuranceDeduction = insurance;
            LateDeduction = lateDeduction;
        }

        public void ApplyHolidayWork(int holidayDays, Money holidayAmount)
        {
            if (IsPaid) throw new BusinessException("Cannot update a paid salary slip.");

            HolidayWorkDays = holidayDays;
            HolidayWorkAmount = holidayAmount;
        }

        public void SetHolidayWork(int days, Money amount)
        {
            if (days < 0) throw new ArgumentException("Days cannot be negative");

            this.HolidayWorkAmount = amount;
        }
        public void AddAllowances(string name, Money amount,bool isManual=false)
        {
            if (IsPaid) throw new BusinessException("Cannot update a paid salary slip.");
            if (amount.Currency != BaseSalary.Currency)
                throw new BusinessException($"Allowance currency ({amount.Currency}) must match base salary currency ({BaseSalary.Currency}).");
            _detailedAllowances.Add(new SalaryAllowance(name, amount,isManual));
        }

        public void AddBonus(Money bonusAmount, string reason)
        {
            if (IsPaid || IsFinalized) throw new BusinessException("Cannot modify bonus.");

            Bonuses = Bonuses.Add(bonusAmount);

            var bonusNote = $"[Bonus: {reason} ({bonusAmount.Amount} {bonusAmount.Currency})]";
            Notes = string.IsNullOrEmpty(Notes) ? bonusNote : $"{Notes} | {bonusNote}";
        }

        public void AddManualDeduction(Money amount, string reason)
        {
            if (IsFinalized || IsPaid)
                throw new BusinessException("Cannot add deduction to a finalized or paid slip");

            ManualDeductions = ManualDeductions.Add(amount);

            var deductionNote = $"[Deduction: {reason} (-{amount.Amount} {amount.Currency})]";
            Notes = string.IsNullOrEmpty(Notes) ? deductionNote : $"{Notes} | {deductionNote}";
        }
        public void FinalizeSlip() 
        {
            IsFinalized = true;
        }
        public void MarkAsPaid(DateTime paymentDate)
        {
            if (IsPaid) throw new BusinessException("Salary slip is already paid.");

            IsPaid = true;
            PaymentDate = paymentDate;
        }
    }
}
