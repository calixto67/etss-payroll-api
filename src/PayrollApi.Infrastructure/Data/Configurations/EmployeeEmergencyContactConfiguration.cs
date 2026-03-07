using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeEmergencyContactConfiguration : IEntityTypeConfiguration<EmployeeEmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmployeeEmergencyContact> builder)
    {
        builder.ToTable("EmployeeEmergencyContacts");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContactName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Relationship).IsRequired().HasMaxLength(50);
        builder.Property(c => c.MobileNumber).IsRequired().HasMaxLength(20);
        builder.Property(c => c.AlternatePhone).HasMaxLength(20);

        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(c => c.UpdatedBy).HasMaxLength(128);
        builder.Property(c => c.DeletedBy).HasMaxLength(128);

        builder.HasIndex(c => c.EmployeeId).HasDatabaseName("IX_EmergencyContacts_EmployeeId");
    }
}
