using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class ConcurrentUpdateTests(ClusterApi cluster)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Two_updates_with_the_same_version_do_not_overwrite_silently()
    {
        var created = await cluster.CreateTaskAsync(cluster.User, "concurrent-update", "dev-user");

        var first = await cluster.User.PutAsJsonAsync($"api/v1/tasks/{created.Id}", new
        {
            title = "first-writer",
            description = "k3d",
            assigneeId = "dev-user",
            version = created.Version
        });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var stored = await first.Content.ReadFromJsonAsync<ClusterApi.TaskBody>(Json);
        Assert.NotNull(stored);
        Assert.Equal("first-writer", stored.Title);

        var second = await cluster.User.PutAsJsonAsync($"api/v1/tasks/{created.Id}", new
        {
            title = "second-writer",
            description = "k3d",
            assigneeId = "dev-user",
            version = created.Version
        });
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var fetched = await cluster.User.GetFromJsonAsync<ClusterApi.TaskBody>($"api/v1/tasks/{created.Id}", Json);
        Assert.Equal("first-writer", fetched?.Title);
        await cluster.DeleteTaskAsync(cluster.User, created.Id, stored.Version);
    }
}
