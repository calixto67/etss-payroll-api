using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.ContributionBracket;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contribution-brackets")]
[Authorize(Policy = "PayrollAdmin")]
public class ContributionBracketsController : BaseController
{
    private readonly IContributionBracketService _service;

    public ContributionBracketsController(IContributionBracketService service) => _service = service;

    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContributionBracketDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string type, CancellationToken ct)
    {
        var items = await _service.GetAllAsync(type, ct);
        return Ok(ApiResponse<IEnumerable<ContributionBracketDto>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<ContributionBracketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<ContributionBracketDto>.Ok(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContributionBracketDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateContributionBracketDto dto, CancellationToken ct)
    {
        var item = await _service.CreateAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponse<ContributionBracketDto>.Ok(item, "Contribution bracket created."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ContributionBracketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateContributionBracketDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<ContributionBracketDto>.Ok(item, "Contribution bracket updated."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return NoContent();
    }
}
