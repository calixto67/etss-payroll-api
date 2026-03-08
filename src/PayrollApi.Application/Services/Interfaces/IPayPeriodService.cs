using PayrollApi.Application.DTOs.PayPeriod;

namespace PayrollApi.Application.Services.Interfaces;

public interface IPayPeriodService
{
    Task<IEnumerable<PayPeriodDto>> GetAllAsync(int? year, string? status, CancellationToken ct);
    Task<PayPeriodDto>              GetByIdAsync(int id, CancellationToken ct);
    Task<PayPeriodDto>              CreateAsync(CreatePayPeriodDto dto, string createdBy, CancellationToken ct);
    Task<PayPeriodDto>              UpdateStatusAsync(int id, string status, string updatedBy, CancellationToken ct);
    Task                            DeleteAsync(int id, string deletedBy, CancellationToken ct);
}
