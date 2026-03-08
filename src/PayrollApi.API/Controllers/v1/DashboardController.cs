using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Dashboard;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dashboard")]
public class DashboardController : BaseController
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    /// <summary>Get the full dashboard summary (employee counts, active period, pending tasks, etc.).</summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] int? periodId,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSummaryAsync(periodId, cancellationToken);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
    }
}
