using Microsoft.Extensions.Configuration;
using TaskManagement.Application.Abstractions;

namespace TaskManagement.Infrastructure.Identity;

internal sealed class DevCurrentUser(IConfiguration configuration) : ICurrentUser
{
    public bool IsAuthenticated { get; } = true;

    public string UserId { get; } = configuration["DevUser:UserId"] ?? "dev-user";

    public bool IsAdministrator { get; } =
        bool.TryParse(configuration["DevUser:IsAdministrator"], out var isAdministrator) && isAdministrator;
}
