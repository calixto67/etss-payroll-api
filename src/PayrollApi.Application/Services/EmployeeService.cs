using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using PayrollApi.Application.Common.Exceptions;
using PayrollApi.Application.Common.Models;
using PayrollApi.Application.DTOs.Employee;
using PayrollApi.Application.Services.Interfaces;
using PayrollApi.Domain.Interfaces;

namespace PayrollApi.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ISqlExecutor _sql;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(ISqlExecutor sql, ILogger<EmployeeService> logger)
    {
        _sql    = sql;
        _logger = logger;
    }

    // ── Internal row classes for Dapper mapping ──────────────────────────

    private sealed class EmployeeRow
    {
        public int      Id              { get; set; }
        public string   EmployeeCode    { get; set; } = "";
        public string   FirstName       { get; set; } = "";
        public string?  MiddleName      { get; set; }
        public string   LastName        { get; set; } = "";
        public string?  Suffix          { get; set; }
        public DateTime DateOfBirth     { get; set; }
        public int      Gender          { get; set; }
        public int      MaritalStatus   { get; set; }
        public string   Email           { get; set; } = "";
        public string   MobileNumber    { get; set; } = "";
        public string?  AlternatePhone  { get; set; }
        public DateTime HireDate        { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public int      Status          { get; set; }
        public string?  StatusRemarks   { get; set; }
        public int      EmploymentType  { get; set; }
        public int      DepartmentId    { get; set; }
        public string?  DepartmentName  { get; set; }
        public int      PositionId      { get; set; }
        public string?  PositionTitle   { get; set; }
        public int?     ManagerId       { get; set; }
        public string?  ManagerName     { get; set; }
        public int?     BranchId        { get; set; }
        public string?  BranchName      { get; set; }
        public decimal  BasicSalary     { get; set; }
        public int      SalaryFrequency { get; set; }
        public string   SssNumber       { get; set; } = "";
        public string?  ProfilePhotoPath { get; set; }
        public DateTime CreatedAt       { get; set; }
        public DateTime? UpdatedAt      { get; set; }
    }

    private sealed class EmployeeDetailRow
    {
        public int      Id              { get; set; }
        public string   EmployeeCode    { get; set; } = "";
        public string   FirstName       { get; set; } = "";
        public string?  MiddleName      { get; set; }
        public string   LastName        { get; set; } = "";
        public string?  Suffix          { get; set; }
        public DateTime DateOfBirth     { get; set; }
        public int      Gender          { get; set; }
        public int      MaritalStatus   { get; set; }

        // Government IDs
        public string TaxIdentificationNumber { get; set; } = "";
        public string SssNumber               { get; set; } = "";
        public string PhilHealthNumber        { get; set; } = "";
        public string PagIbigNumber           { get; set; } = "";

        // Contact
        public string  Email          { get; set; } = "";
        public string? PersonalEmail  { get; set; }
        public string  MobileNumber   { get; set; } = "";
        public string? AlternatePhone { get; set; }

        // Present Address
        public string PresentAddress  { get; set; } = "";
        public string PresentCity     { get; set; } = "";
        public string PresentProvince { get; set; } = "";
        public string PresentZipCode  { get; set; } = "";

        // Permanent Address
        public bool    SameAsPresentAddress { get; set; }
        public string? PermanentAddress     { get; set; }
        public string? PermanentCity        { get; set; }
        public string? PermanentProvince    { get; set; }
        public string? PermanentZipCode     { get; set; }

        // Employment
        public int  DepartmentId   { get; set; }
        public string? DepartmentName { get; set; }
        public int  PositionId     { get; set; }
        public string? PositionTitle  { get; set; }
        public int? ManagerId      { get; set; }
        public string? ManagerName    { get; set; }
        public int? BranchId       { get; set; }
        public string? BranchName     { get; set; }
        public int  EmploymentType { get; set; }

        // Dates
        public DateTime  HireDate           { get; set; }
        public DateTime? TerminationDate    { get; set; }
        public DateTime? ProbationEndDate   { get; set; }
        public DateTime? RegularizationDate { get; set; }
        public DateTime? LastWorkingDate    { get; set; }

        // Compensation
        public decimal  BasicSalary         { get; set; }
        public int      SalaryFrequency     { get; set; }
        public DateTime SalaryEffectiveDate { get; set; }
        public string   BankName            { get; set; } = "";

        // Profile
        public string? ProfilePhotoPath { get; set; }
        public string? BiometricId      { get; set; }

        // Status
        public int       Status          { get; set; }
        public string?   StatusRemarks   { get; set; }
        public DateTime? StatusChangedAt { get; set; }
        public string?   StatusChangedBy { get; set; }

        // Audit
        public string   CreatedBy { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string?  UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    private sealed class StatusHistoryRow
    {
        public int      Id             { get; set; }
        public int      PreviousStatus { get; set; }
        public int      NewStatus      { get; set; }
        public string   Remarks        { get; set; } = "";
        public string   ChangedBy      { get; set; } = "";
        public DateTime ChangedAt      { get; set; }
    }

    private sealed class EmergencyContactRow
    {
        public int     Id             { get; set; }
        public int     EmployeeId     { get; set; }
        public string  ContactName    { get; set; } = "";
        public string  Relationship   { get; set; } = "";
        public string  MobileNumber   { get; set; } = "";
        public string? AlternatePhone { get; set; }
        public bool    IsPrimary      { get; set; }
    }

    private sealed class DocumentRow
    {
        public int       Id           { get; set; }
        public int       EmployeeId   { get; set; }
        public int       DocumentType { get; set; }
        public string    DocumentName { get; set; } = "";
        public string    FilePath     { get; set; } = "";
        public DateTime? ExpiryDate   { get; set; }
        public bool      IsVerified   { get; set; }
        public DateTime  CreatedAt    { get; set; }
    }

    private sealed class SalaryHistoryRow
    {
        public int      Id              { get; set; }
        public decimal  PreviousSalary  { get; set; }
        public decimal  NewSalary       { get; set; }
        public int      SalaryFrequency { get; set; }
        public DateTime EffectiveDate   { get; set; }
        public string   ChangedBy       { get; set; } = "";
        public DateTime ChangedAt       { get; set; }
        public string?  Remarks         { get; set; }
    }

    // ── Status / enum name helpers ───────────────────────────────────────

    private static string StatusName(int status) => status switch
    {
        1 => "Active",
        2 => "Inactive",
        3 => "OnLeave",
        4 => "Terminated",
        5 => "Retired",
        6 => "Suspended",
        _ => "Unknown"
    };

    private static string EmploymentTypeName(int type) => type switch
    {
        1 => "Regular",
        2 => "Probationary",
        3 => "Contractual",
        4 => "PartTime",
        5 => "Consultant",
        _ => "Unknown"
    };

    private static string GenderName(int gender) => gender switch
    {
        0 => "Male",
        1 => "Female",
        2 => "Other",
        _ => "Unknown"
    };

    private static string MaritalStatusName(int ms) => ms switch
    {
        0 => "Single",
        1 => "Married",
        2 => "Widowed",
        3 => "Separated",
        4 => "Divorced",
        _ => "Unknown"
    };

    private static string SalaryFrequencyName(int freq) => freq switch
    {
        0 => "Monthly",
        1 => "SemiMonthly",
        2 => "Weekly",
        3 => "Daily",
        _ => "Unknown"
    };

    private static string DocumentTypeName(int type) => type switch
    {
        0 => "GovernmentId",
        1 => "Certificate",
        2 => "Contract",
        3 => "Resume",
        4 => "Medical",
        5 => "Other",
        _ => "Unknown"
    };

    // ── Row → DTO mapping helpers ────────────────────────────────────────

    private static EmployeeDto MapToEmployeeDto(EmployeeRow r) => new()
    {
        Id             = r.Id,
        EmployeeCode   = r.EmployeeCode,
        FirstName      = r.FirstName,
        MiddleName     = r.MiddleName,
        LastName       = r.LastName,
        Suffix         = r.Suffix,
        DateOfBirth    = r.DateOfBirth,
        Gender         = GenderName(r.Gender),
        MaritalStatus  = MaritalStatusName(r.MaritalStatus),
        Email          = r.Email,
        MobileNumber   = r.MobileNumber,
        AlternatePhone = r.AlternatePhone,
        HireDate       = r.HireDate,
        LastWorkingDate = r.LastWorkingDate,
        Status         = StatusName(r.Status),
        StatusRemarks  = r.StatusRemarks,
        EmploymentType = EmploymentTypeName(r.EmploymentType),
        DepartmentId   = r.DepartmentId,
        DepartmentName = r.DepartmentName,
        PositionId     = r.PositionId,
        PositionTitle  = r.PositionTitle,
        ManagerId      = r.ManagerId,
        ManagerName    = r.ManagerName,
        BranchId       = r.BranchId,
        BranchName     = r.BranchName,
        BasicSalary    = r.BasicSalary,
        SalaryFrequency = SalaryFrequencyName(r.SalaryFrequency),
        SssNumber      = r.SssNumber,
        CreatedAt      = r.CreatedAt,
        UpdatedAt      = r.UpdatedAt,
    };

    private static EmployeeDetailDto MapToDetailDto(
        EmployeeDetailRow r,
        IEnumerable<EmployeeStatusHistoryDto> statusHistory,
        IEnumerable<EmergencyContactDto> contacts,
        IEnumerable<EmployeeDocumentDto> documents,
        IEnumerable<SalaryHistoryDto> salaryHistory) => new()
    {
        Id              = r.Id,
        EmployeeCode    = r.EmployeeCode,
        FirstName       = r.FirstName,
        MiddleName      = r.MiddleName,
        LastName        = r.LastName,
        Suffix          = r.Suffix,
        DateOfBirth     = r.DateOfBirth,
        Gender          = GenderName(r.Gender),
        MaritalStatus   = MaritalStatusName(r.MaritalStatus),
        TaxIdentificationNumber = r.TaxIdentificationNumber,
        SssNumber       = r.SssNumber,
        PhilHealthNumber = r.PhilHealthNumber,
        PagIbigNumber   = r.PagIbigNumber,
        Email           = r.Email,
        PersonalEmail   = r.PersonalEmail,
        MobileNumber    = r.MobileNumber,
        AlternatePhone  = r.AlternatePhone,
        PresentAddress  = r.PresentAddress,
        PresentCity     = r.PresentCity,
        PresentProvince = r.PresentProvince,
        PresentZipCode  = r.PresentZipCode,
        SameAsPresentAddress = r.SameAsPresentAddress,
        PermanentAddress  = r.PermanentAddress,
        PermanentCity     = r.PermanentCity,
        PermanentProvince = r.PermanentProvince,
        PermanentZipCode  = r.PermanentZipCode,
        DepartmentId    = r.DepartmentId,
        DepartmentName  = r.DepartmentName,
        PositionId      = r.PositionId,
        PositionTitle   = r.PositionTitle,
        ManagerId       = r.ManagerId,
        ManagerName     = r.ManagerName,
        BranchId        = r.BranchId,
        BranchName      = r.BranchName,
        EmploymentType  = EmploymentTypeName(r.EmploymentType),
        HireDate        = r.HireDate,
        TerminationDate = r.TerminationDate,
        ProbationEndDate = r.ProbationEndDate,
        RegularizationDate = r.RegularizationDate,
        LastWorkingDate = r.LastWorkingDate,
        BasicSalary     = r.BasicSalary,
        SalaryFrequency = SalaryFrequencyName(r.SalaryFrequency),
        SalaryEffectiveDate = r.SalaryEffectiveDate,
        BankName        = r.BankName,
        ProfilePhotoPath = r.ProfilePhotoPath,
        BiometricId     = r.BiometricId,
        Status          = StatusName(r.Status),
        StatusRemarks   = r.StatusRemarks,
        StatusChangedAt = r.StatusChangedAt,
        StatusChangedBy = r.StatusChangedBy,
        CreatedBy       = r.CreatedBy,
        CreatedAt       = r.CreatedAt,
        UpdatedBy       = r.UpdatedBy,
        UpdatedAt       = r.UpdatedAt,
        StatusHistory     = statusHistory,
        EmergencyContacts = contacts,
        Documents         = documents,
        SalaryHistory     = salaryHistory,
    };

    private static EmployeeStatusHistoryDto MapToStatusHistoryDto(StatusHistoryRow r) => new()
    {
        Id             = r.Id,
        PreviousStatus = StatusName(r.PreviousStatus),
        NewStatus      = StatusName(r.NewStatus),
        Remarks        = r.Remarks,
        ChangedBy      = r.ChangedBy,
        ChangedAt      = r.ChangedAt,
    };

    private static EmergencyContactDto MapToContactDto(EmergencyContactRow r) => new()
    {
        Id             = r.Id,
        EmployeeId     = r.EmployeeId,
        ContactName    = r.ContactName,
        Relationship   = r.Relationship,
        MobileNumber   = r.MobileNumber,
        AlternatePhone = r.AlternatePhone,
        IsPrimary      = r.IsPrimary,
    };

    private static EmployeeDocumentDto MapToDocumentDto(DocumentRow r) => new()
    {
        Id           = r.Id,
        EmployeeId   = r.EmployeeId,
        DocumentType = DocumentTypeName(r.DocumentType),
        DocumentName = r.DocumentName,
        FilePath     = r.FilePath,
        ExpiryDate   = r.ExpiryDate,
        IsVerified   = r.IsVerified,
        CreatedAt    = r.CreatedAt,
    };

    private static SalaryHistoryDto MapToSalaryHistoryDto(SalaryHistoryRow r) => new()
    {
        Id              = r.Id,
        PreviousSalary  = r.PreviousSalary,
        NewSalary       = r.NewSalary,
        SalaryFrequency = SalaryFrequencyName(r.SalaryFrequency),
        EffectiveDate   = r.EffectiveDate,
        ChangedBy       = r.ChangedBy,
        ChangedAt       = r.ChangedAt,
        Remarks         = r.Remarks,
    };

    // ── Core CRUD ────────────────────────────────────────────────────────

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(
        PaginationParams pagination, int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (items, totalCount) = await _sql.QueryPagedAsync<EmployeeRow>(
                "sp_Employee",
                new
                {
                    ActionType = "GET_PAGED",
                    Page     = pagination.Page,
                    PageSize = pagination.PageSize,
                    Search   = pagination.Search,
                    DepartmentId = departmentId
                },
                cancellationToken);

            var dtos = items.Select(MapToEmployeeDto);
            return PagedResult<EmployeeDto>.Create(dtos, totalCount, pagination.Page, pagination.PageSize);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching paged employees");
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new { ActionType = "GET_BY_ID", Id = id },
                cancellationToken)
                ?? throw new NotFoundException("Employee", id);

            return MapToEmployeeDto(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmployeeDetailDto> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeRow = await _sql.QueryFirstOrDefaultAsync<EmployeeDetailRow>(
                "sp_Employee",
                new { ActionType = "GET_BY_ID", Id = id },
                cancellationToken)
                ?? throw new NotFoundException("Employee", id);

            var statusRows = await _sql.QueryAsync<StatusHistoryRow>(
                "sp_Employee",
                new { ActionType = "GET_STATUS_HISTORY", Id = id },
                cancellationToken);

            var contactRows = await _sql.QueryAsync<EmergencyContactRow>(
                "sp_Employee",
                new { ActionType = "GET_EMERGENCY_CONTACTS", Id = id },
                cancellationToken);

            var documentRows = await _sql.QueryAsync<DocumentRow>(
                "sp_Employee",
                new { ActionType = "GET_DOCUMENTS", Id = id },
                cancellationToken);

            var salaryRows = await _sql.QueryAsync<SalaryHistoryRow>(
                "sp_Employee",
                new { ActionType = "GET_SALARY_HISTORY", Id = id },
                cancellationToken);

            return MapToDetailDto(
                employeeRow,
                statusRows.Select(MapToStatusHistoryDto),
                contactRows.Select(MapToContactDto),
                documentRows.Select(MapToDocumentDto),
                salaryRows.Select(MapToSalaryHistoryDto));
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching employee detail {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmployeeDto> GetByEmployeeCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new { ActionType = "GET_BY_CODE", Code = code },
                cancellationToken)
                ?? throw new NotFoundException("Employee", code);

            return MapToEmployeeDto(row);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching employee by code {Code}", code);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating employee {EmployeeCode}", dto.EmployeeCode);

        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new
                {
                    ActionType = "CREATE",
                    dto.EmployeeCode,
                    dto.FirstName,
                    dto.MiddleName,
                    dto.LastName,
                    dto.Suffix,
                    dto.DateOfBirth,
                    dto.Gender,
                    dto.MaritalStatus,
                    dto.TaxIdentificationNumber,
                    dto.SssNumber,
                    dto.PhilHealthNumber,
                    dto.PagIbigNumber,
                    dto.Email,
                    dto.PersonalEmail,
                    dto.MobileNumber,
                    dto.AlternatePhone,
                    dto.PresentAddress,
                    dto.PresentCity,
                    dto.PresentProvince,
                    dto.PresentZipCode,
                    dto.SameAsPresentAddress,
                    dto.PermanentAddress,
                    dto.PermanentCity,
                    dto.PermanentProvince,
                    dto.PermanentZipCode,
                    dto.DepartmentId,
                    dto.PositionId,
                    dto.ManagerId,
                    dto.BranchId,
                    dto.EmploymentType,
                    dto.HireDate,
                    dto.ProbationEndDate,
                    dto.BasicSalary,
                    dto.SalaryFrequency,
                    dto.SalaryEffectiveDate,
                    dto.BankAccountNumber,
                    dto.BankName,
                    dto.BiometricId,
                    CreatedBy = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create employee.");

            _logger.LogInformation("Employee created: {Code} (Id: {Id})", row.EmployeeCode, row.Id);
            return MapToEmployeeDto(row);
        }
        catch (SqlException ex) when (ex.Message.Contains("already in use") || ex.Message.Contains("already registered"))
        {
            throw new ConflictException(ex.Message);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error creating employee {Code}", dto.EmployeeCode);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmployeeDto> UpdateAsync(
        int id, UpdateEmployeeDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new
                {
                    ActionType = "UPDATE",
                    Id = id,
                    dto.FirstName,
                    dto.MiddleName,
                    dto.LastName,
                    dto.Suffix,
                    dto.Gender,
                    dto.MaritalStatus,
                    dto.DateOfBirth,
                    dto.TaxIdentificationNumber,
                    dto.SssNumber,
                    dto.PhilHealthNumber,
                    dto.PagIbigNumber,
                    dto.Email,
                    dto.PersonalEmail,
                    dto.MobileNumber,
                    dto.AlternatePhone,
                    dto.PresentAddress,
                    dto.PresentCity,
                    dto.PresentProvince,
                    dto.PresentZipCode,
                    dto.SameAsPresentAddress,
                    dto.PermanentAddress,
                    dto.PermanentCity,
                    dto.PermanentProvince,
                    dto.PermanentZipCode,
                    dto.DepartmentId,
                    dto.PositionId,
                    dto.ManagerId,
                    dto.BranchId,
                    dto.EmploymentType,
                    dto.ProbationEndDate,
                    dto.RegularizationDate,
                    dto.BasicSalary,
                    dto.SalaryFrequency,
                    dto.SalaryEffectiveDate,
                    dto.BankAccountNumber,
                    dto.BankName,
                    dto.ProfilePhotoPath,
                    dto.BiometricId,
                    UpdatedBy = updatedBy
                },
                cancellationToken)
                ?? throw new NotFoundException("Employee", id);

            _logger.LogInformation("Employee updated: {Id} by {By}", id, updatedBy);
            return MapToEmployeeDto(row);
        }
        catch (SqlException ex) when (ex.Message.Contains("already registered"))
        {
            throw new ConflictException(ex.Message);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("Employee", id);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error updating employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Employee",
                new { ActionType = "DELETE", Id = id, DeletedBy = deletedBy },
                cancellationToken);

            _logger.LogInformation("Employee soft-deleted: {Id} by {By}", id, deletedBy);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("Employee", id);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error deleting employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    // ── Status Management ────────────────────────────────────────────────

    public async Task<EmployeeStatusHistoryDto> ChangeStatusAsync(
        int id, ChangeEmployeeStatusDto dto, string changedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<StatusHistoryRow>(
                "sp_Employee",
                new
                {
                    ActionType     = "CHANGE_STATUS",
                    Id             = id,
                    NewStatus      = dto.NewStatus,
                    Remarks        = dto.Remarks,
                    LastWorkingDate = dto.LastWorkingDate,
                    ChangedBy      = changedBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to change employee status.");

            _logger.LogInformation("Employee {Id} status changed to {Status} by {By}",
                id, StatusName(row.NewStatus), changedBy);

            return MapToStatusHistoryDto(row);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("Employee", id);
        }
        catch (SqlException ex) when (ex.Message.Contains("not allowed"))
        {
            throw new AppException(ex.Message);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error changing status for employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task<IEnumerable<EmployeeStatusHistoryDto>> GetStatusHistoryAsync(
        int id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify employee exists
            _ = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new { ActionType = "GET_BY_ID", Id = id },
                cancellationToken)
                ?? throw new NotFoundException("Employee", id);

            var rows = await _sql.QueryAsync<StatusHistoryRow>(
                "sp_Employee",
                new { ActionType = "GET_STATUS_HISTORY", Id = id },
                cancellationToken);

            return rows.Select(MapToStatusHistoryDto);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching status history for employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    // ── Emergency Contacts ───────────────────────────────────────────────

    public async Task<IEnumerable<EmergencyContactDto>> GetEmergencyContactsAsync(
        int id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify employee exists
            _ = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new { ActionType = "GET_BY_ID", Id = id },
                cancellationToken)
                ?? throw new NotFoundException("Employee", id);

            var rows = await _sql.QueryAsync<EmergencyContactRow>(
                "sp_Employee",
                new { ActionType = "GET_EMERGENCY_CONTACTS", Id = id },
                cancellationToken);

            return rows.Select(MapToContactDto);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching emergency contacts for employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmergencyContactDto> AddEmergencyContactAsync(
        int id, CreateEmergencyContactDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<EmergencyContactRow>(
                "sp_Employee",
                new
                {
                    ActionType = "CREATE_EMERGENCY_CONTACT",
                    Id = id,
                    dto.ContactName,
                    dto.Relationship,
                    Phone = dto.MobileNumber,
                    dto.AlternatePhone,
                    dto.IsPrimary,
                    CreatedBy = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create emergency contact.");

            return MapToContactDto(row);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("Employee", id);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error adding emergency contact for employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmergencyContactDto> UpdateEmergencyContactAsync(
        int employeeId, int contactId, UpdateEmergencyContactDto dto, string updatedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<EmergencyContactRow>(
                "sp_Employee",
                new
                {
                    ActionType = "UPDATE_EMERGENCY_CONTACT",
                    Id         = employeeId,
                    ContactId  = contactId,
                    dto.ContactName,
                    dto.Relationship,
                    Phone = dto.MobileNumber,
                    dto.AlternatePhone,
                    dto.IsPrimary,
                    UpdatedBy = updatedBy
                },
                cancellationToken)
                ?? throw new NotFoundException("EmergencyContact", contactId);

            return MapToContactDto(row);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("EmergencyContact", contactId);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error updating emergency contact {ContactId} for employee {EmpId}", contactId, employeeId);
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteEmergencyContactAsync(
        int employeeId, int contactId, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Employee",
                new { ActionType = "DELETE_EMERGENCY_CONTACT", Id = employeeId, ContactId = contactId, DeletedBy = deletedBy },
                cancellationToken);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("EmergencyContact", contactId);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error deleting emergency contact {ContactId} for employee {EmpId}", contactId, employeeId);
            throw new AppException(ex.Message);
        }
    }

    // ── Documents ────────────────────────────────────────────────────────

    public async Task<IEnumerable<EmployeeDocumentDto>> GetDocumentsAsync(
        int id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify employee exists
            _ = await _sql.QueryFirstOrDefaultAsync<EmployeeRow>(
                "sp_Employee",
                new { ActionType = "GET_BY_ID", Id = id },
                cancellationToken)
                ?? throw new NotFoundException("Employee", id);

            var rows = await _sql.QueryAsync<DocumentRow>(
                "sp_Employee",
                new { ActionType = "GET_DOCUMENTS", Id = id },
                cancellationToken);

            return rows.Select(MapToDocumentDto);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error fetching documents for employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task<EmployeeDocumentDto> AddDocumentAsync(
        int id, UploadDocumentDto dto, string createdBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var row = await _sql.QueryFirstOrDefaultAsync<DocumentRow>(
                "sp_Employee",
                new
                {
                    ActionType = "CREATE_DOCUMENT",
                    Id = id,
                    dto.DocumentType,
                    dto.DocumentName,
                    dto.FilePath,
                    dto.ExpiryDate,
                    CreatedBy = createdBy
                },
                cancellationToken)
                ?? throw new AppException("Failed to create document.");

            return MapToDocumentDto(row);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("Employee", id);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error adding document for employee {Id}", id);
            throw new AppException(ex.Message);
        }
    }

    public async Task DeleteDocumentAsync(
        int employeeId, int documentId, string deletedBy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sql.ExecuteAsync(
                "sp_Employee",
                new { ActionType = "DELETE_DOCUMENT", Id = employeeId, DocumentId = documentId, DeletedBy = deletedBy },
                cancellationToken);
        }
        catch (SqlException ex) when (ex.Message.Contains("not found"))
        {
            throw new NotFoundException("EmployeeDocument", documentId);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Error deleting document {DocId} for employee {EmpId}", documentId, employeeId);
            throw new AppException(ex.Message);
        }
    }
}
