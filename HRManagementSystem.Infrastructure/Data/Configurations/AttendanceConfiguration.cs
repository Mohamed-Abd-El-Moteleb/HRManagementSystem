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
    public class AttendanceConfiguration:IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(a=>a.Id);

            builder.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();

            builder.Property(a => a.Status)
               .HasConversion<int>()
               .IsRequired();

            builder.HasOne(a=>a.Employee).
                WithMany().HasForeignKey(a=>a.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
