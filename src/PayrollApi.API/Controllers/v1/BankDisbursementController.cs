using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Payroll;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/bank-disbursement")]
[Authorize(Policy = "PayrollViewer")]
public class BankDisbursementController : BaseController
{
    private readonly IBankDisbursementService _service;

    public BankDisbursementController(IBankDisbursementService service) => _service = service;

    [HttpGet("by-period/{periodId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BankDisbursementRow>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPeriod(int periodId, CancellationToken ct)
    {
        var rows = await _service.GetByPeriodAsync(periodId, ct);
        return Ok(ApiResponse<IEnumerable<BankDisbursementRow>>.Ok(rows));
    }
}
