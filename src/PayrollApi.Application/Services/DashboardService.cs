using Microsoft.Data.SqlClient;
using PayrollApi.Application.DTOs.Dashboard;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class DashboardService : IDashboardService
{
    private const string SP = "sp_Dashboard";
    private readonly ISqlExecutor _sql;

    public DashboardService(ISqlExecutor sql) => _sql = sql;

    public async Task<DashboardSummaryDto> GetSummaryAsync(int? periodId = null, CancellationToken ct = default)
    {
        try
        {
            var employeeCounts = await _sql.QueryFirstOrDefaultAsync<EmployeeCountsDto>(
                SP, new { ActionType = "GET_EMPLOYEE_COUNTS" }, ct);

            var activePeriod = await _sql.QueryFirstOrDefaultAsync<ActivePeriodDto>(
                SP, new { ActionType = "GET_ACTIVE_PERIOD", PeriodId = periodId }, ct);

            var recentPeriods = await _sql.QueryAsync<RecentPeriodDto>(
                SP, new { ActionType = "GET_RECENT_PERIODS" }, ct);

            var pendingTasks = await _sql.QueryFirstOrDefaultAsync<PendingTasksDto>(
                SP, new { ActionType = "GET_PENDING_TASKS", PeriodId = periodId }, ct);

            var deptBreakdown = await _sql.QueryAsync<DepartmentBreakdownDto>(
                SP, new { ActionType = "GET_DEPARTMENT_BREAKDOWN" }, ct);

            return new DashboardSummaryDto
            {
                EmployeeCounts = employeeCounts ?? new EmployeeCountsDto(),
                ActivePeriod = activePeriod,
                RecentPeriods = recentPeriods.ToList(),
                PendingTasks = pendingTasks ?? new PendingTasksDto(),
                DepartmentBreakdown = deptBreakdown.ToList()
            };
        }
        catch (SqlException ex)
        {
            throw new ApplicationException($"Failed to load dashboard summary: {ex.Message}");
        }
    }
}
