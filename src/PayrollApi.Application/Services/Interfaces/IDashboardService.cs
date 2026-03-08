using PayrollApi.Application.DTOs.Dashboard;

namespace PayrollApi.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(int? periodId = null, CancellationToken ct = default);
}
