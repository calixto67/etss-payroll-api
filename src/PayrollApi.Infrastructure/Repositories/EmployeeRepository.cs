using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context) { }

    public async Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode, cancellationToken);

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbSet.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);

    public async Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, int? departmentId,
        EmploymentStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e =>
                e.FirstName.Contains(search) ||
                e.LastName.Contains(search) ||
                e.EmployeeCode.Contains(search) ||
                e.Email.Contains(search));

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        if (status.HasValue)
            query = query.Where(e => e.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(e =>
            e.Email == email && (!excludeId.HasValue || e.Id != excludeId.Value),
            cancellationToken);

    public async Task<bool> IsEmployeeCodeUniqueAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default) =>
        !await _dbSet.AnyAsync(e =>
            e.EmployeeCode == code && (!excludeId.HasValue || e.Id != excludeId.Value),
            cancellationToken);
}
