using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.ToTable("LeaveBalances");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(20);
        builder.Property(e => e.EmployeeName).IsRequired().HasMaxLength(128);
        builder.Property(e => e.LeaveType).IsRequired().HasMaxLength(64);

        builder.Ignore(e => e.Remaining);

        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(e => e.UpdatedBy).HasMaxLength(128);
        builder.Property(e => e.DeletedBy).HasMaxLength(128);
    }
}
