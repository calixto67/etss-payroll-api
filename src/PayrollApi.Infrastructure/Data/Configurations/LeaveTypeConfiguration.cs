using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("LeaveTypes");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(l => l.Name).IsUnique().HasDatabaseName("IX_LeaveTypes_Name");

        builder.Property(l => l.Code).HasMaxLength(20);
        builder.HasIndex(l => l.Code).IsUnique().HasFilter("[Code] IS NOT NULL").HasDatabaseName("IX_LeaveTypes_Code");

        builder.Property(l => l.Description).HasMaxLength(500);
        builder.Property(l => l.EligibleEmploymentTypes).HasMaxLength(100);

        builder.Property(l => l.DefaultDaysPerYear).HasColumnType("decimal(5,2)");
        builder.Property(l => l.TenureIncrementDays).HasColumnType("decimal(5,2)");
        builder.Property(l => l.TenureMaxDays).HasColumnType("decimal(5,2)");
        builder.Property(l => l.PaidPercentage).HasColumnType("decimal(5,2)");
        builder.Property(l => l.CarryForwardMaxDays).HasColumnType("decimal(5,2)");
        builder.Property(l => l.MaxCashConversionDays).HasColumnType("decimal(5,2)");

        builder.Property(l => l.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(l => l.UpdatedBy).HasMaxLength(128);
        builder.Property(l => l.DeletedBy).HasMaxLength(128);
    }
}
