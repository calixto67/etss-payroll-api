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

    // Holidays
    Task<IEnumerable<Holiday>> GetHolidaysAsync(
        string? search = null, string? type = null,
        CancellationToken cancellationToken = default);

    Task<Holiday?> GetHolidayByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Holiday> AddHolidayAsync(Holiday holiday, CancellationToken cancellationToken = default);
    Task DeleteHolidayAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
