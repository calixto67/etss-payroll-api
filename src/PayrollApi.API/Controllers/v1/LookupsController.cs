using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollApi.Application.Common.Models;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.API.Controllers.v1;

public record DepartmentLookupDto(int Id, string Code, string Name);
public record PositionLookupDto(int Id, string Code, string Title, int DepartmentId);
public record BranchLookupDto(int Id, string Code, string Name);

[ApiVersion("1.0")]
public class LookupsController : BaseController
{
    private readonly AppDbContext _db;

    public LookupsController(AppDbContext db) { _db = db; }

    /// <summary>Get all departments for dropdowns.</summary>
    [HttpGet("departments")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentLookupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var data = await _db.Departments
            .AsNoTracking()
            .OrderBy(d => d.DepartmentName)
            .Select(d => new DepartmentLookupDto(d.Id, d.DepartmentCode, d.DepartmentName))
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<IEnumerable<DepartmentLookupDto>>.Ok(data));
    }

    /// <summary>Get all positions for dropdowns, optionally filtered by department.</summary>
    [HttpGet("positions")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PositionLookupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPositions([FromQuery] int? departmentId, CancellationToken cancellationToken)
    {
        var query = _db.Positions.AsNoTracking();
        if (departmentId.HasValue)
            query = query.Where(p => p.DepartmentId == departmentId.Value);

        var data = await query
            .OrderBy(p => p.PositionTitle)
            .Select(p => new PositionLookupDto(p.Id, p.PositionCode, p.PositionTitle, p.DepartmentId))
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<IEnumerable<PositionLookupDto>>.Ok(data));
    }

    /// <summary>Get all branches for dropdowns.</summary>
    [HttpGet("branches")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BranchLookupDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        var data = await _db.Branches
            .AsNoTracking()
            .OrderBy(b => b.BranchName)
            .Select(b => new BranchLookupDto(b.Id, b.BranchCode, b.BranchName))
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<IEnumerable<BranchLookupDto>>.Ok(data));
    }
}
