using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class WorkScheduleConfiguration : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("WorkSchedules");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(w => w.Name).IsUnique().HasDatabaseName("IX_WorkSchedules_Name");

        builder.Property(w => w.Description).HasMaxLength(500);

        builder.Property(w => w.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(w => w.UpdatedBy).HasMaxLength(128);
        builder.Property(w => w.DeletedBy).HasMaxLength(128);

        builder.HasMany(w => w.Days)
            .WithOne(d => d.WorkSchedule)
            .HasForeignKey(d => d.WorkScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.EmployeeSchedules)
            .WithOne(es => es.WorkSchedule)
            .HasForeignKey(es => es.WorkScheduleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
