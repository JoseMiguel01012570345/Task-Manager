using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class TaskApiTests
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client;

    public TaskApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_and_read_a_task()
    {
        var created = await _client.PostAsJsonAsync("/api/v1/tasks", new
        {
            title = "Desde la fabrica",
            description = "Integracion",
            assigneeId = "dev-user"
        });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var body = await created.Content.ReadFromJsonAsync<CreatedTask>(Json);
        Assert.NotNull(body);

        var fetched = await _client.GetAsync($"/api/v1/tasks/{body.Id}");
        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        var stored = await fetched.Content.ReadFromJsonAsync<CreatedTask>(Json);
        Assert.Equal("Desde la fabrica", stored?.Title);
    }

    private sealed record CreatedTask(Guid Id, string Title);
}
