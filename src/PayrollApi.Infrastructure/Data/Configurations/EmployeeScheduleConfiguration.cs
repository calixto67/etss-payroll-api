using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeScheduleConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
    {
        builder.ToTable("EmployeeSchedules");
        builder.HasKey(es => es.Id);

        builder.HasIndex(es => es.EmployeeId).HasDatabaseName("IX_EmployeeSchedules_EmployeeId");

        builder.HasOne(es => es.Employee)
            .WithMany()
            .HasForeignKey(es => es.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(es => es.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(es => es.UpdatedBy).HasMaxLength(128);
        builder.Property(es => es.DeletedBy).HasMaxLength(128);
    }
}
