using PayrollApi.Application.DTOs.Reports;

namespace PayrollApi.Application.Services.Interfaces;

public interface IPayrollSummaryReportService
{
    Task<IEnumerable<PayrollSummaryRowDto>> GetByPeriodAsync(int payrollPeriodId, int? departmentId, CancellationToken ct);
}
