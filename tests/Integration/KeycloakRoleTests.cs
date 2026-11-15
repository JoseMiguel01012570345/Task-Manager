using System.Net;
using System.Net.Http.Json;

namespace TaskManagement.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class KeycloakRoleTests(ClusterApi cluster)
{
    [Fact]
    public async Task Realm_roles_decide_who_can_assign_a_task()
    {
        var created = await cluster.CreateTaskAsync(cluster.Admin, "Asignada por admin", "dev-user");

        var forbidden = await cluster.User.PostAsJsonAsync("api/v1/tasks", new
        {
            title = "No permitida",
            description = "k3d",
            assigneeId = "admin-user"
        });

        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
        await cluster.DeleteTaskAsync(cluster.Admin, created.Id, created.Version);
    }
}
