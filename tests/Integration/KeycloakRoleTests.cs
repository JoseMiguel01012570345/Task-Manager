using System.Security.Claims;
using TaskManagement.API.Auth;

namespace TaskManagement.IntegrationTests;

public class KeycloakRoleTests
{
    [Fact]
    public async Task Realm_roles_become_role_claims()
    {
        var identity = new ClaimsIdentity(
            [new Claim("realm_access", """{"roles":["Administrator","User"]}""")],
            authenticationType: "test");
        var principal = new ClaimsPrincipal(identity);

        var transformed = await new KeycloakRoles().TransformAsync(principal);

        Assert.True(transformed.IsInRole("Administrator"));
        Assert.True(transformed.IsInRole("User"));
    }
}
