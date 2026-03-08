using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.DeductionType;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/deduction-types")]
[Authorize(Policy = "PayrollAdmin")]
public class DeductionTypesController : BaseController
{
    private readonly IDeductionTypeService _service;

    public DeductionTypesController(IDeductionTypeService service) => _service = service;

    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DeductionTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<DeductionTypeDto>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<DeductionTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var item = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<DeductionTypeDto>.Ok(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DeductionTypeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDeductionTypeDto dto, CancellationToken ct)
    {
        var item = await _service.CreateAsync(dto, CurrentUser, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponse<DeductionTypeDto>.Ok(item, "Deduction type created."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<DeductionTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeductionTypeDto dto, CancellationToken ct)
    {
        var item = await _service.UpdateAsync(id, dto, CurrentUser, ct);
        return Ok(ApiResponse<DeductionTypeDto>.Ok(item, "Deduction type updated."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return NoContent();
    }
}
