using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.WorkSchedule;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Authorize(Policy = "PayrollAdmin")]
[Route("api/v{version:apiVersion}/schedule-rules")]
public class ScheduleRulesController : BaseController
{
    private readonly IScheduleRuleService _service;

    public ScheduleRulesController(IScheduleRuleService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ScheduleRuleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var rule = await _service.GetAsync(ct);
        return Ok(ApiResponse<ScheduleRuleDto>.Ok(rule));
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<ScheduleRuleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateScheduleRuleDto dto, CancellationToken ct)
    {
        var rule = await _service.UpdateAsync(dto, CurrentUser, ct);
        return Ok(ApiResponse<ScheduleRuleDto>.Ok(rule, "Schedule rules updated."));
    }
}
