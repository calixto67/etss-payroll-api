using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class AllowanceTypeConfiguration : IEntityTypeConfiguration<AllowanceType>
{
    public void Configure(EntityTypeBuilder<AllowanceType> builder)
    {
        builder.ToTable("AllowanceTypes");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(a => a.Name).IsUnique().HasDatabaseName("IX_AllowanceTypes_Name");

        builder.Property(a => a.TaxExemptLimit).HasColumnType("decimal(18,2)");

        builder.Property(a => a.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(a => a.UpdatedBy).HasMaxLength(128);
        builder.Property(a => a.DeletedBy).HasMaxLength(128);
    }
}
