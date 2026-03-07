using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BranchCode).IsRequired().HasMaxLength(20);
        builder.HasIndex(b => b.BranchCode).IsUnique().HasDatabaseName("IX_Branches_BranchCode");

        builder.Property(b => b.BranchName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Address).HasMaxLength(500);

        builder.Property(b => b.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(b => b.UpdatedBy).HasMaxLength(128);
        builder.Property(b => b.DeletedBy).HasMaxLength(128);
    }
}
