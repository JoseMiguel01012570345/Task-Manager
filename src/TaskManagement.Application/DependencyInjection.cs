using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Tasks;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();
        services.AddScoped<CreateTask.Handler>();
        return services;
    }
}
