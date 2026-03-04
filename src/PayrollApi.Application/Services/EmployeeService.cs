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
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmployeeService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(
        PaginationParams pagination, int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching employees - Page: {Page}, PageSize: {PageSize}, Search: {Search}",
            pagination.Page, pagination.PageSize, pagination.Search);

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
        _logger.LogInformation("Creating employee with code {EmployeeCode}", dto.EmployeeCode);

        if (!await _unitOfWork.Employees.IsEmployeeCodeUniqueAsync(dto.EmployeeCode, cancellationToken: cancellationToken))
            throw new ConflictException($"Employee code '{dto.EmployeeCode}' is already in use.");

        if (!await _unitOfWork.Employees.IsEmailUniqueAsync(dto.Email, cancellationToken: cancellationToken))
            throw new ConflictException($"Email '{dto.Email}' is already registered.");

        var employee = _mapper.Map<Employee>(dto);
        employee.CreatedBy = createdBy;

        await _unitOfWork.Employees.AddAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Employee created: {EmployeeCode} (Id: {Id})", employee.EmployeeCode, employee.Id);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(
        int id, UpdateEmployeeDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        if (!await _unitOfWork.Employees.IsEmailUniqueAsync(dto.Email, id, cancellationToken))
            throw new ConflictException($"Email '{dto.Email}' is already registered.");

        _mapper.Map(dto, employee);
        employee.UpdatedBy = updatedBy;
        employee.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Employees.UpdateAsync(employee, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Employee updated: {Id} by {UpdatedBy}", id, updatedBy);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        await _unitOfWork.Employees.DeleteAsync(id, deletedBy, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Employee soft-deleted: {Id} by {DeletedBy}", id, deletedBy);
    }
}
