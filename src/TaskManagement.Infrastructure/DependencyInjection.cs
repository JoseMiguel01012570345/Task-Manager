using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Tasks;
using TaskManagement.Infrastructure.Auditing;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Time;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "TaskManagement";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool seedDevelopmentData = false)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = AppDbContextFactory.DefaultConnectionString;
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IAuditLog, EfAuditLog>();

        if (seedDevelopmentData)
        {
            services.AddHostedService<DevelopmentSeedHostedService>();
        }

        return services;
    }
}
