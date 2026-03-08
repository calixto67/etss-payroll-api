using PayrollApi.Application.DTOs.Payroll;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class BankDisbursementService : IBankDisbursementService
{
    private readonly ISqlExecutor _sql;

    public BankDisbursementService(ISqlExecutor sql) => _sql = sql;

    public async Task<IEnumerable<BankDisbursementRow>> GetByPeriodAsync(int payrollPeriodId, CancellationToken ct = default)
    {
        return await _sql.QueryAsync<BankDisbursementRow>(
            "sp_BankDisbursement",
            new { ActionType = "GET_BY_PERIOD", PayrollPeriodId = payrollPeriodId },
            ct);
    }
}
