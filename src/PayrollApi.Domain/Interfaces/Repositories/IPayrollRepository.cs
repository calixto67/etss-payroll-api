using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IPayrollRepository : IBaseRepository<PayrollRecord>
{
    Task<IEnumerable<PayrollRecord>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PayrollRecord>> GetByPeriodIdAsync(int periodId, CancellationToken cancellationToken = default);
    Task<PayrollRecord?> GetByEmployeeAndPeriodAsync(int employeeId, int periodId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<PayrollRecord> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, int? employeeId, int? periodId,
        PayrollStatus? status, CancellationToken cancellationToken = default);
}
