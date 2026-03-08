using AutoMapper;
using PayrollApi.Application.DTOs.PayPeriod;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Application.Mappings;

public class PayPeriodMappingProfile : Profile
{
    public PayPeriodMappingProfile()
    {
        CreateMap<PayrollPeriod, PayPeriodDto>()
            .ForMember(d => d.PeriodType,   o => o.MapFrom(s => s.PeriodType.ToString()))
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString().ToLower()))
            .ForMember(d => d.EmployeeCount, o => o.MapFrom(s => s.PayrollRecords != null ? s.PayrollRecords.Count : 0));
    }
}
