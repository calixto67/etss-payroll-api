using PayrollApi.Application.DTOs.WorkSchedule;

namespace PayrollApi.Application.Services.Interfaces;

public interface IWorkScheduleService
{
    Task<IEnumerable<WorkScheduleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkScheduleDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<WorkScheduleDto> CreateAsync(CreateWorkScheduleDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<WorkScheduleDto> UpdateAsync(int id, UpdateWorkScheduleDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeScheduleDto>> AssignEmployeesAsync(int scheduleId, AssignEmployeeScheduleDto dto, string assignedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeScheduleDto>> GetEmployeesAsync(int scheduleId, CancellationToken cancellationToken = default);
    Task UnassignEmployeeAsync(int scheduleId, int employeeId, string unassignedBy, CancellationToken cancellationToken = default);
}
