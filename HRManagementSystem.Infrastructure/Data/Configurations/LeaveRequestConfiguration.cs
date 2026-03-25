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
    public class LeaveRequestConfiguration: IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.HasKey(lr => lr.Id);

            builder.Property(lr => lr.Reason)
            .HasMaxLength(500);

            builder.Property(lr => lr.ManagerComment)
                .HasMaxLength(500);

            builder.Property(lr => lr.Type)
            .IsRequired();

            builder.Property(lr => lr.Status)
                .IsRequired();

            builder.HasOne(lr => lr.Employee).WithMany().HasForeignKey(lr=>lr.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
