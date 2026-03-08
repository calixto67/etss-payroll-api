using PayrollApi.Application.DTOs.OvertimeApplication;

namespace PayrollApi.Application.Services.Interfaces;

public interface IOvertimeApplicationService
{
    Task<IEnumerable<OvertimeApplicationDto>> GetApplicationsAsync(
        string? search, string? status, int? payrollPeriodId,
        CancellationToken cancellationToken = default);

    Task<OvertimeApplicationDto> CreateApplicationAsync(
        CreateOvertimeApplicationDto dto, string createdBy,
        CancellationToken cancellationToken = default);

    Task<OvertimeApplicationDto> UpdateApplicationStatusAsync(
        int id, UpdateOvertimeApplicationStatusDto dto, string updatedBy,
        CancellationToken cancellationToken = default);

    Task DeleteApplicationAsync(int id, string deletedBy,
        CancellationToken cancellationToken = default);
}
