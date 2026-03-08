using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class GlobalConfigConfiguration : IEntityTypeConfiguration<GlobalConfig>
{
    public void Configure(EntityTypeBuilder<GlobalConfig> builder)
    {
        builder.ToTable("GlobalConfig");
        builder.HasIndex(g => g.ConfigName).IsUnique();
        builder.Property(g => g.ConfigName).HasMaxLength(128);
        builder.Property(g => g.MimeType).HasMaxLength(128);
        builder.Property(g => g.CreatedDate).HasColumnType("datetime2");
        builder.Property(g => g.UpdatedDate).HasColumnType("datetime2");
    }
}
