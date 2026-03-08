using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class DeductionTypeConfiguration : IEntityTypeConfiguration<DeductionType>
{
    public void Configure(EntityTypeBuilder<DeductionType> builder)
    {
        builder.ToTable("DeductionTypes");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(d => d.Name).IsUnique().HasDatabaseName("IX_DeductionTypes_Name");

        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.DefaultAmount).HasColumnType("decimal(18,2)");

        builder.Property(d => d.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(d => d.UpdatedBy).HasMaxLength(128);
        builder.Property(d => d.DeletedBy).HasMaxLength(128);
    }
}
