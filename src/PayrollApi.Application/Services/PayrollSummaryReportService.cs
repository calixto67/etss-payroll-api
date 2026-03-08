using PayrollApi.Application.DTOs.Reports;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class PayrollSummaryReportService : IPayrollSummaryReportService
{
    private readonly ISqlExecutor _sql;

    public PayrollSummaryReportService(ISqlExecutor sql) => _sql = sql;

    public async Task<IEnumerable<PayrollSummaryRowDto>> GetByPeriodAsync(int payrollPeriodId, int? departmentId, CancellationToken ct)
    {
        return await _sql.QueryAsync<PayrollSummaryRowDto>(
            "sp_PayrollSummary_GetByPeriod",
            new { PayrollPeriodId = payrollPeriodId, DepartmentId = departmentId },
            ct);
    }
}
