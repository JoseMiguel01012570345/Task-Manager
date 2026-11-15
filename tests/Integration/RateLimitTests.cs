using System.Diagnostics;
using System.Net;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class RateLimitTests(ClusterApi cluster)
{
    [Fact]
    public async Task Excess_requests_return_429_and_retry_after()
    {
        var started = Stopwatch.StartNew();
        HttpResponseMessage? limited = null;
        var sent = 0;
        while (sent < 600 && started.Elapsed < TimeSpan.FromSeconds(9))
        {
            var response = await cluster.Admin.GetAsync("api/v1/tasks");
            sent++;
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                limited = response;
                break;
            }

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        Assert.True(limited is not null, $"Ninguna de las {sent} lecturas recibió 429. Cada réplica cuenta su propio cupo.");
        var retryAfter = limited.Headers.RetryAfter?.Delta?.Seconds.ToString()
            ?? limited.Headers.GetValues("Retry-After").Single();
        Assert.Equal("10", retryAfter);

        await Task.Delay(TimeSpan.FromSeconds(11));
        var again = await cluster.Admin.GetAsync("api/v1/tasks");
        Assert.Equal(HttpStatusCode.OK, again.StatusCode);
    }
}
