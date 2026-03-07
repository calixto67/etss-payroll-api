using PayrollApi.Application.DTOs.Leave;

namespace PayrollApi.Application.Services.Interfaces;

public interface ILeaveService
{
    // Applications
    Task<IEnumerable<LeaveApplicationDto>> GetApplicationsAsync(
        string? search, string? status, string? leaveType,
        CancellationToken cancellationToken = default);

    Task<LeaveApplicationDto> CreateApplicationAsync(
        CreateLeaveApplicationDto dto, string createdBy,
        CancellationToken cancellationToken = default);

    Task<LeaveApplicationDto> UpdateApplicationStatusAsync(
        int id, UpdateLeaveApplicationStatusDto dto, string updatedBy,
        CancellationToken cancellationToken = default);

    // Balances
    Task<IEnumerable<LeaveBalanceDto>> GetBalancesAsync(
        string? search, string? leaveType,
        CancellationToken cancellationToken = default);

    // Holidays
    Task<IEnumerable<HolidayDto>> GetHolidaysAsync(
        string? search, string? type,
        CancellationToken cancellationToken = default);

    Task<HolidayDto> CreateHolidayAsync(
        CreateHolidayDto dto, string createdBy,
        CancellationToken cancellationToken = default);

    Task DeleteHolidayAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
