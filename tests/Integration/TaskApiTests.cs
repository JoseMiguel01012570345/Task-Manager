using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class TaskApiTests(ClusterApi cluster)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Create_and_read_a_task()
    {
        var created = await cluster.CreateTaskAsync(cluster.User, "Desde el cluster", "dev-user");

        var fetched = await cluster.User.GetAsync($"api/v1/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
        var stored = await fetched.Content.ReadFromJsonAsync<ClusterApi.TaskBody>(Json);
        Assert.Equal("Desde el cluster", stored?.Title);

        await cluster.DeleteTaskAsync(cluster.User, created.Id, created.Version);
    }

    [Fact]
    public async Task Repeating_the_current_status_is_rejected()
    {
        var created = await cluster.CreateTaskAsync(cluster.User, "Sin cambio", "dev-user");

        var response = await cluster.User.PatchAsJsonAsync($"api/v1/tasks/{created.Id}/status", new
        {
            status = "Pending",
            version = created.Version
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        await cluster.DeleteTaskAsync(cluster.User, created.Id, created.Version);
    }
}
