using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface ILeaveRepository
{
    // Applications
    Task<IEnumerable<LeaveApplication>> GetApplicationsAsync(
        string? search = null, string? status = null, string? leaveType = null,
        CancellationToken cancellationToken = default);

    Task<LeaveApplication?> GetApplicationByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LeaveApplication> AddApplicationAsync(LeaveApplication application, CancellationToken cancellationToken = default);
    Task UpdateApplicationAsync(LeaveApplication application, CancellationToken cancellationToken = default);
    Task<string> GenerateReferenceNumberAsync(CancellationToken cancellationToken = default);

    // Balances
    Task<IEnumerable<LeaveBalance>> GetBalancesAsync(
        string? search = null, string? leaveType = null,
        CancellationToken cancellationToken = default);

    Task<LeaveBalance?> GetBalanceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddBalanceAsync(LeaveBalance balance, CancellationToken cancellationToken = default);
    Task UpdateBalanceAsync(LeaveBalance balance, CancellationToken cancellationToken = default);

    Task<IEnumerable<LeaveBalance>> GetAllBalancesAsync(CancellationToken cancellationToken = default);
    Task<bool> BalanceExistsAsync(string employeeCode, string leaveType, CancellationToken cancellationToken = default);
    Task DeleteBalanceAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task AddBalancesRangeAsync(IEnumerable<LeaveBalance> balances, CancellationToken cancellationToken = default);
    Task RemoveBalancesRangeAsync(IEnumerable<LeaveBalance> balances, CancellationToken cancellationToken = default);

    // Year-End Batches
    Task<LeaveYearEndBatch?> GetLastCompletedBatchAsync(CancellationToken cancellationToken = default);
    Task AddBatchAsync(LeaveYearEndBatch batch, CancellationToken cancellationToken = default);
    Task UpdateBatchAsync(LeaveYearEndBatch batch, CancellationToken cancellationToken = default);

    // Holidays
    Task<IEnumerable<Holiday>> GetHolidaysAsync(
        string? search = null, string? type = null,
        CancellationToken cancellationToken = default);

    Task<Holiday?> GetHolidayByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Holiday> AddHolidayAsync(Holiday holiday, CancellationToken cancellationToken = default);
    Task DeleteHolidayAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
