using Microsoft.Extensions.Logging;
using PayrollApi.Application.Common.Exceptions;
#pragma warning disable CS0108
using PayrollApi.Application.DTOs.Leave;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class LeaveService : ILeaveService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<LeaveService> _logger;

    public LeaveService(IUnitOfWork uow, ILogger<LeaveService> logger)
    {
        _uow    = uow;
        _logger = logger;
    }

    // ── Applications ───────────────────────────────────────────────
    public async Task<IEnumerable<LeaveApplicationDto>> GetApplicationsAsync(
        string? search, string? status, string? leaveType,
        CancellationToken cancellationToken = default)
    {
        var items = await _uow.Leave.GetApplicationsAsync(search, status, leaveType, cancellationToken);
        return items.Select(MapApplication);
    }

    public async Task<LeaveApplicationDto> CreateApplicationAsync(
        CreateLeaveApplicationDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        if (!DateTime.TryParse(dto.StartDate, out var start) ||
            !DateTime.TryParse(dto.EndDate, out var end))
            throw new AppException("Invalid date format.");

        if (end < start)
            throw new AppException("End date cannot be before start date.");

        var refNo = await _uow.Leave.GenerateReferenceNumberAsync(cancellationToken);
        var days  = (int)(end.Date - start.Date).TotalDays + 1;

        var application = new LeaveApplication
        {
            ReferenceNumber = refNo,
            EmployeeName    = dto.Employee,
            LeaveType       = dto.LeaveType,
            StartDate       = start,
            EndDate         = end,
            DeductibleDays  = days,
            Reason          = dto.Reason,
            ApproverName    = dto.Approver,
            Status          = LeaveApplicationStatus.Pending,
            SubmittedOn     = DateTime.UtcNow,
            CreatedBy       = createdBy
        };

        await _uow.Leave.AddApplicationAsync(application, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Leave application {RefNo} created by {User}", refNo, createdBy);
        return MapApplication(application);
    }

    public async Task<LeaveApplicationDto> UpdateApplicationStatusAsync(
        int id, UpdateLeaveApplicationStatusDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var application = await _uow.Leave.GetApplicationByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("LeaveApplication", id);

        if (!Enum.TryParse<LeaveApplicationStatus>(dto.Status, out var newStatus))
            throw new AppException($"Invalid status '{dto.Status}'.");

        application.Status          = newStatus;
        application.ApproverRemarks = dto.Remarks;
        application.UpdatedBy       = updatedBy;
        application.UpdatedAt       = DateTime.UtcNow;

        await _uow.Leave.UpdateApplicationAsync(application, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Leave application {Id} status updated to {Status} by {User}", id, dto.Status, updatedBy);
        return MapApplication(application);
    }

    // ── Balances ───────────────────────────────────────────────────
    public async Task<IEnumerable<LeaveBalanceDto>> GetBalancesAsync(
        string? search, string? leaveType,
        CancellationToken cancellationToken = default)
    {
        var items = await _uow.Leave.GetBalancesAsync(search, leaveType, cancellationToken);
        return items.Select(b => new LeaveBalanceDto(
            b.Id, b.EmployeeCode, b.EmployeeName, b.LeaveType,
            b.Entitlement, b.Used, b.Pending, b.CarryOver, b.Remaining));
    }

    // ── Holidays ───────────────────────────────────────────────────
    public async Task<IEnumerable<HolidayDto>> GetHolidaysAsync(
        string? search, string? type,
        CancellationToken cancellationToken = default)
    {
        var items = await _uow.Leave.GetHolidaysAsync(search, type, cancellationToken);
        return items.Select(MapHoliday);
    }

    public async Task<HolidayDto> CreateHolidayAsync(
        CreateHolidayDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        if (!DateTime.TryParse(dto.Date, out var date))
            throw new AppException("Invalid holiday date.");

        if (!Enum.TryParse<HolidayType>(dto.Type, out var holidayType))
            throw new AppException($"Invalid holiday type '{dto.Type}'.");

        var holiday = new Holiday
        {
            Name        = dto.Name.Trim(),
            Date        = date,
            Type        = holidayType,
            Region      = dto.Region.Trim(),
            IsRecurring = dto.IsRecurring,
            CreatedBy   = createdBy
        };

        await _uow.Leave.AddHolidayAsync(holiday, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Holiday '{Name}' added by {User}", holiday.Name, createdBy);
        return MapHoliday(holiday);
    }

    public async Task DeleteHolidayAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var holiday = await _uow.Leave.GetHolidayByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Holiday", id);

        await _uow.Leave.DeleteHolidayAsync(id, deletedBy, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Holiday {Id} deleted by {User}", id, deletedBy);
    }

    // ── Mappers ────────────────────────────────────────────────────
    private static LeaveApplicationDto MapApplication(LeaveApplication a) => new(
        a.Id,
        a.ReferenceNumber,
        a.EmployeeName,
        a.LeaveType,
        a.StartDate.ToString("yyyy-MM-dd"),
        a.EndDate.ToString("yyyy-MM-dd"),
        a.DeductibleDays,
        a.Reason,
        a.Status.ToString(),
        a.SubmittedOn.ToString("yyyy-MM-dd"),
        a.ApproverName,
        a.ApproverRemarks
    );

    private static HolidayDto MapHoliday(Holiday h) => new(
        h.Id,
        h.Name,
        h.Date.ToString("yyyy-MM-dd"),
        h.Type.ToString(),
        h.Region,
        h.IsRecurring
    );
}
