using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _context;

    public LeaveRepository(AppDbContext context) => _context = context;

    // ── Applications ───────────────────────────────────────────────
    public async Task<IEnumerable<LeaveApplication>> GetApplicationsAsync(
        string? search = null, string? status = null, string? leaveType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LeaveApplications.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a =>
                a.EmployeeName.Contains(search) ||
                a.ReferenceNumber.Contains(search));

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<LeaveApplicationStatus>(status, out var parsedStatus))
            query = query.Where(a => a.Status == parsedStatus);

        if (!string.IsNullOrWhiteSpace(leaveType))
            query = query.Where(a => a.LeaveType == leaveType);

        return await query.OrderByDescending(a => a.SubmittedOn).ToListAsync(cancellationToken);
    }

    public async Task<LeaveApplication?> GetApplicationByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.LeaveApplications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<LeaveApplication> AddApplicationAsync(LeaveApplication application, CancellationToken cancellationToken = default)
    {
        await _context.LeaveApplications.AddAsync(application, cancellationToken);
        return application;
    }

    public Task UpdateApplicationAsync(LeaveApplication application, CancellationToken cancellationToken = default)
    {
        _context.LeaveApplications.Update(application);
        return Task.CompletedTask;
    }

    public async Task<string> GenerateReferenceNumberAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.Now.Year;
        var count = await _context.LeaveApplications
            .CountAsync(a => a.SubmittedOn.Year == year, cancellationToken);
        return $"LVA-{year}-{(count + 1):D5}";
    }

    // ── Balances ───────────────────────────────────────────────────
    public async Task<IEnumerable<LeaveBalance>> GetBalancesAsync(
        string? search = null, string? leaveType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LeaveBalances.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.EmployeeName.Contains(search) || b.EmployeeCode.Contains(search));

        if (!string.IsNullOrWhiteSpace(leaveType))
            query = query.Where(b => b.LeaveType == leaveType);

        return await query.OrderBy(b => b.EmployeeCode).ThenBy(b => b.LeaveType).ToListAsync(cancellationToken);
    }

    public async Task<LeaveBalance?> GetBalanceByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.LeaveBalances.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task AddBalanceAsync(LeaveBalance balance, CancellationToken cancellationToken = default) =>
        await _context.LeaveBalances.AddAsync(balance, cancellationToken);

    public Task UpdateBalanceAsync(LeaveBalance balance, CancellationToken cancellationToken = default)
    {
        _context.LeaveBalances.Update(balance);
        return Task.CompletedTask;
    }

    public async Task<bool> BalanceExistsAsync(string employeeCode, string leaveType, CancellationToken cancellationToken = default) =>
        await _context.LeaveBalances.AnyAsync(b => b.EmployeeCode == employeeCode && b.LeaveType == leaveType, cancellationToken);

    public async Task DeleteBalanceAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var balance = await _context.LeaveBalances.FindAsync(new object[] { id }, cancellationToken);
        if (balance is null) return;
        balance.IsDeleted = true;
        balance.DeletedAt = DateTime.Now;
        balance.DeletedBy = deletedBy;
        _context.LeaveBalances.Update(balance);
    }

    public async Task<IEnumerable<LeaveBalance>> GetAllBalancesAsync(CancellationToken cancellationToken = default) =>
        await _context.LeaveBalances.ToListAsync(cancellationToken);

    public async Task AddBalancesRangeAsync(IEnumerable<LeaveBalance> balances, CancellationToken cancellationToken = default) =>
        await _context.LeaveBalances.AddRangeAsync(balances, cancellationToken);

    public Task RemoveBalancesRangeAsync(IEnumerable<LeaveBalance> balances, CancellationToken cancellationToken = default)
    {
        _context.LeaveBalances.RemoveRange(balances);
        return Task.CompletedTask;
    }

    // ── Year-End Batches ─────────────────────────────────────────
    public async Task<LeaveYearEndBatch?> GetLastCompletedBatchAsync(CancellationToken cancellationToken = default) =>
        await _context.LeaveYearEndBatches
            .Where(b => b.Status == LeaveYearEndStatus.Completed)
            .OrderByDescending(b => b.ProcessedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddBatchAsync(LeaveYearEndBatch batch, CancellationToken cancellationToken = default) =>
        await _context.LeaveYearEndBatches.AddAsync(batch, cancellationToken);

    public Task UpdateBatchAsync(LeaveYearEndBatch batch, CancellationToken cancellationToken = default)
    {
        _context.LeaveYearEndBatches.Update(batch);
        return Task.CompletedTask;
    }

    // ── Holidays ───────────────────────────────────────────────────
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(
        string? search = null, string? type = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Holidays.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(h => h.Name.Contains(search));

        if (!string.IsNullOrWhiteSpace(type) &&
            Enum.TryParse<HolidayType>(type, out var parsedType))
            query = query.Where(h => h.Type == parsedType);

        return await query.OrderBy(h => h.Date).ToListAsync(cancellationToken);
    }

    public async Task<Holiday?> GetHolidayByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _context.Holidays.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public async Task<Holiday> AddHolidayAsync(Holiday holiday, CancellationToken cancellationToken = default)
    {
        await _context.Holidays.AddAsync(holiday, cancellationToken);
        return holiday;
    }

    public async Task DeleteHolidayAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var holiday = await _context.Holidays.FindAsync(new object[] { id }, cancellationToken);
        if (holiday is null) return;
        holiday.IsDeleted  = true;
        holiday.DeletedAt  = DateTime.Now;
        holiday.DeletedBy  = deletedBy;
        _context.Holidays.Update(holiday);
    }
}
