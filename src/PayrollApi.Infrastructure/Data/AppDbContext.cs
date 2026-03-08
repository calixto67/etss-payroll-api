using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Employees module
    public DbSet<Employee>                 Employees              => Set<Employee>();
    public DbSet<EmployeeStatusHistory>    EmployeeStatusHistory  => Set<EmployeeStatusHistory>();
    public DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts => Set<EmployeeEmergencyContact>();
    public DbSet<EmployeeDocument>         EmployeeDocuments      => Set<EmployeeDocument>();
    public DbSet<Branch>                   Branches               => Set<Branch>();
    public DbSet<Department>               Departments            => Set<Department>();
    public DbSet<Position>                 Positions              => Set<Position>();

    // Other modules
    public DbSet<User>                Users              => Set<User>();
    public DbSet<PayrollRecord>        PayrollRecords     => Set<PayrollRecord>();
    public DbSet<PayrollPeriod>        PayrollPeriods     => Set<PayrollPeriod>();
    public DbSet<LeaveApplication>     LeaveApplications  => Set<LeaveApplication>();
    public DbSet<LeaveBalance>         LeaveBalances      => Set<LeaveBalance>();
    public DbSet<Holiday>              Holidays           => Set<Holiday>();
    public DbSet<LeaveYearEndBatch>    LeaveYearEndBatches => Set<LeaveYearEndBatch>();
    public DbSet<Role>                 Roles              => Set<Role>();
    public DbSet<RolePermission>       RolePermissions    => Set<RolePermission>();
    public DbSet<CompanySettings>      CompanySettings    => Set<CompanySettings>();
    public DbSet<GlobalConfig>        GlobalConfigs      => Set<GlobalConfig>();
    public DbSet<AllowanceType>       AllowanceTypes     => Set<AllowanceType>();
    public DbSet<LeaveType>           LeaveTypes         => Set<LeaveType>();

    // Work Schedule module
    public DbSet<WorkSchedule>        WorkSchedules      => Set<WorkSchedule>();
    public DbSet<WorkScheduleDay>     WorkScheduleDays   => Set<WorkScheduleDay>();
    public DbSet<ScheduleRule>        ScheduleRules      => Set<ScheduleRule>();
    public DbSet<EmployeeSchedule>    EmployeeSchedules  => Set<EmployeeSchedule>();

    // Salary history
    public DbSet<SalaryHistory>       SalaryHistory      => Set<SalaryHistory>();

    // Attendance module
    public DbSet<Attendance>          Attendances        => Set<Attendance>();
    public DbSet<AttendanceDetail>    AttendanceDetails  => Set<AttendanceDetail>();

    // Deduction types
    public DbSet<DeductionType>       DeductionTypes     => Set<DeductionType>();

    // Overtime applications
    public DbSet<OvertimeApplication> OvertimeApplications => Set<OvertimeApplication>();

    // Employee enrollments
    public DbSet<EmployeeAllowance>   EmployeeAllowances => Set<EmployeeAllowance>();
    public DbSet<EmployeeDeduction>   EmployeeDeductions => Set<EmployeeDeduction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── Global soft-delete query filters ────────────────────────────────
        modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmployeeStatusHistory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmployeeEmergencyContact>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmployeeDocument>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Branch>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Position>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PayrollRecord>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PayrollPeriod>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<LeaveApplication>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<LeaveBalance>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Holiday>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<LeaveYearEndBatch>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<RolePermission>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CompanySettings>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AllowanceType>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<LeaveType>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WorkSchedule>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WorkScheduleDay>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ScheduleRule>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmployeeSchedule>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SalaryHistory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Attendance>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AttendanceDetail>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DeductionType>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmployeeAllowance>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<EmployeeDeduction>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<OvertimeApplication>().HasQueryFilter(e => !e.IsDeleted);

        // CompanySettings decimal precision
        modelBuilder.Entity<CompanySettings>(b =>
        {
            b.Property(c => c.DefaultSssContribution).HasColumnType("decimal(18,2)");
            b.Property(c => c.DefaultPhilHealthContribution).HasColumnType("decimal(18,2)");
            b.Property(c => c.DefaultPagIbigContribution).HasColumnType("decimal(18,2)");
        });

        // Role → RolePermissions
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Role → Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.PermissionRole)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name).IsUnique();

        modelBuilder.Entity<RolePermission>()
            .HasIndex(p => new { p.RoleId, p.ModuleKey }).IsUnique();
    }
}
