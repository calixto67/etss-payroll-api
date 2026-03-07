using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);

        // Identity
        builder.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(20);
        builder.HasIndex(e => e.EmployeeCode).IsUnique().HasDatabaseName("IX_Employees_EmployeeCode");

        // Personal
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.MiddleName).HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Suffix).HasMaxLength(20);
        builder.Property(e => e.Gender).HasConversion<int>().IsRequired();
        builder.Property(e => e.MaritalStatus).HasConversion<int>().IsRequired();

        // Government IDs
        builder.Property(e => e.TaxIdentificationNumber).IsRequired().HasMaxLength(32);
        builder.Property(e => e.SssNumber).IsRequired().HasMaxLength(32);
        builder.Property(e => e.PhilHealthNumber).IsRequired().HasMaxLength(32);
        builder.Property(e => e.PagIbigNumber).IsRequired().HasMaxLength(32);

        // Contact
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Employees_Email");
        builder.Property(e => e.PersonalEmail).HasMaxLength(256);
        builder.Property(e => e.MobileNumber).IsRequired().HasMaxLength(20);
        builder.Property(e => e.AlternatePhone).HasMaxLength(20);

        // Present Address
        builder.Property(e => e.PresentAddress).IsRequired().HasMaxLength(500);
        builder.Property(e => e.PresentCity).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PresentProvince).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PresentZipCode).IsRequired().HasMaxLength(10);

        // Permanent Address
        builder.Property(e => e.PermanentAddress).HasMaxLength(500);
        builder.Property(e => e.PermanentCity).HasMaxLength(100);
        builder.Property(e => e.PermanentProvince).HasMaxLength(100);
        builder.Property(e => e.PermanentZipCode).HasMaxLength(10);

        // Employment
        builder.Property(e => e.EmploymentType).HasConversion<int>().IsRequired();
        builder.Property(e => e.SalaryFrequency).HasConversion<int>().IsRequired();

        // Compensation
        builder.Property(e => e.BasicSalary).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(e => e.BankAccountNumber).IsRequired().HasMaxLength(64);
        builder.Property(e => e.BankName).IsRequired().HasMaxLength(128);

        // Profile
        builder.Property(e => e.ProfilePhotoPath).HasMaxLength(500);
        builder.Property(e => e.BiometricId).HasMaxLength(50);

        // Status
        builder.Property(e => e.Status).HasConversion<int>().IsRequired();
        builder.Property(e => e.StatusRemarks).HasMaxLength(500);
        builder.Property(e => e.StatusChangedBy).HasMaxLength(128);

        // Audit
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(e => e.UpdatedBy).HasMaxLength(128);
        builder.Property(e => e.DeletedBy).HasMaxLength(128);

        // ── Relationships ────────────────────────────────────────────────────
        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Manager)
            .WithMany()
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(e => e.Branch)
            .WithMany(b => b.Employees)
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(e => e.StatusHistory)
            .WithOne(h => h.Employee)
            .HasForeignKey(h => h.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.EmergencyContacts)
            .WithOne(c => c.Employee)
            .HasForeignKey(c => c.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Documents)
            .WithOne(d => d.Employee)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Performance indexes
        builder.HasIndex(e => e.DepartmentId).HasDatabaseName("IX_Employees_DepartmentId");
        builder.HasIndex(e => e.Status).HasDatabaseName("IX_Employees_Status");
        builder.HasIndex(e => e.ManagerId).HasDatabaseName("IX_Employees_ManagerId");
    }
}
