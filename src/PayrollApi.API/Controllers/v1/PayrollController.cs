using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Payroll;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
public class PayrollController : BaseController
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService) => _payrollService = payrollService;

    /// <summary>Get paginated payroll records.</summary>
    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PayrollRecordDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] int? employeeId,
        [FromQuery] int? periodId,
        CancellationToken cancellationToken)
    {
        var result = await _payrollService.GetPagedAsync(pagination, employeeId, periodId, cancellationToken);
        var meta = new ApiMeta(result.Page, result.PageSize, result.TotalCount, result.TotalPages);
        return Ok(ApiResponse<PagedResult<PayrollRecordDto>>.Ok(result, meta: meta));
    }

    /// <summary>Get a single payroll record by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<PayrollRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _payrollService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PayrollRecordDto>.Ok(result));
    }

    /// <summary>
    /// Run payroll for a given period.
    /// Computes gross pay, statutory deductions (SSS, PhilHealth, Pag-IBIG, withholding tax), and net pay.
    /// </summary>
    [HttpPost("run")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PayrollRecordDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RunPayroll([FromBody] RunPayrollDto dto, CancellationToken cancellationToken)
    {
        var dtoWithUser = dto with { InitiatedBy = CurrentUser };
        var result = await _payrollService.RunPayrollAsync(dtoWithUser, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PayrollRecordDto>>.Ok(result, $"Payroll run completed. {result.Count()} records processed."));
    }

    /// <summary>Approve a payroll record.</summary>
    [HttpPatch("{id:int}/approve")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PayrollRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _payrollService.ApproveAsync(id, CurrentUser, cancellationToken);
        return Ok(ApiResponse<PayrollRecordDto>.Ok(result, "Payroll record approved."));
    }

    /// <summary>Release (disburse) an approved payroll record.</summary>
    [HttpPatch("{id:int}/release")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PayrollRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Release([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _payrollService.ReleaseAsync(id, CurrentUser, cancellationToken);
        return Ok(ApiResponse<PayrollRecordDto>.Ok(result, "Payroll record released."));
    }
}
