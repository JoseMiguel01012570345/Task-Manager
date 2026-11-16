using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class RateLimitTests(ClusterApi cluster)
{
    [Fact]
    public async Task Excess_requests_return_429_and_retry_after()
    {
        var pod = Kubectl(
            "get", "pods", "-n", "taskmanagement", "-l", "app=api",
            "--field-selector=status.phase=Running",
            "-o", "jsonpath={.items[0].metadata.name}");
        Assert.False(string.IsNullOrWhiteSpace(pod), "No hay un pod de la API para medir su cupo.");

        var port = FreePort();
        var forwardStart = new ProcessStartInfo
        {
            FileName = "kubectl",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        forwardStart.ArgumentList.Add("port-forward");
        forwardStart.ArgumentList.Add("-n");
        forwardStart.ArgumentList.Add("taskmanagement");
        forwardStart.ArgumentList.Add($"pod/{pod}");
        forwardStart.ArgumentList.Add($"{port}:8080");
        using var forward = Process.Start(forwardStart) ?? throw new InvalidOperationException("kubectl port-forward no arrancó.");
        forward.BeginOutputReadLine();
        forward.BeginErrorReadLine();

        using var client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}/"), Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.Authorization = cluster.Admin.DefaultRequestHeaders.Authorization;
        try
        {
            await WaitUntilForwardedAsync(client);

            for (var warm = 0; warm < 3; warm++)
            {
                using var primed = await ReadAsync(client);
            }

            var started = Stopwatch.StartNew();
            HttpResponseMessage? limited = null;
            var sent = 0;
            while (limited is null && sent < 800 && started.Elapsed < TimeSpan.FromSeconds(12))
            {
                var responses = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => ReadAsync(client)));
                foreach (var response in responses)
                {
                    if (response is null)
                    {
                        continue;
                    }

                    sent++;
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        limited = response;
                        continue;
                    }

                    if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout)
                    {
                        response.Dispose();
                        continue;
                    }

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }

            Assert.True(limited is not null, $"Ninguna de las {sent} lecturas recibió 429. Cada réplica cuenta su propio cupo.");
            var retryAfter = limited.Headers.RetryAfter?.Delta?.Seconds.ToString()
                ?? limited.Headers.GetValues("Retry-After").Single();
            Assert.Equal("10", retryAfter);

            await Task.Delay(TimeSpan.FromSeconds(11));
            var again = await ReadAsync(client);
            Assert.NotNull(again);
            Assert.Equal(HttpStatusCode.OK, again.StatusCode);
        }
        finally
        {
            if (!forward.HasExited)
            {
                forward.Kill(entireProcessTree: true);
            }
        }
    }

    private static async Task WaitUntilForwardedAsync(HttpClient client)
    {
        var deadline = DateTime.UtcNow.AddSeconds(20);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var health = await client.GetAsync("health/live");
                if (health.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
            }

            await Task.Delay(TimeSpan.FromMilliseconds(300));
        }

        throw new InvalidOperationException("El port-forward hacia la API no quedó listo.");
    }

    private static async Task<HttpResponseMessage?> ReadAsync(HttpClient client)
    {
        using var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(8));
        try
        {
            return await client.GetAsync("api/v1/tasks", cancel.Token);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return null;
        }
    }

    private static int FreePort()
    {
        using var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string Kubectl(params string[] arguments)
    {
        var start = new ProcessStartInfo
        {
            FileName = "kubectl",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        using var process = Process.Start(start) ?? throw new InvalidOperationException("kubectl no está en PATH.");

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"kubectl terminó con {process.ExitCode}.");
        }

        return output.Trim();
    }
}
