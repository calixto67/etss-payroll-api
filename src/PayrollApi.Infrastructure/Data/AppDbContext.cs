using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration classes in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filter: exclude soft-deleted records from all queries
        modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Position>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PayrollRecord>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PayrollPeriod>().HasQueryFilter(e => !e.IsDeleted);
    }
}
