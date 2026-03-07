using AutoMapper;
using Microsoft.Extensions.Logging;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Employee;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper     _mapper;
    private readonly ILogger<EmployeeService> _logger;

    // Allowed status transitions: key = current status, value = allowed next statuses
    private static readonly Dictionary<EmploymentStatus, EmploymentStatus[]> AllowedTransitions = new()
    {
        [EmploymentStatus.Active]     = [EmploymentStatus.Inactive, EmploymentStatus.OnLeave, EmploymentStatus.Suspended, EmploymentStatus.Terminated, EmploymentStatus.Retired],
        [EmploymentStatus.Inactive]   = [EmploymentStatus.Active, EmploymentStatus.Terminated, EmploymentStatus.Retired],
        [EmploymentStatus.OnLeave]    = [EmploymentStatus.Active, EmploymentStatus.Terminated],
        [EmploymentStatus.Suspended]  = [EmploymentStatus.Active, EmploymentStatus.Terminated],
        [EmploymentStatus.Terminated] = [],
        [EmploymentStatus.Retired]    = [],
    };

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmployeeService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper     = mapper;
        _logger     = logger;
    }

    // ── Core CRUD ─────────────────────────────────────────────────────────────

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(
        PaginationParams pagination, int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Employees.GetPagedAsync(
            pagination.Page, pagination.PageSize, pagination.Search,
            departmentId, null, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(items);
        return PagedResult<EmployeeDto>.Create(dtos, totalCount, pagination.Page, pagination.PageSize);
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDetailDto> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);
        return _mapper.Map<EmployeeDetailDto>(employee);
    }

    public async Task<EmployeeDto> GetByEmployeeCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByEmployeeCodeAsync(code, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), code);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating employee {EmployeeCode}", dto.EmployeeCode);

        if (!await _unitOfWork.Employees.IsEmployeeCodeUniqueAsync(dto.EmployeeCode, cancellationToken: cancellationToken))
            throw new ConflictException($"Employee code '{dto.EmployeeCode}' is already in use.");

        if (!await _unitOfWork.Employees.IsEmailUniqueAsync(dto.Email, cancellationToken: cancellationToken))
            throw new ConflictException($"Email '{dto.Email}' is already registered.");

        if (dto.ManagerId.HasValue)
        {
            var manager = await _unitOfWork.Employees.GetByIdAsync(dto.ManagerId.Value, cancellationToken)
                ?? throw new AppException($"Manager with ID {dto.ManagerId.Value} does not exist.");
            if (manager.Status != EmploymentStatus.Active)
                throw new AppException("The assigned manager must be an Active employee.");
        }

        if (dto.BranchId.HasValue)
        {
            var branchExists = await _unitOfWork.Branches.ExistsAsync(b => b.Id == dto.BranchId.Value, cancellationToken);
            if (!branchExists) throw new AppException($"Branch with ID {dto.BranchId.Value} does not exist.");
        }

        var employee = _mapper.Map<Employee>(dto);
        employee.CreatedBy = createdBy;
        employee.CreatedAt = DateTime.UtcNow;

        if (employee.SameAsPresentAddress)
        {
            employee.PermanentAddress  = employee.PresentAddress;
            employee.PermanentCity     = employee.PresentCity;
            employee.PermanentProvince = employee.PresentProvince;
            employee.PermanentZipCode  = employee.PresentZipCode;
        }

        await _unitOfWork.Employees.AddAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Employee created: {Code} (Id: {Id})", employee.EmployeeCode, employee.Id);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(
        int id, UpdateEmployeeDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        if (employee.Status is EmploymentStatus.Terminated or EmploymentStatus.Retired)
            throw new AppException("Terminated or retired employees cannot be edited. Only documents and emergency contacts may be updated.");

        if (!await _unitOfWork.Employees.IsEmailUniqueAsync(dto.Email, id, cancellationToken))
            throw new ConflictException($"Email '{dto.Email}' is already registered.");

        if (dto.ManagerId.HasValue)
        {
            if (dto.ManagerId.Value == id)
                throw new AppException("An employee cannot be their own manager.");
            var manager = await _unitOfWork.Employees.GetByIdAsync(dto.ManagerId.Value, cancellationToken)
                ?? throw new AppException($"Manager with ID {dto.ManagerId.Value} does not exist.");
            if (manager.Status != EmploymentStatus.Active)
                throw new AppException("The assigned manager must be an Active employee.");
        }

        if (dto.BranchId.HasValue)
        {
            var branchExists = await _unitOfWork.Branches.ExistsAsync(b => b.Id == dto.BranchId.Value, cancellationToken);
            if (!branchExists) throw new AppException($"Branch with ID {dto.BranchId.Value} does not exist.");
        }

        _mapper.Map(dto, employee);
        employee.UpdatedBy = updatedBy;
        employee.UpdatedAt = DateTime.UtcNow;

        if (employee.SameAsPresentAddress)
        {
            employee.PermanentAddress  = employee.PresentAddress;
            employee.PermanentCity     = employee.PresentCity;
            employee.PermanentProvince = employee.PresentProvince;
            employee.PermanentZipCode  = employee.PresentZipCode;
        }

        await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Employee updated: {Id} by {By}", id, updatedBy);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        if (employee.Status == EmploymentStatus.Active)
            throw new AppException("Cannot delete an Active employee. Change their status first.");

        if (await _unitOfWork.Employees.HasActiveSubordinatesAsync(id, cancellationToken))
            throw new AppException("Cannot delete an employee who is currently managing active subordinates. Reassign them first.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Cascade soft-delete on emergency contacts and documents
            var contacts = await _unitOfWork.EmergencyContacts.GetByEmployeeIdAsync(id, cancellationToken);
            foreach (var c in contacts)
                await _unitOfWork.EmergencyContacts.DeleteAsync(c.Id, deletedBy, cancellationToken);

            var docs = await _unitOfWork.EmployeeDocuments.GetByEmployeeIdAsync(id, cancellationToken);
            foreach (var d in docs)
                await _unitOfWork.EmployeeDocuments.DeleteAsync(d.Id, deletedBy, cancellationToken);

            await _unitOfWork.Employees.DeleteAsync(id, deletedBy, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        _logger.LogInformation("Employee soft-deleted: {Id} by {By}", id, deletedBy);
    }

    // ── Status Management ────────────────────────────────────────────────────

    public async Task<EmployeeStatusHistoryDto> ChangeStatusAsync(
        int id, ChangeEmployeeStatusDto dto, string changedBy,
        CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        var newStatus = (EmploymentStatus)dto.NewStatus;

        if (!AllowedTransitions.TryGetValue(employee.Status, out var allowed) || !allowed.Contains(newStatus))
            throw new AppException($"Status transition from '{employee.Status}' to '{newStatus}' is not allowed.");

        if ((newStatus is EmploymentStatus.Terminated or EmploymentStatus.Retired) && !dto.LastWorkingDate.HasValue)
            throw new AppException("Last working date is required for Terminated or Retired status.");

        var history = new EmployeeStatusHistory
        {
            EmployeeId     = id,
            PreviousStatus = employee.Status,
            NewStatus      = newStatus,
            Remarks        = dto.Remarks,
            ChangedBy      = changedBy,
            ChangedAt      = DateTime.UtcNow,
            CreatedBy      = changedBy,
            CreatedAt      = DateTime.UtcNow,
        };

        employee.Status          = newStatus;
        employee.StatusRemarks   = dto.Remarks;
        employee.StatusChangedAt = DateTime.UtcNow;
        employee.StatusChangedBy = changedBy;
        employee.UpdatedBy       = changedBy;
        employee.UpdatedAt       = DateTime.UtcNow;

        if (dto.LastWorkingDate.HasValue)
        {
            employee.LastWorkingDate = dto.LastWorkingDate.Value;
            if (newStatus == EmploymentStatus.Terminated)
                employee.TerminationDate = dto.LastWorkingDate.Value;
        }

        await _unitOfWork.EmployeeStatusHistory.AddAsync(history, cancellationToken);
        await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Employee {Id} status changed to {Status} by {By}", id, newStatus, changedBy);
        return _mapper.Map<EmployeeStatusHistoryDto>(history);
    }

    public async Task<IEnumerable<EmployeeStatusHistoryDto>> GetStatusHistoryAsync(
        int id, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Employees.ExistsAsync(e => e.Id == id, cancellationToken))
            throw new NotFoundException(nameof(Employee), id);

        var history = await _unitOfWork.EmployeeStatusHistory.GetByEmployeeIdAsync(id, cancellationToken);
        return _mapper.Map<IEnumerable<EmployeeStatusHistoryDto>>(history);
    }

    // ── Emergency Contacts ────────────────────────────────────────────────────

    public async Task<IEnumerable<EmergencyContactDto>> GetEmergencyContactsAsync(
        int id, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Employees.ExistsAsync(e => e.Id == id, cancellationToken))
            throw new NotFoundException(nameof(Employee), id);

        var contacts = await _unitOfWork.EmergencyContacts.GetByEmployeeIdAsync(id, cancellationToken);
        return _mapper.Map<IEnumerable<EmergencyContactDto>>(contacts);
    }

    public async Task<EmergencyContactDto> AddEmergencyContactAsync(
        int id, CreateEmergencyContactDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Employees.ExistsAsync(e => e.Id == id, cancellationToken))
            throw new NotFoundException(nameof(Employee), id);

        var contact = _mapper.Map<EmployeeEmergencyContact>(dto);
        contact.EmployeeId = id;
        contact.CreatedBy  = createdBy;
        contact.CreatedAt  = DateTime.UtcNow;

        await _unitOfWork.EmergencyContacts.AddAsync(contact, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<EmergencyContactDto>(contact);
    }

    public async Task<EmergencyContactDto> UpdateEmergencyContactAsync(
        int employeeId, int contactId, UpdateEmergencyContactDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var contact = await _unitOfWork.EmergencyContacts.GetByIdAsync(contactId, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeEmergencyContact), contactId);

        if (contact.EmployeeId != employeeId)
            throw new NotFoundException(nameof(EmployeeEmergencyContact), contactId);

        _mapper.Map(dto, contact);
        contact.UpdatedBy = updatedBy;
        contact.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.EmergencyContacts.UpdateAsync(contact, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<EmergencyContactDto>(contact);
    }

    public async Task DeleteEmergencyContactAsync(
        int employeeId, int contactId, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        var contact = await _unitOfWork.EmergencyContacts.GetByIdAsync(contactId, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeEmergencyContact), contactId);

        if (contact.EmployeeId != employeeId)
            throw new NotFoundException(nameof(EmployeeEmergencyContact), contactId);

        await _unitOfWork.EmergencyContacts.DeleteAsync(contactId, deletedBy, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }

    // ── Documents ────────────────────────────────────────────────────────────

    public async Task<IEnumerable<EmployeeDocumentDto>> GetDocumentsAsync(
        int id, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Employees.ExistsAsync(e => e.Id == id, cancellationToken))
            throw new NotFoundException(nameof(Employee), id);

        var docs = await _unitOfWork.EmployeeDocuments.GetByEmployeeIdAsync(id, cancellationToken);
        return _mapper.Map<IEnumerable<EmployeeDocumentDto>>(docs);
    }

    public async Task<EmployeeDocumentDto> AddDocumentAsync(
        int id, UploadDocumentDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Employees.ExistsAsync(e => e.Id == id, cancellationToken))
            throw new NotFoundException(nameof(Employee), id);

        var doc = _mapper.Map<EmployeeDocument>(dto);
        doc.EmployeeId = id;
        doc.CreatedBy  = createdBy;
        doc.CreatedAt  = DateTime.UtcNow;

        await _unitOfWork.EmployeeDocuments.AddAsync(doc, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<EmployeeDocumentDto>(doc);
    }

    public async Task DeleteDocumentAsync(
        int employeeId, int documentId, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        var doc = await _unitOfWork.EmployeeDocuments.GetByIdAsync(documentId, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeDocument), documentId);

        if (doc.EmployeeId != employeeId)
            throw new NotFoundException(nameof(EmployeeDocument), documentId);

        await _unitOfWork.EmployeeDocuments.DeleteAsync(documentId, deletedBy, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
