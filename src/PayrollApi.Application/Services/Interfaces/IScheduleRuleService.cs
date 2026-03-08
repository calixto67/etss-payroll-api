using PayrollApi.Application.DTOs.WorkSchedule;

namespace PayrollApi.Application.Services.Interfaces;

public interface IScheduleRuleService
{
    Task<ScheduleRuleDto> GetAsync(CancellationToken cancellationToken = default);
    Task<ScheduleRuleDto> UpdateAsync(UpdateScheduleRuleDto dto, string updatedBy, CancellationToken cancellationToken = default);
}
