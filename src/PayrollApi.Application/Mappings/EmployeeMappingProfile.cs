using AutoMapper;
using PayrollApi.Application.DTOs.Employee;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Application.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => src.EmploymentType.ToString()))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : null))
            .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.PositionTitle : null));

        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => EmploymentStatus.Active))
            .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => (EmploymentType)src.EmploymentType))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EmploymentStatus)src.Status))
            .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => (EmploymentType)src.EmploymentType))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeCode, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
