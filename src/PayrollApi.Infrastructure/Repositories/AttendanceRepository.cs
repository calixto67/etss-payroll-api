using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;

    public AttendanceRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Attendance>> GetByPeriodAsync(int payrollPeriodId, string? search = null, CancellationToken ct = default)
    {
        var query = _context.Attendances
            .Include(a => a.Employee)
            .Include(a => a.Details)
            .Where(a => a.PayrollPeriodId == payrollPeriodId && !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.ToLower();
            query = query.Where(a =>
                a.Employee.LastName.ToLower().Contains(q) ||
                a.Employee.FirstName.ToLower().Contains(q) ||
                a.Employee.EmployeeCode.ToLower().Contains(q));
        }

        return await query
            .OrderBy(a => a.Employee.LastName)
            .ThenBy(a => a.Employee.FirstName)
            .ToListAsync(ct);
    }

    public async Task<Attendance?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Attendances
            .Include(a => a.Employee)
            .Include(a => a.Details.OrderBy(d => d.Date))
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
    }

    public async Task<Attendance?> GetByPeriodAndEmployeeAsync(int payrollPeriodId, int employeeId, CancellationToken ct = default)
    {
        return await _context.Attendances
            .FirstOrDefaultAsync(a =>
                a.PayrollPeriodId == payrollPeriodId &&
                a.EmployeeId == employeeId &&
                !a.IsDeleted, ct);
    }

    public async Task<Attendance> AddAsync(Attendance attendance, CancellationToken ct = default)
    {
        await _context.Attendances.AddAsync(attendance, ct);
        return attendance;
    }

    public async Task AddRangeAsync(IEnumerable<Attendance> records, CancellationToken ct = default)
    {
        await _context.Attendances.AddRangeAsync(records, ct);
    }

    public Task UpdateAsync(Attendance attendance, CancellationToken ct = default)
    {
        _context.Attendances.Update(attendance);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
    {
        var record = await _context.Attendances
            .Include(a => a.Details)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
        if (record != null)
        {
            record.IsDeleted = true;
            record.DeletedAt = DateTime.Now;
            record.DeletedBy = deletedBy;
            // Soft-delete details too
            foreach (var detail in record.Details)
            {
                detail.IsDeleted = true;
                detail.DeletedAt = DateTime.Now;
                detail.DeletedBy = deletedBy;
            }
        }
    }
}
