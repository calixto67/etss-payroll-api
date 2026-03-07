using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class LeaveApplicationConfiguration : IEntityTypeConfiguration<LeaveApplication>
{
    public void Configure(EntityTypeBuilder<LeaveApplication> builder)
    {
        builder.ToTable("LeaveApplications");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(32);
        builder.HasIndex(e => e.ReferenceNumber).IsUnique().HasDatabaseName("IX_LeaveApplications_RefNo");

        builder.Property(e => e.EmployeeName).IsRequired().HasMaxLength(128);
        builder.Property(e => e.LeaveType).IsRequired().HasMaxLength(64);
        builder.Property(e => e.Reason).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.ApproverName).IsRequired().HasMaxLength(128);
        builder.Property(e => e.ApproverRemarks).HasMaxLength(1000);
        builder.Property(e => e.Status).HasConversion<int>();

        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(e => e.UpdatedBy).HasMaxLength(128);
        builder.Property(e => e.DeletedBy).HasMaxLength(128);
    }
}
