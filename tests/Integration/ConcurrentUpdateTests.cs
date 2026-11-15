using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Tasks;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.IntegrationTests;

public class ConcurrentUpdateTests
{
    [Fact]
    public async Task Two_updates_with_the_same_version_do_not_overwrite_silently()
    {
        var now = DateTimeOffset.UtcNow;
        Guid id;

        await using (var setup = new AppDbContextFactory().CreateDbContext([]))
        {
            var created = TaskItem.Create("concurrent-update", null, "dev-user", "dev-user", null, now);
            setup.Tasks.Add(created);
            await setup.SaveChangesAsync();
            id = created.Id;
        }

        await using var first = new AppDbContextFactory().CreateDbContext([]);
        await using var second = new AppDbContextFactory().CreateDbContext([]);
        var left = await first.Tasks.FindAsync(id);
        var right = await second.Tasks.FindAsync(id);
        Assert.NotNull(left);
        Assert.NotNull(right);
        Assert.Equal(left.Version, right.Version);

        left.Update("first-writer", null, "dev-user", null, left.Version, now.AddMinutes(1));
        right.Update("second-writer", null, "dev-user", null, right.Version, now.AddMinutes(1));

        await first.SaveChangesAsync();
        var repository = new TaskRepository(second);
        await Assert.ThrowsAsync<ConcurrencyException>(() => repository.SaveChangesAsync(CancellationToken.None));

        await using var check = new AppDbContextFactory().CreateDbContext([]);
        var stored = await check.Tasks.FindAsync(id);
        Assert.NotNull(stored);
        Assert.Equal("first-writer", stored.Title);

        stored.Delete(stored.Version, now.AddMinutes(2));
        await check.SaveChangesAsync();
    }
}
