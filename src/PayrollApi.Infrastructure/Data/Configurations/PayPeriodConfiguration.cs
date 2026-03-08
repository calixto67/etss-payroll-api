using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class PayPeriodConfiguration : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.ToTable("PayrollPeriods");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(30).IsRequired();
        builder.Property(p => p.PeriodCode).HasMaxLength(20).IsRequired();
        builder.Property(p => p.PeriodType).HasConversion<int>();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(p => p.Name).IsUnique().HasDatabaseName("IX_PayrollPeriods_Name");
        builder.HasIndex(p => p.PeriodCode).IsUnique().HasDatabaseName("IX_PayrollPeriods_PeriodCode");

        builder.HasMany(p => p.PayrollRecords)
            .WithOne(r => r.PayrollPeriod)
            .HasForeignKey(r => r.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
