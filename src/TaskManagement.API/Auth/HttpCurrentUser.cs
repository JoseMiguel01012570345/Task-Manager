using System.Security.Claims;
using TaskManagement.Application.Abstractions;

namespace TaskManagement.API.Auth;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public string UserId =>
        Principal?.FindFirst("preferred_username")?.Value
        ?? Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? "anonymous";

    public bool IsAdministrator => Principal?.IsInRole("Administrator") == true;
}
