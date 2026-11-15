namespace TaskManagement.API.Security;

public static class LocalCors
{
    public const string PolicyName = "LocalWeb";
    public const string Origin = "http://localhost:5173";

    public static IServiceCollection AddLocalCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins(Origin)
                    .WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Patch, HttpMethods.Delete)
                    .WithHeaders("Authorization", "Content-Type", "Accept")
                    .DisallowCredentials();
            });
        });

        return services;
    }
}
