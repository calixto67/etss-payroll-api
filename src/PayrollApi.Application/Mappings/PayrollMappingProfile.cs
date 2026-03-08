using AutoMapper;
using PayrollApi.Application.DTOs.Payroll;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Application.Mappings;

public class PayrollMappingProfile : Profile
{
    public PayrollMappingProfile()
    {
        CreateMap<PayrollRecord, PayrollRecordDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : string.Empty))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src =>
                src.Employee != null ? src.Employee.EmployeeCode : string.Empty))
            .ForMember(dest => dest.PeriodCode, opt => opt.MapFrom(src =>
                src.PayrollPeriod != null ? src.PayrollPeriod.PeriodCode : string.Empty))
            .ForMember(dest => dest.PeriodName, opt => opt.MapFrom(src =>
                src.PayrollPeriod != null ? src.PayrollPeriod.Name : string.Empty));
    }
}
