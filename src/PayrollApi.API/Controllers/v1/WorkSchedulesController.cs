using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.WorkSchedule;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Authorize(Policy = "PayrollAdmin")]
[Route("api/v{version:apiVersion}/work-schedules")]
public class WorkSchedulesController : BaseController
{
    private readonly IWorkScheduleService _service;

    public WorkSchedulesController(IWorkScheduleService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<WorkScheduleDto>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<WorkScheduleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<WorkScheduleDto>.Ok(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorkScheduleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateWorkScheduleDto dto, CancellationToken ct)
    {
        var item = await _service.CreateAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponse<WorkScheduleDto>.Ok(item, "Work schedule created."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<WorkScheduleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkScheduleDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<WorkScheduleDto>.Ok(item, "Work schedule updated."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/assign")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignEmployees(int id, [FromBody] AssignEmployeeScheduleDto dto, CancellationToken ct)
    {
        var items = await _service.AssignEmployeesAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<IEnumerable<EmployeeScheduleDto>>.Ok(items, "Employees assigned."));
    }

    [HttpGet("{id:int}/employees")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployees(int id, CancellationToken ct)
    {
        var items = await _service.GetEmployeesAsync(id, ct);
        return Ok(ApiResponse<IEnumerable<EmployeeScheduleDto>>.Ok(items));
    }

    [HttpDelete("{id:int}/employees/{employeeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnassignEmployee(int id, int employeeId, CancellationToken ct)
    {
        await _service.UnassignEmployeeAsync(id, employeeId, CurrentUser, ct);
        return NoContent();
    }
}
