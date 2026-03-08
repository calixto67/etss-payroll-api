using PayrollApi.Domain.Entities;

namespace PayrollApi.Domain.Interfaces.Repositories;

public interface IAttendanceRepository
{
    Task<IEnumerable<Attendance>> GetByPeriodAsync(int payrollPeriodId, string? search = null, CancellationToken ct = default);
    Task<Attendance?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Attendance?> GetByPeriodAndEmployeeAsync(int payrollPeriodId, int employeeId, CancellationToken ct = default);
    Task<Attendance> AddAsync(Attendance attendance, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Attendance> records, CancellationToken ct = default);
    Task UpdateAsync(Attendance attendance, CancellationToken ct = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken ct = default);
}
