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
        while (limited is null && sent < 900 && started.Elapsed < TimeSpan.FromSeconds(9))
        {
            var batch = Math.Min(8, 900 - sent);
            var responses = await Task.WhenAll(
                Enumerable.Range(0, batch).Select(_ => cluster.Admin.GetAsync("api/v1/tasks")));
            sent += responses.Length;
            foreach (var response in responses)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    limited = response;
                    continue;
                }

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                response.Dispose();
            }
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
