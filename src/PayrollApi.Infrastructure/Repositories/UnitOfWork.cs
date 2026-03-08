using Microsoft.EntityFrameworkCore.Storage;
using PayrollApi.Domain.Interfaces;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IUserRepository                  Users                  { get; }
    public IEmployeeRepository              Employees              { get; }
    public IEmployeeStatusHistoryRepository EmployeeStatusHistory  { get; }
    public IEmergencyContactRepository      EmergencyContacts      { get; }
    public IEmployeeDocumentRepository      EmployeeDocuments      { get; }
    public IBranchRepository                Branches               { get; }
    public IPayrollRepository               PayrollRecords         { get; }
    public IPayPeriodRepository             PayPeriods             { get; }
    public ILeaveRepository                 Leave                  { get; }
    public IRoleRepository                  Roles                  { get; }
    public ICompanySettingsRepository       CompanySettings        { get; }
    public IGlobalConfigRepository          GlobalConfigs          { get; }
    public IAllowanceTypeRepository         AllowanceTypes         { get; }
    public ILeaveTypeRepository             LeaveTypes             { get; }
    public IWorkScheduleRepository          WorkSchedules          { get; }
    public IScheduleRuleRepository          ScheduleRules          { get; }
    public IEmployeeScheduleRepository      EmployeeSchedules      { get; }
    public ISalaryHistoryRepository         SalaryHistory          { get; }
    public IAttendanceRepository            Attendances            { get; }
    public IDeductionTypeRepository         DeductionTypes         { get; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository users,
        IEmployeeRepository employees,
        IEmployeeStatusHistoryRepository employeeStatusHistory,
        IEmergencyContactRepository emergencyContacts,
        IEmployeeDocumentRepository employeeDocuments,
        IBranchRepository branches,
        IPayrollRepository payrollRecords,
        IPayPeriodRepository payPeriods,
        ILeaveRepository leave,
        IRoleRepository roles,
        ICompanySettingsRepository companySettings,
        IGlobalConfigRepository globalConfigs,
        IAllowanceTypeRepository allowanceTypes,
        IWorkScheduleRepository workSchedules,
        IScheduleRuleRepository scheduleRules,
        IEmployeeScheduleRepository employeeSchedules,
        ILeaveTypeRepository leaveTypes,
        ISalaryHistoryRepository salaryHistory,
        IAttendanceRepository attendances,
        IDeductionTypeRepository deductionTypes)
    {
        _context              = context;
        Users                 = users;
        Employees             = employees;
        EmployeeStatusHistory = employeeStatusHistory;
        EmergencyContacts     = emergencyContacts;
        EmployeeDocuments     = employeeDocuments;
        Branches              = branches;
        PayrollRecords        = payrollRecords;
        PayPeriods            = payPeriods;
        Leave                 = leave;
        Roles                 = roles;
        CompanySettings       = companySettings;
        GlobalConfigs         = globalConfigs;
        AllowanceTypes        = allowanceTypes;
        WorkSchedules         = workSchedules;
        ScheduleRules         = scheduleRules;
        EmployeeSchedules     = employeeSchedules;
        LeaveTypes            = leaveTypes;
        SalaryHistory         = salaryHistory;
        Attendances           = attendances;
        DeductionTypes        = deductionTypes;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        return result;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
