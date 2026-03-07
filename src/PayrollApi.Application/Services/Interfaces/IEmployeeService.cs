using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Employee;

namespace PayrollApi.Application.Services.Interfaces;

public interface IEmployeeService
{
    // Core CRUD
    Task<PagedResult<EmployeeDto>> GetPagedAsync(PaginationParams pagination, int? departmentId = null, CancellationToken cancellationToken = default);
    Task<EmployeeDto>       GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDetailDto> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto>       GetByEmployeeCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<EmployeeDto>       CreateAsync(CreateEmployeeDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<EmployeeDto>       UpdateAsync(int id, UpdateEmployeeDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task                    DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);

    // Status management
    Task<EmployeeStatusHistoryDto> ChangeStatusAsync(int id, ChangeEmployeeStatusDto dto, string changedBy, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeStatusHistoryDto>> GetStatusHistoryAsync(int id, CancellationToken cancellationToken = default);

    // Emergency contacts
    Task<IEnumerable<EmergencyContactDto>> GetEmergencyContactsAsync(int id, CancellationToken cancellationToken = default);
    Task<EmergencyContactDto> AddEmergencyContactAsync(int id, CreateEmergencyContactDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<EmergencyContactDto> UpdateEmergencyContactAsync(int employeeId, int contactId, UpdateEmergencyContactDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteEmergencyContactAsync(int employeeId, int contactId, string deletedBy, CancellationToken cancellationToken = default);

    // Documents
    Task<IEnumerable<EmployeeDocumentDto>> GetDocumentsAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDocumentDto> AddDocumentAsync(int id, UploadDocumentDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(int employeeId, int documentId, string deletedBy, CancellationToken cancellationToken = default);
}
