using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.OvertimeApplication;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/overtime")]
public class OvertimeApplicationController : BaseController
{
    private readonly IOvertimeApplicationService _service;

    public OvertimeApplicationController(IOvertimeApplicationService service)
    {
        _service = service;
    }

    /// <summary>Get all overtime applications with optional filters.</summary>
    [HttpGet("applications")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OvertimeApplicationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplications(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int? payrollPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetApplicationsAsync(search, status, payrollPeriodId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<OvertimeApplicationDto>>.Ok(result));
    }

    /// <summary>Submit a new overtime application.</summary>
    [HttpPost("applications")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<OvertimeApplicationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateApplication(
        [FromBody] CreateOvertimeApplicationDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateApplicationAsync(dto, CurrentUser, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<OvertimeApplicationDto>.Ok(result, "Overtime application submitted successfully."));
    }

    /// <summary>Approve or reject an overtime application.</summary>
    [HttpPut("applications/{id:int}/status")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<OvertimeApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateApplicationStatus(
        [FromRoute] int id,
        [FromBody] UpdateOvertimeApplicationStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateApplicationStatusAsync(id, dto, CurrentUser, cancellationToken);
        return Ok(ApiResponse<OvertimeApplicationDto>.Ok(result, $"Overtime application {result.RefNo} {result.Status.ToLower()}."));
    }

    /// <summary>Delete an overtime application.</summary>
    [HttpDelete("applications/{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteApplication(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _service.DeleteApplicationAsync(id, CurrentUser, cancellationToken);
        return NoContent();
    }
}
