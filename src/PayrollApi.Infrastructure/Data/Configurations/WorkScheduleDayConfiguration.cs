using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class WorkScheduleDayConfiguration : IEntityTypeConfiguration<WorkScheduleDay>
{
    public void Configure(EntityTypeBuilder<WorkScheduleDay> builder)
    {
        builder.ToTable("WorkScheduleDays");
        builder.HasKey(d => d.Id);

        builder.HasIndex(d => new { d.WorkScheduleId, d.DayOfWeek })
            .IsUnique()
            .HasDatabaseName("IX_WorkScheduleDays_ScheduleDay");

        builder.Property(d => d.ShiftStart).HasColumnType("time");
        builder.Property(d => d.ShiftEnd).HasColumnType("time");
        builder.Property(d => d.BreakStart).HasColumnType("time");
        builder.Property(d => d.BreakEnd).HasColumnType("time");

        builder.Property(d => d.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(d => d.UpdatedBy).HasMaxLength(128);
        builder.Property(d => d.DeletedBy).HasMaxLength(128);
    }
}
