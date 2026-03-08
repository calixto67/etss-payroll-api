using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollApi.Domain.Interfaces;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;
using PayrollApi.Infrastructure.Repositories;

namespace PayrollApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            });
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeStatusHistoryRepository, EmployeeStatusHistoryRepository>();
        services.AddScoped<IEmergencyContactRepository, EmergencyContactRepository>();
        services.AddScoped<IEmployeeDocumentRepository, EmployeeDocumentRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IPayPeriodRepository, PayPeriodRepository>();
        services.AddScoped<ILeaveRepository, LeaveRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICompanySettingsRepository, CompanySettingsRepository>();
        services.AddScoped<IGlobalConfigRepository, GlobalConfigRepository>();
        services.AddScoped<IAllowanceTypeRepository, AllowanceTypeRepository>();
        services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
        services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
        services.AddScoped<IScheduleRuleRepository, ScheduleRuleRepository>();
        services.AddScoped<IEmployeeScheduleRepository, EmployeeScheduleRepository>();
        services.AddScoped<ISalaryHistoryRepository, SalaryHistoryRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IDeductionTypeRepository, DeductionTypeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISqlExecutor, SqlExecutor>();

        return services;
    }
}
