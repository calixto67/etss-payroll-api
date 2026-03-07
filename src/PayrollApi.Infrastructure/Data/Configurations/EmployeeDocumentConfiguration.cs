using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeDocumentConfiguration : IEntityTypeConfiguration<EmployeeDocument>
{
    public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
    {
        builder.ToTable("EmployeeDocuments");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentType).HasConversion<int>().IsRequired();
        builder.Property(d => d.DocumentName).IsRequired().HasMaxLength(255);
        builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);

        builder.Property(d => d.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(d => d.UpdatedBy).HasMaxLength(128);
        builder.Property(d => d.DeletedBy).HasMaxLength(128);

        builder.HasIndex(d => d.EmployeeId).HasDatabaseName("IX_EmployeeDocuments_EmployeeId");
    }
}
