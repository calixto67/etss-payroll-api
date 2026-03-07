using PayrollApi.Domain.Interfaces.Repositories;

namespace PayrollApi.Domain.Interfaces;

/// <summary>
/// Unit of Work: groups repository operations into a single transaction boundary.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IUserRepository                  Users                  { get; }
    IEmployeeRepository              Employees              { get; }
    IEmployeeStatusHistoryRepository EmployeeStatusHistory  { get; }
    IEmergencyContactRepository      EmergencyContacts      { get; }
    IEmployeeDocumentRepository      EmployeeDocuments      { get; }
    IBranchRepository                Branches               { get; }
    IPayrollRepository               PayrollRecords         { get; }
    ILeaveRepository                 Leave                  { get; }
    IRoleRepository                  Roles                  { get; }
    ICompanySettingsRepository       CompanySettings        { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
}
