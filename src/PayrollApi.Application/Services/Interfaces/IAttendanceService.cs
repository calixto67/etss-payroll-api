using PayrollApi.Application.DTOs.Attendance;

namespace PayrollApi.Application.Services.Interfaces;

public interface IAttendanceService
{
    Task<IEnumerable<AttendanceDto>> GetByPeriodAsync(int periodId, string? search, CancellationToken ct);
    Task<AttendanceDto> GetByIdAsync(int id, CancellationToken ct);
    Task<AttendanceDto> CreateAsync(int periodId, CreateAttendanceDto dto, string createdBy, CancellationToken ct);
    Task<AttendanceDto> UpdateAsync(int id, UpdateAttendanceDto dto, string updatedBy, CancellationToken ct);
    Task<AttendanceDto> ResolveAsync(int id, ResolveAttendanceDto dto, string updatedBy, CancellationToken ct);
    Task DeleteAsync(int id, string deletedBy, CancellationToken ct);
    Task<int> ImportAsync(int periodId, IEnumerable<ImportAttendanceRowDto> rows, string createdBy, CancellationToken ct);
    Task<IEnumerable<AttendanceDetailDto>> GetDetailsAsync(int attendanceId, CancellationToken ct);
    Task<AttendanceDto> UpdateDetailsAsync(int attendanceId, IEnumerable<UpdateAttendanceDetailDto> details, string updatedBy, CancellationToken ct);
    Task<int> ImportRawPunchesAsync(int periodId, IEnumerable<ImportRawPunchDto> punches, string createdBy, CancellationToken ct);
    Task<IEnumerable<ScheduleCheckResultDto>> CheckEmployeeSchedulesAsync(IEnumerable<string> employeeCodes, CancellationToken ct);
}
