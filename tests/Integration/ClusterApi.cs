using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskManagement.IntegrationTests;

public sealed class ClusterApi : IAsyncLifetime
{
    public const string BaseUrlVariable = "TASKMANAGEMENT_BASE_URL";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public Uri BaseUri { get; }

    public HttpClient Anonymous { get; private set; } = null!;

    public HttpClient User { get; private set; } = null!;

    public HttpClient Admin { get; private set; } = null!;

    public ClusterApi()
    {
        var configured = Environment.GetEnvironmentVariable(BaseUrlVariable);
        var text = string.IsNullOrWhiteSpace(configured) ? "http://localhost:8088" : configured.Trim();
        BaseUri = new Uri(text.TrimEnd('/') + "/");
    }

    public async Task InitializeAsync()
    {
        Anonymous = CreateClient();
        try
        {
            var ready = await Anonymous.GetAsync("health/ready");
            if (!ready.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"k3d respondió {(int)ready.StatusCode} en {BaseUri}health/ready. Ejecuta Deploy-Cluster.ps1.");
            }
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                $"No hay un clúster k3d en {BaseUri}. Ejecuta Deploy-Cluster.ps1.",
                exception);
        }

        User = await AuthenticatedAsync("dev-user", Password("DEV_USER_PASSWORD", "lab-dev-user"));
        Admin = await AuthenticatedAsync("admin-user", Password("ADMIN_USER_PASSWORD", "lab-admin-user"));
    }

    public async Task<string> IssueUserAccessTokenAsync()
    {
        using var client = await AuthenticatedAsync("dev-user", Password("DEV_USER_PASSWORD", "lab-dev-user"));
        return client.DefaultRequestHeaders.Authorization?.Parameter
            ?? throw new InvalidOperationException("Keycloak no devolvió un access token para dev-user.");
    }

    public Task DisposeAsync()
    {
        User.Dispose();
        Admin.Dispose();
        Anonymous.Dispose();
        return Task.CompletedTask;
    }

    public async Task<TaskBody> CreateTaskAsync(HttpClient client, string title, string assigneeId)
    {
        var response = await client.PostAsJsonAsync("api/v1/tasks", new
        {
            title,
            description = "k3d",
            assigneeId
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskBody>(Json);
        Assert.NotNull(body);
        return body;
    }

    public async Task DeleteTaskAsync(HttpClient client, Guid id, uint version)
    {
        var response = await client.DeleteAsync($"api/v1/tasks/{id}?version={version}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<HttpClient> AuthenticatedAsync(string username, string password)
    {
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "taskmanagement-web",
            ["username"] = username,
            ["password"] = password
        });
        var response = await Anonymous.PostAsync("realms/taskmanagement/protocol/openid-connect/token", content);
        var payload = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Keycloak rechazó a {username} en {BaseUri}: {(int)response.StatusCode} {payload}");
        }

        using var document = JsonDocument.Parse(payload);
        var accessToken = document.RootElement.GetProperty("access_token").GetString();
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private HttpClient CreateClient()
    {
        return new HttpClient { BaseAddress = BaseUri, Timeout = TimeSpan.FromSeconds(30) };
    }

    private static string Password(string variable, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(variable);
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    public sealed record TaskBody(Guid Id, string Title, uint Version);
}
