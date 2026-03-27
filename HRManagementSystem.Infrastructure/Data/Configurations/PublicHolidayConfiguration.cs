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
    public class PublicHolidayConfiguration:IEntityTypeConfiguration<PublicHoliday>
    {
        public void Configure(EntityTypeBuilder<PublicHoliday> builder)
        {
            builder.ToTable("PublicHolidays");

            builder.HasKey(ph => ph.Id);

            builder.Property(ph => ph.Name).IsRequired().HasMaxLength(100);

            builder.OwnsOne(ph => ph.Period, pd =>
            {
               pd.Property(p => p.StartDate).HasColumnName("StartDate").IsRequired();
                pd.Property(p => p.EndDate).HasColumnName("EndDate").IsRequired();

                pd.HasIndex(p => p.StartDate);
                pd.HasIndex(p => p.EndDate);
            });

            builder.Property(ph => ph.Notes).HasMaxLength(300);

            builder.Ignore(ph=>ph.Year);
            builder.Ignore(ph=>ph.TotalDays);

            builder.Property(ph => ph.IsPaid).HasDefaultValue(true);


        }
    }
}
