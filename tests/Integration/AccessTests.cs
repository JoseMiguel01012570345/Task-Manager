using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class AccessTests(ClusterApi cluster)
{
    [Fact]
    public async Task Missing_token_is_rejected()
    {
        var response = await cluster.Anonymous.GetAsync("api/v1/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Invalid_bearer_token_is_rejected()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/tasks");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-jwt");

        var response = await cluster.Anonymous.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Bearer_token_with_a_past_expiry_is_rejected()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/tasks");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ExpiredToken());

        var response = await cluster.Anonymous.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Another_users_task_is_not_visible()
    {
        var created = await cluster.CreateTaskAsync(cluster.Admin, "Ajena", "admin-user");

        var response = await cluster.User.GetAsync($"api/v1/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await cluster.DeleteTaskAsync(cluster.Admin, created.Id, created.Version);
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
