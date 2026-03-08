using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface ILeaveTypeRepository : IBaseRepository<LeaveType>
{
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeaveType>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
