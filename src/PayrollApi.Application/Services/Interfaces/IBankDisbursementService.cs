using PayrollApi.Application.DTOs.Payroll;

namespace PayrollApi.Application.Services.Interfaces;

public interface IBankDisbursementService
{
    Task<IEnumerable<BankDisbursementRow>> GetByPeriodAsync(int payrollPeriodId, CancellationToken ct = default);
}
