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
    public DbSet<Role>                 Roles              => Set<Role>();
    public DbSet<RolePermission>       RolePermissions    => Set<RolePermission>();
    public DbSet<CompanySettings>      CompanySettings    => Set<CompanySettings>();

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
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<RolePermission>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CompanySettings>().HasQueryFilter(e => !e.IsDeleted);

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
