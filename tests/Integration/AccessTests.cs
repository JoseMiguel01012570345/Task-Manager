using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Domain.Tasks;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class AccessTests(ApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Missing_token_uses_the_development_user()
    {
        var response = await _client.GetAsync("/api/v1/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_bearer_token_is_rejected()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/tasks");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-jwt");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Bearer_token_with_a_past_expiry_is_rejected()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/tasks");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ExpiredToken());

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Another_users_task_is_not_visible()
    {
        using var scope = factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var task = TaskItem.Create("Ajena", null, "other-user", "other-user", null, DateTimeOffset.UtcNow);
        database.Tasks.Add(task);
        await database.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/v1/tasks/{task.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static string ExpiredToken()
    {
        var header = Base64Url("{\"alg\":\"none\",\"typ\":\"JWT\"}");
        var payload = Base64Url("{\"exp\":1,\"aud\":\"taskmanagement-api\"}");
        return $"{header}.{payload}.";
    }

    private static string Base64Url(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
