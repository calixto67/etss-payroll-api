using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.PayPeriod;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pay-periods")]
public class PayPeriodsController : BaseController
{
    private readonly IPayPeriodService _service;

    public PayPeriodsController(IPayPeriodService service) => _service = service;

    /// <summary>Get all pay periods, optionally filtered by year and status.</summary>
    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PayPeriodDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? year,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(year, status, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PayPeriodDto>>.Ok(result));
    }

    /// <summary>Get a single pay period by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<PayPeriodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PayPeriodDto>.Ok(result));
    }

    /// <summary>Create a new pay period (auto-generates period code from dates).</summary>
    [HttpPost]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PayPeriodDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePayPeriodDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, CurrentUser, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<PayPeriodDto>.Ok(result, "Pay period created successfully."));
    }

    /// <summary>Update the status of a pay period (e.g. open, computed, approved, locked, exported).</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PayPeriodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] int id,
        [FromBody] UpdatePayPeriodStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateStatusAsync(id, dto.Status, CurrentUser, cancellationToken);
        return Ok(ApiResponse<PayPeriodDto>.Ok(result, "Status updated successfully."));
    }

    /// <summary>Delete a draft pay period.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, CurrentUser, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null!, "Pay period deleted."));
    }
}
