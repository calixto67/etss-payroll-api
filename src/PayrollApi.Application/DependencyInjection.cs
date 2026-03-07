using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PayrollApi.Application.Services;
using PayrollApi.Application.Services.Interfaces;
using System.Reflection;

namespace PayrollApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICompanySettingsService, CompanySettingsService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<ILeaveService, LeaveService>();

        return services;
    }
}
