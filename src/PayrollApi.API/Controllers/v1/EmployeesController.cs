using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Employee;
using PayrollApi.Application.Services.Interfaces;

namespace PayrollApi.API.Controllers.v1;

[ApiVersion("1.0")]
public class EmployeesController : BaseController
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    /// <summary>Get a paginated list of employees.</summary>
    [HttpGet]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationParams pagination,
        [FromQuery] int? departmentId,
        CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetPagedAsync(pagination, departmentId, cancellationToken);
        var meta = new ApiMeta(result.Page, result.PageSize, result.TotalCount, result.TotalPages);
        return Ok(ApiResponse<PagedResult<EmployeeDto>>.Ok(result, meta: meta));
    }

    /// <summary>Get a single employee by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<EmployeeDto>.Ok(result));
    }

    /// <summary>Get a single employee by employee code.</summary>
    [HttpGet("code/{code}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode([FromRoute] string code, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByEmployeeCodeAsync(code, cancellationToken);
        return Ok(ApiResponse<EmployeeDto>.Ok(result));
    }

    /// <summary>Create a new employee.</summary>
    [HttpPost]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var result = await _employeeService.CreateAsync(dto, CurrentUser, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id, version = "1" },
            ApiResponse<EmployeeDto>.Ok(result, "Employee created successfully."));
    }

    /// <summary>Update an existing employee.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] int id, [FromBody] UpdateEmployeeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _employeeService.UpdateAsync(id, dto, CurrentUser, cancellationToken);
        return Ok(ApiResponse<EmployeeDto>.Ok(result, "Employee updated successfully."));
    }

    /// <summary>Soft-delete an employee.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _employeeService.DeleteAsync(id, CurrentUser, cancellationToken);
        return NoContent();
    }
}
