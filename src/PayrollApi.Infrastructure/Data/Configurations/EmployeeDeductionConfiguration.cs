using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeDeductionConfiguration : IEntityTypeConfiguration<EmployeeDeduction>
{
    public void Configure(EntityTypeBuilder<EmployeeDeduction> builder)
    {
        builder.ToTable("EmployeeDeductions");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Amount).HasColumnType("decimal(18,2)");
        builder.Property(a => a.Remarks).HasMaxLength(500);
        builder.Property(a => a.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(a => a.UpdatedBy).HasMaxLength(128);
        builder.Property(a => a.DeletedBy).HasMaxLength(128);

        builder.HasIndex(a => new { a.EmployeeId, a.DeductionTypeId })
            .IsUnique()
            .HasDatabaseName("UQ_EmployeeDeductions_Employee_Type");
    }
}
