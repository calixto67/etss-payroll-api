using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeAllowanceConfiguration : IEntityTypeConfiguration<EmployeeAllowance>
{
    public void Configure(EntityTypeBuilder<EmployeeAllowance> builder)
    {
        builder.ToTable("EmployeeAllowances");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Amount).HasColumnType("decimal(18,2)");
        builder.Property(a => a.Remarks).HasMaxLength(500);
        builder.Property(a => a.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(a => a.UpdatedBy).HasMaxLength(128);
        builder.Property(a => a.DeletedBy).HasMaxLength(128);

        builder.HasIndex(a => new { a.EmployeeId, a.AllowanceTypeId })
            .IsUnique()
            .HasDatabaseName("UQ_EmployeeAllowances_Employee_Type");
    }
}
