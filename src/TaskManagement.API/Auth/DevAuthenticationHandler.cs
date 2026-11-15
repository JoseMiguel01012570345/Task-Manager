using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace TaskManagement.API.Auth;

public sealed class DevAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Dev";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = configuration["DevUser:UserId"] ?? "dev-user";
        var administrator = bool.TryParse(configuration["DevUser:IsAdministrator"], out var isAdministrator) && isAdministrator;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("preferred_username", userId),
            new(ClaimTypes.Role, administrator ? "Administrator" : "User"),
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
