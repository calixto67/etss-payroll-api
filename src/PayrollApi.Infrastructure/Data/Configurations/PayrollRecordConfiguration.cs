using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.ToTable("PayrollRecords");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.BasicPay).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.OvertimePay).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.HolidayPay).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Allowances).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.GrossPay).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.SssDeduction).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.PhilHealthDeduction).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.PagIbigDeduction).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.TaxWithheld).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.OtherDeductions).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.TotalDeductions).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.NetPay).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.ProcessedBy).HasMaxLength(128);
        builder.Property(p => p.Remarks).HasMaxLength(512);

        builder.HasIndex(p => new { p.EmployeeId, p.PayrollPeriodId })
            .IsUnique()
            .HasDatabaseName("IX_PayrollRecords_EmployeeId_PeriodId");

        builder.HasOne(p => p.Employee)
            .WithMany(e => e.PayrollRecords)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
