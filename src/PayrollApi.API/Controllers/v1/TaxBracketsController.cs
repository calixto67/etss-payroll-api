using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.TaxBracket;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tax-brackets")]
[Authorize(Policy = "PayrollAdmin")]
public class TaxBracketsController : BaseController
{
    private readonly ITaxBracketService _service;

    public TaxBracketsController(ITaxBracketService service) => _service = service;

    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TaxBracketDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<TaxBracketDto>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<TaxBracketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<TaxBracketDto>.Ok(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaxBracketDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTaxBracketDto dto, CancellationToken ct)
    {
        var item = await _service.CreateAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponse<TaxBracketDto>.Ok(item, "Tax bracket created."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TaxBracketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaxBracketDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<TaxBracketDto>.Ok(item, "Tax bracket updated."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return NoContent();
    }
}
