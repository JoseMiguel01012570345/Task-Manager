using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace TaskManagement.API.Auth;

public sealed class KeycloakRoles : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var realmAccess = principal.FindFirst("realm_access")?.Value;
        if (string.IsNullOrWhiteSpace(realmAccess))
        {
            return Task.FromResult(principal);
        }

        using var document = JsonDocument.Parse(realmAccess);
        if (!document.RootElement.TryGetProperty("roles", out var roles) || roles.ValueKind != JsonValueKind.Array)
        {
            return Task.FromResult(principal);
        }

        var identity = new ClaimsIdentity();
        foreach (var role in roles.EnumerateArray())
        {
            var name = role.GetString();
            if (!string.IsNullOrWhiteSpace(name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, name));
            }
        }

        principal.AddIdentity(identity);
        return Task.FromResult(principal);
    }
}
