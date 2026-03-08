using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.DaysWorked).HasColumnType("decimal(5,2)");
        builder.Property(a => a.TotalDays).HasColumnType("decimal(5,2)");
        builder.Property(a => a.LateHours).HasColumnType("decimal(5,2)");
        builder.Property(a => a.UndertimeHours).HasColumnType("decimal(5,2)");
        builder.Property(a => a.OtHours).HasColumnType("decimal(5,2)");
        builder.Property(a => a.NightDiffHours).HasColumnType("decimal(5,2)");
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Issue).HasMaxLength(500);
        builder.Property(a => a.ResolutionNotes).HasMaxLength(1000);

        builder.HasIndex(a => new { a.PayrollPeriodId, a.EmployeeId })
            .IsUnique()
            .HasDatabaseName("IX_Attendances_Period_Employee");

        builder.HasOne(a => a.PayrollPeriod)
            .WithMany()
            .HasForeignKey(a => a.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Details)
            .WithOne(d => d.Attendance)
            .HasForeignKey(d => d.AttendanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AttendanceDetailConfiguration : IEntityTypeConfiguration<AttendanceDetail>
{
    public void Configure(EntityTypeBuilder<AttendanceDetail> builder)
    {
        builder.ToTable("AttendanceDetails");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.LateHours).HasColumnType("decimal(5,2)");
        builder.Property(d => d.UndertimeHours).HasColumnType("decimal(5,2)");
        builder.Property(d => d.OtHours).HasColumnType("decimal(5,2)");
        builder.Property(d => d.NightDiffHours).HasColumnType("decimal(5,2)");
        builder.Property(d => d.Status).HasMaxLength(20);
        builder.Property(d => d.Remarks).HasMaxLength(500);

        builder.HasIndex(d => new { d.AttendanceId, d.Date })
            .IsUnique()
            .HasDatabaseName("IX_AttendanceDetails_Attendance_Date");
    }
}
