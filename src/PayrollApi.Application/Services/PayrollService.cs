using AutoMapper;
using Microsoft.Extensions.Logging;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Payroll;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class PayrollService : IPayrollService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PayrollService> _logger;

    public PayrollService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PayrollService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<PayrollRecordDto>> GetPagedAsync(
        PaginationParams pagination, int? employeeId, int? periodId,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.PayrollRecords.GetPagedAsync(
            pagination.Page, pagination.PageSize, employeeId, periodId, null, cancellationToken);

        var dtos = _mapper.Map<IEnumerable<PayrollRecordDto>>(items);
        return PagedResult<PayrollRecordDto>.Create(dtos, totalCount, pagination.Page, pagination.PageSize);
    }

    public async Task<PayrollRecordDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var record = await _unitOfWork.PayrollRecords.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(PayrollRecord), id);

        return _mapper.Map<PayrollRecordDto>(record);
    }

    public async Task<IEnumerable<PayrollRecordDto>> RunPayrollAsync(
        RunPayrollDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running payroll for period {PeriodId} initiated by {InitiatedBy}",
            dto.PayrollPeriodId, dto.InitiatedBy);

        var employees = dto.EmployeeIds?.Any() == true
            ? await _unitOfWork.Employees.FindAsync(
                e => dto.EmployeeIds.Contains(e.Id) && e.Status == EmploymentStatus.Active,
                cancellationToken)
            : await _unitOfWork.Employees.FindAsync(
                e => e.Status == EmploymentStatus.Active,
                cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var results = new List<PayrollRecord>();

            foreach (var employee in employees)
            {
                var existing = await _unitOfWork.PayrollRecords
                    .GetByEmployeeAndPeriodAsync(employee.Id, dto.PayrollPeriodId, cancellationToken);

                if (existing is not null)
                {
                    _logger.LogWarning("Payroll already exists for Employee {EmployeeId} Period {PeriodId} — skipping.",
                        employee.Id, dto.PayrollPeriodId);
                    continue;
                }

                var record = ComputePayroll(employee, dto.PayrollPeriodId, dto.InitiatedBy);
                await _unitOfWork.PayrollRecords.AddAsync(record, cancellationToken);
                results.Add(record);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Payroll run complete. {Count} records created.", results.Count);

            return _mapper.Map<IEnumerable<PayrollRecordDto>>(results);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<PayrollRecordDto> ApproveAsync(
        int id, string approvedBy, CancellationToken cancellationToken = default)
    {
        var record = await _unitOfWork.PayrollRecords.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(PayrollRecord), id);

        if (record.Status != PayrollStatus.ForApproval)
            throw new AppException($"Payroll record {id} is not in 'ForApproval' status.");

        record.Status = PayrollStatus.Approved;
        record.UpdatedBy = approvedBy;
        record.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.PayrollRecords.UpdateAsync(record, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Payroll record {Id} approved by {ApprovedBy}", id, approvedBy);

        return _mapper.Map<PayrollRecordDto>(record);
    }

    public async Task<PayrollRecordDto> ReleaseAsync(
        int id, string releasedBy, CancellationToken cancellationToken = default)
    {
        var record = await _unitOfWork.PayrollRecords.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(PayrollRecord), id);

        if (record.Status != PayrollStatus.Approved)
            throw new AppException($"Payroll record {id} must be approved before release.");

        record.Status = PayrollStatus.Released;
        record.ProcessedAt = DateTime.UtcNow;
        record.ProcessedBy = releasedBy;
        record.UpdatedBy = releasedBy;
        record.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.PayrollRecords.UpdateAsync(record, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Payroll record {Id} released by {ReleasedBy}", id, releasedBy);

        return _mapper.Map<PayrollRecordDto>(record);
    }

    // ─── Private: Payroll Computation ─────────────────────────────────────────

    private static PayrollRecord ComputePayroll(Employee employee, int periodId, string initiatedBy)
    {
        var basicPay = employee.BasicSalary;
        var grossPay = basicPay; // extend with OT, holiday, allowances as needed

        // Philippine statutory deductions (simplified flat-rate placeholders)
        var sss = ComputeSss(basicPay);
        var philHealth = ComputePhilHealth(basicPay);
        var pagIbig = 100m; // flat Pag-IBIG employee share
        var tax = ComputeWithholdingTax(grossPay - sss - philHealth - pagIbig);

        var totalDeductions = sss + philHealth + pagIbig + tax;

        return new PayrollRecord
        {
            EmployeeId = employee.Id,
            PayrollPeriodId = periodId,
            BasicPay = basicPay,
            OvertimePay = 0,
            HolidayPay = 0,
            Allowances = 0,
            GrossPay = grossPay,
            SssDeduction = sss,
            PhilHealthDeduction = philHealth,
            PagIbigDeduction = pagIbig,
            TaxWithheld = tax,
            OtherDeductions = 0,
            TotalDeductions = totalDeductions,
            NetPay = grossPay - totalDeductions,
            Status = PayrollStatus.ForApproval,
            CreatedBy = initiatedBy,
            CreatedAt = DateTime.UtcNow,
        };
    }

    private static decimal ComputeSss(decimal monthlySalary)
    {
        // SSS 2024 table simplified — replace with actual contribution table
        return monthlySalary switch
        {
            <= 4249 => 180m,
            <= 29750 => Math.Round(monthlySalary * 0.045m / 10, MidpointRounding.AwayFromZero) * 10,
            _ => 1350m
        };
    }

    private static decimal ComputePhilHealth(decimal monthlySalary)
    {
        // PhilHealth 2024: 5% of salary, employee share = 2.5%, max 50k salary base
        var basis = Math.Min(monthlySalary, 50000m);
        return Math.Round(basis * 0.025m, 2);
    }

    private static decimal ComputeWithholdingTax(decimal taxableIncome)
    {
        // BIR 2023 monthly tax table (TRAIN Law)
        return taxableIncome switch
        {
            <= 20833 => 0m,
            <= 33332 => Math.Round((taxableIncome - 20833m) * 0.15m, 2),
            <= 66666 => Math.Round(1875m + (taxableIncome - 33333m) * 0.20m, 2),
            <= 166666 => Math.Round(8541.80m + (taxableIncome - 66667m) * 0.25m, 2),
            <= 666666 => Math.Round(33541.80m + (taxableIncome - 166667m) * 0.30m, 2),
            _ => Math.Round(183541.80m + (taxableIncome - 666667m) * 0.35m, 2)
        };
    }
}
