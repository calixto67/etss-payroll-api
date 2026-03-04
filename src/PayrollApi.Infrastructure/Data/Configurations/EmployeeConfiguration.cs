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

        builder.Property(e => e.EmployeeCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(e => e.EmployeeCode)
            .IsUnique()
            .HasDatabaseName("IX_Employees_EmployeeCode");

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(64);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(64);

        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Employees_Email");

        builder.Property(e => e.PhoneNumber).HasMaxLength(20);

        builder.Property(e => e.BasicSalary)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Sensitive fields - stored encrypted at application level
        builder.Property(e => e.BankAccountNumber).IsRequired().HasMaxLength(64);
        builder.Property(e => e.BankName).IsRequired().HasMaxLength(128);
        builder.Property(e => e.TaxIdentificationNumber).IsRequired().HasMaxLength(32);
        builder.Property(e => e.SssNumber).IsRequired().HasMaxLength(32);
        builder.Property(e => e.PhilHealthNumber).IsRequired().HasMaxLength(32);
        builder.Property(e => e.PagIbigNumber).IsRequired().HasMaxLength(32);

        builder.Property(e => e.Status).HasConversion<int>();
        builder.Property(e => e.EmploymentType).HasConversion<int>();

        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(128);
        builder.Property(e => e.UpdatedBy).HasMaxLength(128);
        builder.Property(e => e.DeletedBy).HasMaxLength(128);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
