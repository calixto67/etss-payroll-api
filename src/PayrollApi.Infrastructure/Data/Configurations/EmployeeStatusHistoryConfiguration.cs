using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeStatusHistoryConfiguration : IEntityTypeConfiguration<EmployeeStatusHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeStatusHistory> builder)
    {
        builder.ToTable("EmployeeStatusHistory");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.PreviousStatus).HasConversion<int>().IsRequired();
        builder.Property(h => h.NewStatus).HasConversion<int>().IsRequired();
        builder.Property(h => h.Remarks).IsRequired().HasMaxLength(500);
        builder.Property(h => h.ChangedBy).IsRequired().HasMaxLength(128);
        builder.Property(h => h.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(h => h.UpdatedBy).HasMaxLength(128);
        builder.Property(h => h.DeletedBy).HasMaxLength(128);

        builder.HasIndex(h => h.EmployeeId).HasDatabaseName("IX_EmployeeStatusHistory_EmployeeId");
    }
}
