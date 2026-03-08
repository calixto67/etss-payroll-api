using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class ScheduleRuleConfiguration : IEntityTypeConfiguration<ScheduleRule>
{
    public void Configure(EntityTypeBuilder<ScheduleRule> builder)
    {
        builder.ToTable("ScheduleRules");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.HalfDayThresholdHours).HasColumnType("decimal(5,2)");
        builder.Property(r => r.NightDiffStartTime).HasColumnType("time");
        builder.Property(r => r.NightDiffEndTime).HasColumnType("time");
        builder.Property(r => r.NightDiffRate).HasColumnType("decimal(5,2)");
        builder.Property(r => r.RegularHoursPerDay).HasColumnType("decimal(5,2)");

        builder.Property(r => r.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(r => r.UpdatedBy).HasMaxLength(128);
        builder.Property(r => r.DeletedBy).HasMaxLength(128);
    }
}
