using PayrollApi.Application.DTOs.LeaveType;

namespace PayrollApi.Application.Services.Interfaces;

public interface ILeaveTypeService
{
    Task<IEnumerable<LeaveTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<LeaveTypeDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<LeaveTypeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LeaveTypeDto> CreateAsync(CreateLeaveTypeDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<LeaveTypeDto> UpdateAsync(int id, UpdateLeaveTypeDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
