using AutoMapper;
using PayrollApi.Application.DTOs.Employee;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Application.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        // ── Employee → EmployeeDto (list) ─────────────────────────────────────
        CreateMap<Employee, EmployeeDto>()
            .ForMember(d => d.Gender,          o => o.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.MaritalStatus,   o => o.MapFrom(s => s.MaritalStatus.ToString()))
            .ForMember(d => d.Status,          o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.EmploymentType,  o => o.MapFrom(s => s.EmploymentType.ToString()))
            .ForMember(d => d.SalaryFrequency, o => o.MapFrom(s => s.SalaryFrequency.ToString()))
            .ForMember(d => d.DepartmentName,  o => o.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.PositionTitle,   o => o.MapFrom(s => s.Position   != null ? s.Position.PositionTitle   : null))
            .ForMember(d => d.ManagerName,     o => o.MapFrom(s => s.Manager    != null ? $"{s.Manager.FirstName} {s.Manager.LastName}" : null))
            .ForMember(d => d.BranchName,      o => o.MapFrom(s => s.Branch     != null ? s.Branch.BranchName        : null));

        // ── Employee → EmployeeDetailDto (full) ──────────────────────────────
        CreateMap<Employee, EmployeeDetailDto>()
            .ForMember(d => d.Gender,          o => o.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.MaritalStatus,   o => o.MapFrom(s => s.MaritalStatus.ToString()))
            .ForMember(d => d.Status,          o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.EmploymentType,  o => o.MapFrom(s => s.EmploymentType.ToString()))
            .ForMember(d => d.SalaryFrequency, o => o.MapFrom(s => s.SalaryFrequency.ToString()))
            .ForMember(d => d.DepartmentName,  o => o.MapFrom(s => s.Department != null ? s.Department.DepartmentName : null))
            .ForMember(d => d.PositionTitle,   o => o.MapFrom(s => s.Position   != null ? s.Position.PositionTitle   : null))
            .ForMember(d => d.ManagerName,     o => o.MapFrom(s => s.Manager    != null ? $"{s.Manager.FirstName} {s.Manager.LastName}" : null))
            .ForMember(d => d.BranchName,      o => o.MapFrom(s => s.Branch     != null ? s.Branch.BranchName        : null))
            .ForMember(d => d.StatusHistory,   o => o.MapFrom(s => s.StatusHistory))
            .ForMember(d => d.EmergencyContacts, o => o.MapFrom(s => s.EmergencyContacts))
            .ForMember(d => d.Documents,       o => o.MapFrom(s => s.Documents));

        // ── CreateEmployeeDto → Employee ─────────────────────────────────────
        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(d => d.Gender,               o => o.MapFrom(s => (Gender)s.Gender))
            .ForMember(d => d.MaritalStatus,        o => o.MapFrom(s => (MaritalStatus)s.MaritalStatus))
            .ForMember(d => d.EmploymentType,       o => o.MapFrom(s => (EmploymentType)s.EmploymentType))
            .ForMember(d => d.SalaryFrequency,      o => o.MapFrom(s => (SalaryFrequency)s.SalaryFrequency))
            .ForMember(d => d.Status,               o => o.MapFrom(_ => EmploymentStatus.Active))
            .ForMember(d => d.Id,                   o => o.Ignore())
            .ForMember(d => d.IsDeleted,            o => o.Ignore())
            .ForMember(d => d.CreatedAt,            o => o.Ignore())
            .ForMember(d => d.CreatedBy,            o => o.Ignore())
            .ForMember(d => d.UpdatedAt,            o => o.Ignore())
            .ForMember(d => d.UpdatedBy,            o => o.Ignore())
            .ForMember(d => d.DeletedAt,            o => o.Ignore())
            .ForMember(d => d.DeletedBy,            o => o.Ignore())
            .ForMember(d => d.StatusChangedAt,      o => o.Ignore())
            .ForMember(d => d.StatusChangedBy,      o => o.Ignore())
            .ForMember(d => d.StatusRemarks,        o => o.Ignore())
            .ForMember(d => d.TerminationDate,      o => o.Ignore())
            .ForMember(d => d.RegularizationDate,   o => o.Ignore())
            .ForMember(d => d.LastWorkingDate,      o => o.Ignore())
            .ForMember(d => d.ProfilePhotoPath,     o => o.Ignore())
            .ForMember(d => d.BiometricId,          o => o.Ignore())
            .ForMember(d => d.Department,           o => o.Ignore())
            .ForMember(d => d.Position,             o => o.Ignore())
            .ForMember(d => d.Manager,              o => o.Ignore())
            .ForMember(d => d.Branch,               o => o.Ignore())
            .ForMember(d => d.PayrollRecords,       o => o.Ignore())
            .ForMember(d => d.StatusHistory,        o => o.Ignore())
            .ForMember(d => d.EmergencyContacts,    o => o.Ignore())
            .ForMember(d => d.Documents,            o => o.Ignore());

        // ── UpdateEmployeeDto → Employee ─────────────────────────────────────
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(d => d.Gender,               o => o.MapFrom(s => (Gender)s.Gender))
            .ForMember(d => d.MaritalStatus,        o => o.MapFrom(s => (MaritalStatus)s.MaritalStatus))
            .ForMember(d => d.EmploymentType,       o => o.MapFrom(s => (EmploymentType)s.EmploymentType))
            .ForMember(d => d.SalaryFrequency,      o => o.MapFrom(s => (SalaryFrequency)s.SalaryFrequency))
            .ForMember(d => d.Id,                   o => o.Ignore())
            .ForMember(d => d.EmployeeCode,         o => o.Ignore())
            .ForMember(d => d.HireDate,             o => o.Ignore())
            .ForMember(d => d.TerminationDate,      o => o.Ignore())
            .ForMember(d => d.LastWorkingDate,      o => o.Ignore())
            .ForMember(d => d.Status,               o => o.Ignore())
            .ForMember(d => d.StatusRemarks,        o => o.Ignore())
            .ForMember(d => d.StatusChangedAt,      o => o.Ignore())
            .ForMember(d => d.StatusChangedBy,      o => o.Ignore())
            .ForMember(d => d.IsDeleted,            o => o.Ignore())
            .ForMember(d => d.CreatedAt,            o => o.Ignore())
            .ForMember(d => d.CreatedBy,            o => o.Ignore())
            .ForMember(d => d.UpdatedAt,            o => o.Ignore())
            .ForMember(d => d.UpdatedBy,            o => o.Ignore())
            .ForMember(d => d.DeletedAt,            o => o.Ignore())
            .ForMember(d => d.DeletedBy,            o => o.Ignore())
            .ForMember(d => d.Department,           o => o.Ignore())
            .ForMember(d => d.Position,             o => o.Ignore())
            .ForMember(d => d.Manager,              o => o.Ignore())
            .ForMember(d => d.Branch,               o => o.Ignore())
            .ForMember(d => d.PayrollRecords,       o => o.Ignore())
            .ForMember(d => d.StatusHistory,        o => o.Ignore())
            .ForMember(d => d.EmergencyContacts,    o => o.Ignore())
            .ForMember(d => d.Documents,            o => o.Ignore())
            .ForMember(d => d.DateOfBirth,          o => o.Ignore())
            .ForMember(d => d.TaxIdentificationNumber, o => o.MapFrom(s => s.TaxIdentificationNumber))
            .ForMember(d => d.SssNumber,            o => o.MapFrom(s => s.SssNumber))
            .ForMember(d => d.PhilHealthNumber,     o => o.MapFrom(s => s.PhilHealthNumber))
            .ForMember(d => d.PagIbigNumber,        o => o.MapFrom(s => s.PagIbigNumber));

        // ── Related entities ─────────────────────────────────────────────────
        CreateMap<EmployeeStatusHistory, EmployeeStatusHistoryDto>()
            .ForMember(d => d.PreviousStatus, o => o.MapFrom(s => s.PreviousStatus.ToString()))
            .ForMember(d => d.NewStatus,      o => o.MapFrom(s => s.NewStatus.ToString()));

        CreateMap<EmployeeEmergencyContact, EmergencyContactDto>();
        CreateMap<CreateEmergencyContactDto, EmployeeEmergencyContact>()
            .ForMember(d => d.Id,         o => o.Ignore())
            .ForMember(d => d.EmployeeId, o => o.Ignore())
            .ForMember(d => d.Employee,   o => o.Ignore())
            .ForMember(d => d.IsDeleted,  o => o.Ignore())
            .ForMember(d => d.CreatedAt,  o => o.Ignore())
            .ForMember(d => d.CreatedBy,  o => o.Ignore())
            .ForMember(d => d.UpdatedAt,  o => o.Ignore())
            .ForMember(d => d.UpdatedBy,  o => o.Ignore())
            .ForMember(d => d.DeletedAt,  o => o.Ignore())
            .ForMember(d => d.DeletedBy,  o => o.Ignore());
        CreateMap<UpdateEmergencyContactDto, EmployeeEmergencyContact>()
            .ForMember(d => d.Id,         o => o.Ignore())
            .ForMember(d => d.EmployeeId, o => o.Ignore())
            .ForMember(d => d.Employee,   o => o.Ignore())
            .ForMember(d => d.IsDeleted,  o => o.Ignore())
            .ForMember(d => d.CreatedAt,  o => o.Ignore())
            .ForMember(d => d.CreatedBy,  o => o.Ignore())
            .ForMember(d => d.UpdatedAt,  o => o.Ignore())
            .ForMember(d => d.UpdatedBy,  o => o.Ignore())
            .ForMember(d => d.DeletedAt,  o => o.Ignore())
            .ForMember(d => d.DeletedBy,  o => o.Ignore());

        CreateMap<EmployeeDocument, EmployeeDocumentDto>()
            .ForMember(d => d.DocumentType, o => o.MapFrom(s => s.DocumentType.ToString()));
        CreateMap<UploadDocumentDto, EmployeeDocument>()
            .ForMember(d => d.DocumentType, o => o.MapFrom(s => (DocumentType)s.DocumentType))
            .ForMember(d => d.Id,           o => o.Ignore())
            .ForMember(d => d.EmployeeId,   o => o.Ignore())
            .ForMember(d => d.Employee,     o => o.Ignore())
            .ForMember(d => d.IsVerified,   o => o.Ignore())
            .ForMember(d => d.IsDeleted,    o => o.Ignore())
            .ForMember(d => d.CreatedAt,    o => o.Ignore())
            .ForMember(d => d.CreatedBy,    o => o.Ignore())
            .ForMember(d => d.UpdatedAt,    o => o.Ignore())
            .ForMember(d => d.UpdatedBy,    o => o.Ignore())
            .ForMember(d => d.DeletedAt,    o => o.Ignore())
            .ForMember(d => d.DeletedBy,    o => o.Ignore());
    }
}
