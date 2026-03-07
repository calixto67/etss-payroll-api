using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Leave;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
public class LeaveController : BaseController
{
    private readonly ILeaveService _leaveService;
    private readonly ILogger<LeaveController> _logger;

    public LeaveController(ILeaveService leaveService, ILogger<LeaveController> logger)
    {
        _leaveService = leaveService;
        _logger       = logger;
    }

    // ── Applications ───────────────────────────────────────────────

    /// <summary>Get all leave applications with optional filters.</summary>
    [HttpGet("applications")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaveApplicationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplications(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? leaveType,
        CancellationToken cancellationToken)
    {
        var result = await _leaveService.GetApplicationsAsync(search, status, leaveType, cancellationToken);
        return Ok(ApiResponse<IEnumerable<LeaveApplicationDto>>.Ok(result));
    }

    /// <summary>Submit a new leave application.</summary>
    [HttpPost("applications")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<LeaveApplicationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateApplication(
        [FromBody] CreateLeaveApplicationDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _leaveService.CreateApplicationAsync(dto, CurrentUser, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<LeaveApplicationDto>.Ok(result, "Leave application submitted successfully."));
    }

    /// <summary>Approve or reject a leave application.</summary>
    [HttpPut("applications/{id:int}/status")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<LeaveApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateApplicationStatus(
        [FromRoute] int id,
        [FromBody] UpdateLeaveApplicationStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _leaveService.UpdateApplicationStatusAsync(id, dto, CurrentUser, cancellationToken);
        return Ok(ApiResponse<LeaveApplicationDto>.Ok(result, $"Application {result.RefNo} {result.Status.ToLower()}."));
    }

    // ── Balances ───────────────────────────────────────────────────

    /// <summary>Get leave balances for all employees.</summary>
    [HttpGet("balances")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaveBalanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalances(
        [FromQuery] string? search,
        [FromQuery] string? leaveType,
        CancellationToken cancellationToken)
    {
        var result = await _leaveService.GetBalancesAsync(search, leaveType, cancellationToken);
        return Ok(ApiResponse<IEnumerable<LeaveBalanceDto>>.Ok(result));
    }

    // ── Holidays ───────────────────────────────────────────────────

    /// <summary>Get all holidays.</summary>
    [HttpGet("holidays")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<HolidayDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHolidays(
        [FromQuery] string? search,
        [FromQuery] string? type,
        CancellationToken cancellationToken)
    {
        var result = await _leaveService.GetHolidaysAsync(search, type, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HolidayDto>>.Ok(result));
    }

    /// <summary>Add a new holiday.</summary>
    [HttpPost("holidays")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<HolidayDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateHoliday(
        [FromBody] CreateHolidayDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _leaveService.CreateHolidayAsync(dto, CurrentUser, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<HolidayDto>.Ok(result, "Holiday added successfully."));
    }

    /// <summary>Remove a holiday.</summary>
    [HttpDelete("holidays/{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHoliday(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        await _leaveService.DeleteHolidayAsync(id, CurrentUser, cancellationToken);
        return NoContent();
    }
}
