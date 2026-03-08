using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.EmployeeEnrollment;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employee-enrollments")]
[Authorize(Policy = "PayrollAdmin")]
public class EmployeeEnrollmentsController : BaseController
{
    private readonly IEmployeeEnrollmentService _service;

    public EmployeeEnrollmentsController(IEmployeeEnrollmentService service) => _service = service;

    // ── Allowances ─────────────────────────────────────────────────────

    [HttpGet("allowances")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeAllowanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowances([FromQuery] int? employeeId, [FromQuery] string? search, [FromQuery] int? departmentId, [FromQuery] int? branchId, CancellationToken ct)
    {
        var items = await _service.GetAllowancesAsync(employeeId, search, departmentId, branchId, ct);
        return Ok(ApiResponse<IEnumerable<EmployeeAllowanceDto>>.Ok(items));
    }

    [HttpGet("allowances/{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeAllowanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllowanceById(int id, CancellationToken ct)
    {
        var item = await _service.GetAllowanceByIdAsync(id, ct);
        return Ok(ApiResponse<EmployeeAllowanceDto>.Ok(item));
    }

    [HttpPost("allowances")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeAllowanceDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAllowance([FromBody] CreateEmployeeAllowanceDto dto, CancellationToken ct)
    {
        var item = await _service.CreateAllowanceAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetAllowanceById), new { id = item.Id }, ApiResponse<EmployeeAllowanceDto>.Ok(item, "Employee allowance enrolled."));
    }

    [HttpPut("allowances/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeAllowanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAllowance(int id, [FromBody] UpdateEmployeeAllowanceDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateAllowanceAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<EmployeeAllowanceDto>.Ok(item, "Employee allowance updated."));
    }

    [HttpDelete("allowances/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAllowance(int id, CancellationToken ct)
    {
        await _service.DeleteAllowanceAsync(id, CurrentUser, ct);
        return NoContent();
    }

    // ── Deductions ─────────────────────────────────────────────────────

    [HttpGet("deductions")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDeductionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeductions([FromQuery] int? employeeId, [FromQuery] string? search, [FromQuery] int? departmentId, [FromQuery] int? branchId, CancellationToken ct)
    {
        var items = await _service.GetDeductionsAsync(employeeId, search, departmentId, branchId, ct);
        return Ok(ApiResponse<IEnumerable<EmployeeDeductionDto>>.Ok(items));
    }

    [HttpGet("deductions/{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDeductionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeductionById(int id, CancellationToken ct)
    {
        var item = await _service.GetDeductionByIdAsync(id, ct);
        return Ok(ApiResponse<EmployeeDeductionDto>.Ok(item));
    }

    [HttpPost("deductions")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDeductionDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDeduction([FromBody] CreateEmployeeDeductionDto dto, CancellationToken ct)
    {
        var item = await _service.CreateDeductionAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetDeductionById), new { id = item.Id }, ApiResponse<EmployeeDeductionDto>.Ok(item, "Employee deduction enrolled."));
    }

    [HttpPut("deductions/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDeductionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateDeduction(int id, [FromBody] UpdateEmployeeDeductionDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateDeductionAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<EmployeeDeductionDto>.Ok(item, "Employee deduction updated."));
    }

    [HttpDelete("deductions/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteDeduction(int id, CancellationToken ct)
    {
        await _service.DeleteDeductionAsync(id, CurrentUser, ct);
        return NoContent();
    }
}
