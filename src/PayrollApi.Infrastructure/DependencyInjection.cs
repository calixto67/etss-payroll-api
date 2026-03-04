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

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
