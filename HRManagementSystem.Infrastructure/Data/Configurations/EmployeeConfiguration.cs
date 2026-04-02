using HRManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagementSystem.Infrastructure.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            // FullName (Value Object)
            builder.OwnsOne(e => e.FullName, fn =>
            {
                fn.Property(f => f.FirstName)
                    .HasColumnName("FirstName")
                    .IsRequired()
                    .HasMaxLength(50);

                fn.Property(f => f.LastName)
                    .HasColumnName("LastName")
                    .IsRequired()
                    .HasMaxLength(50);
            }).Navigation(e => e.FullName).IsRequired();

            // ContactInfo (Value Object)
            builder.OwnsOne(e => e.ContactInfo, ci =>
            {
                ci.Property(c => c.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(100)
                    .IsRequired();
                ci.HasIndex(c => c.Email).IsUnique();

                ci.Property(c => c.PhoneNumber).IsRequired()
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            }).Navigation(e=>e.ContactInfo).IsRequired();

            // Address (Value Object)
            builder.OwnsOne(e => e.Address, a =>
            {
                a.Property(ad => ad.City)
                    .HasColumnName("City")
                    .HasMaxLength(50);

                a.Property(ad => ad.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(100);

                a.Property(ad => ad.BuildingNumber)
                    .HasColumnName("BuildingNumber")
                    .HasMaxLength(10);
            }).Navigation(e => e.Address).IsRequired();

            // National Identity (Value Object)
            builder.OwnsOne(e => e.NationalId, ni =>
            {
                ni.Property(n => n.NationalId)
                    .HasColumnName("NationalIdNumber")
                    .HasMaxLength(14)
                    .IsRequired();
                ni.HasIndex(n=>n.NationalId).IsUnique();

            }).Navigation(e => e.NationalId).IsRequired();

            // Money (Value Object)
            builder.OwnsOne(e => e.Salary, s =>
            {
                s.Property(m => m.Amount)
                    .HasColumnName("SalaryAmount")
                    .HasPrecision(18, 2).IsRequired();
                ;

                s.Property(m => m.Currency)
                    .HasColumnName("SalaryCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            }).Navigation(e=>e.Salary).IsRequired();

            // BankAccount (Value Object)
            builder.OwnsOne(e => e.BankAccount, b =>
            {
                b.Property(x => x.AccountNumber)
                    .HasColumnName("BankAccountNumber")
                    .HasMaxLength(30)
                    .IsRequired();

                b.Property(x => x.BankName)
                    .HasColumnName("BankName")
                    .HasMaxLength(100);
            });

            // ContractDetails (Value Object)
            builder.OwnsOne(e => e.ContractDetails, c =>
            {
                c.Property(cd => cd.StartDate)
                    .HasColumnName("ContractStartDate").IsRequired();

                c.Property(cd => cd.EndDate)
                    .HasColumnName("ContractEndDate");

                c.Property(cd => cd.ContractType).IsRequired()
                    .HasColumnName("ContractType")
                    .HasConversion<string>();
            }).Navigation(e => e.ContractDetails).IsRequired();

            builder.Property(e => e.DepartmentId)
            .IsRequired();
            builder.HasIndex(e => e.DepartmentId);
            // Relations
            builder.HasOne(e => e.Department)
                   .WithMany(d => d.Employees)
                   .HasForeignKey(e => e.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Enum conversions
            builder.Property(e => e.Gender).IsRequired()
                   .HasConversion<string>();

            builder.Property(e => e.Status).IsRequired()
                   .HasConversion<string>();

            builder.Property(e => e.JobLevel).IsRequired()
                   .HasConversion<string>();

            builder.Property(e => e.JobTitle)
           .IsRequired()
           .HasMaxLength(100);

            builder.Property(e => e.ProfileImagePath)
                   .HasMaxLength(255);


            builder.OwnsMany(e => e.FixedAllowances, a =>
            {
                a.ToTable("EmployeeFixedAllowances"); 
                a.WithOwner().HasForeignKey("EmployeeId");
                a.Property<int>("Id");
                a.HasKey("Id");

                a.Property(x => x.Name).IsRequired().HasMaxLength(100);

                a.OwnsOne(x => x.Amount, money =>
                {
                    money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
                    money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
                });
            });

            builder.Navigation(e => e.FixedAllowances).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
