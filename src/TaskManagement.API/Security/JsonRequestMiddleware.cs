namespace TaskManagement.API.Security;

public static class JsonRequestMiddleware
{
    public const long MaxBodyBytes = 64 * 1024;

    public static IApplicationBuilder UseJsonRequests(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (HttpMethods.IsPost(context.Request.Method)
                || HttpMethods.IsPut(context.Request.Method)
                || HttpMethods.IsPatch(context.Request.Method))
            {
                var contentType = context.Request.ContentType ?? string.Empty;
                if (!contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                    return;
                }
            }

            await next(context).ConfigureAwait(false);
        });
    }
}
