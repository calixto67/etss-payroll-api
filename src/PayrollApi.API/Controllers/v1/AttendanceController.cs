using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Attendance;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pay-periods/{periodId:int}/attendance")]
public class AttendanceController : BaseController
{
    private readonly IAttendanceService _service;

    public AttendanceController(IAttendanceService service) => _service = service;

    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    public async Task<IActionResult> GetByPeriod(
        [FromRoute] int periodId,
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var result = await _service.GetByPeriodAsync(periodId, search, ct);
        return Ok(ApiResponse<IEnumerable<AttendanceDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<AttendanceDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> Create(
        [FromRoute] int periodId,
        [FromBody] CreateAttendanceDto dto,
        CancellationToken ct)
    {
        var result = await _service.CreateAsync(periodId, dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetById), new { periodId, id = result.Id },
            ApiResponse<AttendanceDto>.Ok(result, "Attendance record created."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateAttendanceDto dto,
        CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<AttendanceDto>.Ok(result, "Attendance record updated."));
    }

    [HttpPatch("{id:int}/resolve")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> Resolve(
        [FromRoute] int id,
        [FromBody] ResolveAttendanceDto dto,
        CancellationToken ct)
    {
        var result = await _service.ResolveAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<AttendanceDto>.Ok(result, "Issue resolved."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Attendance record deleted."));
    }

    [HttpPost("import")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> Import(
        [FromRoute] int periodId,
        [FromBody] IEnumerable<ImportAttendanceRowDto> rows,
        CancellationToken ct)
    {
        var count = await _service.ImportAsync(periodId, rows, CurrentUser, ct);
        return Ok(ApiResponse<object>.Ok(new { imported = count }, $"{count} attendance records imported."));
    }

    [HttpPost("check-schedules")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> CheckSchedules(
        [FromBody] IEnumerable<string> employeeCodes,
        CancellationToken ct)
    {
        var result = await _service.CheckEmployeeSchedulesAsync(employeeCodes, ct);
        return Ok(ApiResponse<IEnumerable<ScheduleCheckResultDto>>.Ok(result));
    }

    [HttpPost("import-raw")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> ImportRawPunches(
        [FromRoute] int periodId,
        [FromBody] IEnumerable<ImportRawPunchDto> punches,
        CancellationToken ct)
    {
        var count = await _service.ImportRawPunchesAsync(periodId, punches, CurrentUser, ct);
        return Ok(ApiResponse<object>.Ok(new { imported = count }, $"{count} attendance records imported."));
    }

    [HttpGet("{id:int}/details")]
    [Authorize(Policy = "PayrollViewer")]
    public async Task<IActionResult> GetDetails([FromRoute] int id, CancellationToken ct)
    {
        var result = await _service.GetDetailsAsync(id, ct);
        return Ok(ApiResponse<IEnumerable<AttendanceDetailDto>>.Ok(result));
    }

    [HttpPut("{id:int}/details")]
    [Authorize(Policy = "PayrollAdmin")]
    public async Task<IActionResult> UpdateDetails(
        [FromRoute] int id,
        [FromBody] IEnumerable<UpdateAttendanceDetailDto> details,
        CancellationToken ct)
    {
        var result = await _service.UpdateDetailsAsync(id, details, CurrentUser, ct);
        return Ok(ApiResponse<AttendanceDto>.Ok(result, "Attendance details updated."));
    }
}
