using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Data.Configurations
{
    public class SalarySlipConfiguration:IEntityTypeConfiguration<SalarySlip>
    {
        public void Configure(EntityTypeBuilder<SalarySlip> builder)
        {
            builder.ToTable("SalarySlips");

            builder.HasKey(s => s.Id);

            builder.OwnsMany(s => s.DetailedAllowances, a =>
            {
                a.ToTable("SalarySlipAllowances"); 
                a.WithOwner().HasForeignKey("SalarySlipId");
                a.Property<int>("Id");
                a.HasKey("Id");

                a.Property(x => x.Name).IsRequired().HasMaxLength(100);

                a.OwnsOne(x => x.Amount, money =>
                {
                    money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
                    money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
                });
            });

            builder.Navigation(s => s.DetailedAllowances)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

            ConfigureMoneyProperty(builder, s => s.BaseSalary, "BaseSalary");
            ConfigureMoneyProperty(builder, s => s.OvertimeAmount, "OvertimeAmount");
            ConfigureMoneyProperty(builder, s => s.HolidayWorkAmount, "HolidayWorkAmount");
            ConfigureMoneyProperty(builder, s => s.Bonuses, "Bonuses");
            ConfigureMoneyProperty(builder, s => s.AbsenceDeduction, "AbsenceDeduction");
            ConfigureMoneyProperty(builder, s => s.LateDeduction, "LateDeduction");
            ConfigureMoneyProperty(builder, s => s.InsuranceDeduction, "InsuranceDeduction");
            ConfigureMoneyProperty(builder, s => s.TaxDeduction, "TaxDeduction");
            ConfigureMoneyProperty(builder, s => s.ManualDeductions, "ManualDeductions");

            builder.HasOne(s => s.Employee)
                       .WithMany()
                       .HasForeignKey(s => s.EmployeeId)
                       .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.EmployeeId, s.Month, s.Year }).IsUnique();


            builder.Property(s => s.Month).IsRequired();
            builder.Property(s => s.Year).IsRequired();
            builder.Property(s => s.EmployeeId).IsRequired();

            builder.Property(s => s.CalculationDate).IsRequired();
            builder.Property(s => s.IsPaid).IsRequired().HasDefaultValue(false);

            builder.Property(s => s.Notes).HasMaxLength(500);

        }

        private void ConfigureMoneyProperty<T>(EntityTypeBuilder<T> builder,
            System.Linq.Expressions.Expression<System.Func<T, Money>> navigationExpression,
            string prefix) where T : class
        {
            builder.OwnsOne(navigationExpression, m =>
            {
                m.Property(x => x.Amount).IsRequired().HasColumnName($"{prefix}_Amount").HasColumnType("decimal(18,2)");
                m.Property(x => x.Currency).IsRequired().HasColumnName($"{prefix}_Currency").HasMaxLength(3);
            });
        }
    }
}
