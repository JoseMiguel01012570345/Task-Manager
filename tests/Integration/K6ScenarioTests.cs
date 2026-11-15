using System.Diagnostics;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class K6ScenarioTests(ClusterApi cluster)
{
    [Theory]
    [InlineData("smoke.js", false)]
    [InlineData("load.js", false)]
    [InlineData("stress.js", false)]
    [InlineData("spike.js", false)]
    [InlineData("soak.js", false)]
    [InlineData("mixed.js", true)]
    [InlineData("rate-limit.js", false)]
    public async Task Scenario_passes(string script, bool authenticated)
    {
        var file = Path.Combine(FindRepoRoot(), "tests", "k6", script);
        Assert.True(File.Exists(file), file);

        var start = new ProcessStartInfo
        {
            FileName = ResolveK6(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        start.ArgumentList.Add("run");
        start.ArgumentList.Add(file);
        start.Environment["BASE_URL"] = cluster.BaseUri.GetLeftPart(UriPartial.Authority);
        start.Environment["SUITE"] = "1";
        if (authenticated)
        {
            start.Environment["TOKEN"] = await cluster.IssueUserAccessTokenAsync();
        }

        using var process = Process.Start(start);
        Assert.NotNull(process);
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        Assert.True(
            process.ExitCode == 0,
            $"k6 {script} terminó con {process.ExitCode}.{Environment.NewLine}{Tail(stdout)}{Environment.NewLine}{Tail(stderr)}");
    }

    private static string ResolveK6()
    {
        var path = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? [];
        foreach (var directory in path)
        {
            foreach (var name in new[] { "k6.exe", "k6" })
            {
                var candidate = Path.Combine(directory.Trim(), name);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException("k6 no está en PATH.");
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "TaskManagement.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio.");
    }

    private static string Tail(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var lines = text.Split('\n');
        return string.Join('\n', lines.TakeLast(40));
    }
}
