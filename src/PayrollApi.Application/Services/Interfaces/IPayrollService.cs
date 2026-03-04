using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Payroll;

namespace PayrollApi.Application.Services.Interfaces;

public interface IPayrollService
{
    Task<PagedResult<PayrollRecordDto>> GetPagedAsync(PaginationParams pagination, int? employeeId, int? periodId, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PayrollRecordDto>> RunPayrollAsync(RunPayrollDto dto, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto> ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto> ReleaseAsync(int id, string releasedBy, CancellationToken cancellationToken = default);
}
