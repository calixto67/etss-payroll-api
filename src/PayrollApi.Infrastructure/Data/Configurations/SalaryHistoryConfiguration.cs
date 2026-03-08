using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class SalaryHistoryConfiguration : IEntityTypeConfiguration<SalaryHistory>
{
    public void Configure(EntityTypeBuilder<SalaryHistory> builder)
    {
        builder.ToTable("SalaryHistory");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.PreviousSalary).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.NewSalary).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.ChangedBy).IsRequired().HasMaxLength(128);
        builder.Property(h => h.Remarks).HasMaxLength(500);
        builder.Property(h => h.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(h => h.UpdatedBy).HasMaxLength(128);
        builder.Property(h => h.DeletedBy).HasMaxLength(128);

        builder.HasIndex(h => h.EmployeeId).HasDatabaseName("IX_SalaryHistory_EmployeeId");
    }
}
