using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.API.Controllers.v1;

public class DepartmentLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class PositionLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public int DepartmentId { get; set; }
}

public class BranchLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

[ApiVersion("1.0")]
public class LookupsController : BaseController
{
    private readonly ISqlExecutor _sql;

    public LookupsController(ISqlExecutor sql) { _sql = sql; }

    /// <summary>Get all departments for dropdowns.</summary>
    [HttpGet("departments")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentLookupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var data = await _sql.QueryAsync<DepartmentLookupDto>(
            "sp_Lookup", new { ActionType = "GET_DEPARTMENTS" }, cancellationToken);
        return Ok(ApiResponse<IEnumerable<DepartmentLookupDto>>.Ok(data));
    }

    /// <summary>Get all positions for dropdowns, optionally filtered by department.</summary>
    [HttpGet("positions")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PositionLookupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPositions([FromQuery] int? departmentId, CancellationToken cancellationToken)
    {
        var data = await _sql.QueryAsync<PositionLookupDto>(
            "sp_Lookup", new { ActionType = "GET_POSITIONS", DepartmentId = departmentId }, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PositionLookupDto>>.Ok(data));
    }

    /// <summary>Get all branches for dropdowns.</summary>
    [HttpGet("branches")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BranchLookupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        var data = await _sql.QueryAsync<BranchLookupDto>(
            "sp_Lookup", new { ActionType = "GET_BRANCHES" }, cancellationToken);
        return Ok(ApiResponse<IEnumerable<BranchLookupDto>>.Ok(data));
    }
}
