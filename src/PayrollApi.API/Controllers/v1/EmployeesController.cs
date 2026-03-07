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
        _logger          = logger;
    }

    // ── Core CRUD ──────────────────────────────────────────────────────────

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
        var meta   = new ApiMeta(result.Page, result.PageSize, result.TotalCount, result.TotalPages);
        return Ok(ApiResponse<PagedResult<EmployeeDto>>.Ok(result, meta: meta));
    }

    /// <summary>Get a single employee summary by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<EmployeeDto>.Ok(result));
    }

    /// <summary>Get full employee detail including related data.</summary>
    [HttpGet("{id:int}/details")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetDetailAsync(id, cancellationToken);
        return Ok(ApiResponse<EmployeeDetailDto>.Ok(result));
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

    // ── Status Management ────────────────────────────────────────────────

    /// <summary>Change employee employment status.</summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeStatusHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] int id, [FromBody] ChangeEmployeeStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _employeeService.ChangeStatusAsync(id, dto, CurrentUser, cancellationToken);
        return Ok(ApiResponse<EmployeeStatusHistoryDto>.Ok(result, "Employee status updated."));
    }

    /// <summary>Get the complete status change history for an employee.</summary>
    [HttpGet("{id:int}/status-history")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeStatusHistoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatusHistory([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetStatusHistoryAsync(id, cancellationToken);
        return Ok(ApiResponse<IEnumerable<EmployeeStatusHistoryDto>>.Ok(result));
    }

    // ── Emergency Contacts ───────────────────────────────────────────────

    /// <summary>List all emergency contacts for an employee.</summary>
    [HttpGet("{id:int}/emergency-contacts")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmergencyContactDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmergencyContacts([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetEmergencyContactsAsync(id, cancellationToken);
        return Ok(ApiResponse<IEnumerable<EmergencyContactDto>>.Ok(result));
    }

    /// <summary>Add an emergency contact.</summary>
    [HttpPost("{id:int}/emergency-contacts")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EmergencyContactDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddEmergencyContact(
        [FromRoute] int id, [FromBody] CreateEmergencyContactDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _employeeService.AddEmergencyContactAsync(id, dto, CurrentUser, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<EmergencyContactDto>.Ok(result));
    }

    /// <summary>Update an emergency contact.</summary>
    [HttpPut("{id:int}/emergency-contacts/{contactId:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EmergencyContactDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEmergencyContact(
        [FromRoute] int id, [FromRoute] int contactId,
        [FromBody] UpdateEmergencyContactDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _employeeService.UpdateEmergencyContactAsync(id, contactId, dto, CurrentUser, cancellationToken);
        return Ok(ApiResponse<EmergencyContactDto>.Ok(result));
    }

    /// <summary>Remove an emergency contact (soft delete).</summary>
    [HttpDelete("{id:int}/emergency-contacts/{contactId:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteEmergencyContact(
        [FromRoute] int id, [FromRoute] int contactId,
        CancellationToken cancellationToken)
    {
        await _employeeService.DeleteEmergencyContactAsync(id, contactId, CurrentUser, cancellationToken);
        return NoContent();
    }

    // ── Documents ────────────────────────────────────────────────────────

    /// <summary>List all documents for an employee.</summary>
    [HttpGet("{id:int}/documents")]
    [Authorize(Policy = "PayrollViewer")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDocumentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocuments([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetDocumentsAsync(id, cancellationToken);
        return Ok(ApiResponse<IEnumerable<EmployeeDocumentDto>>.Ok(result));
    }

    /// <summary>Upload/attach a document.</summary>
    [HttpPost("{id:int}/documents")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDocumentDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddDocument(
        [FromRoute] int id, [FromBody] UploadDocumentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _employeeService.AddDocumentAsync(id, dto, CurrentUser, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<EmployeeDocumentDto>.Ok(result));
    }

    /// <summary>Remove a document (soft delete).</summary>
    [HttpDelete("{id:int}/documents/{documentId:int}")]
    [Authorize(Policy = "PayrollAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteDocument(
        [FromRoute] int id, [FromRoute] int documentId,
        CancellationToken cancellationToken)
    {
        await _employeeService.DeleteDocumentAsync(id, documentId, CurrentUser, cancellationToken);
        return NoContent();
    }
}
