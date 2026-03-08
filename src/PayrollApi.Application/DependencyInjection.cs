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
        services.AddScoped<IGlobalConfigService, GlobalConfigService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IPayPeriodService, PayPeriodService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAllowanceTypeService, AllowanceTypeService>();
        services.AddScoped<ILeaveTypeService, LeaveTypeService>();
        services.AddScoped<IWorkScheduleService, WorkScheduleService>();
        services.AddScoped<IScheduleRuleService, ScheduleRuleService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IDeductionTypeService, DeductionTypeService>();
        services.AddScoped<IPayrollSummaryReportService, PayrollSummaryReportService>();
        services.AddScoped<IGovernmentReportService, GovernmentReportService>();
        services.AddScoped<IBankDisbursementService, BankDisbursementService>();
        services.AddScoped<IAttendanceReportService, AttendanceReportService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IBranchService, BranchService>();

        return services;
    }
}
