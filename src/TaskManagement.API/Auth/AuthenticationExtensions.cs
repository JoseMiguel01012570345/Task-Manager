using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TaskManagement.API.Auth;

public static class AuthenticationExtensions
{
    public const string SmartScheme = "Smart";

    public static IServiceCollection AddTaskAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<Application.Abstractions.ICurrentUser, HttpCurrentUser>();
        services.AddTransient<IClaimsTransformation, KeycloakRoles>();

        var authority = configuration["Authentication:Authority"]
            ?? "http://localhost:8080/realms/taskmanagement";
        var audience = configuration["Authentication:Audience"] ?? "taskmanagement-api";

        var auth = services.AddAuthentication(SmartScheme)
            .AddPolicyScheme(SmartScheme, SmartScheme, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var header = context.Request.Headers.Authorization.ToString();
                    if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    return environment.IsDevelopment()
                        ? DevAuthenticationHandler.SchemeName
                        : JwtBearerDefaults.AuthenticationScheme;
                };
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = bool.TryParse(configuration["Authentication:RequireHttpsMetadata"], out var requireHttps) && requireHttps;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidAudience = audience;
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "role";
            });

        if (environment.IsDevelopment())
        {
            auth.AddScheme<AuthenticationSchemeOptions, DevAuthenticationHandler>(DevAuthenticationHandler.SchemeName, _ => { });
        }

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(SmartScheme)
                .RequireAuthenticatedUser()
                .Build();
            options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("TaskAccess", policy => policy.RequireAuthenticatedUser());
        });

        return services;
    }
}
