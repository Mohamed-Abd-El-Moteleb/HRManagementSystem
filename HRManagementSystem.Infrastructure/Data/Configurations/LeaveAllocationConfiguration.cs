using HRManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Data.Configurations
{
    public class LeaveAllocationConfiguration:IEntityTypeConfiguration<LeaveAllocation>
    {
        public void Configure(EntityTypeBuilder<LeaveAllocation> builder)
        {
            builder.ToTable("LeaveAllocations");

            builder.HasKey(la => la.Id);

            builder.HasIndex(la => new { la.EmployeeId, la.Year, la.LeaveType })
             .IsUnique()
             .HasDatabaseName("IX_Employee_Year_LeaveType_Unique");

            builder.Property(la => la.TotalDays)
            .IsRequired();

            builder.Property(la => la.UsedDays)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(la => la.Employee)
            .WithMany()
            .HasForeignKey(la => la.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
