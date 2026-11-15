using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class GetTaskHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_OwnTask_ReturnsIt()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Mine", null, "dev-user", "dev-user", null, Now);
        repository.Items.Add(task);
        var handler = new GetTask.Handler(repository, new TestCurrentUser("dev-user"));

        var found = await handler.Handle(task.Id, CancellationToken.None);

        Assert.Equal(task.Id, found.Id);
    }

    [Fact]
    public async Task Handle_MissingOrHiddenTask_ThrowsNotFound()
    {
        var repository = new FakeTaskRepository();
        var hidden = TaskItem.Create("Hidden", null, "other-user", "other-user", null, Now);
        repository.Items.Add(hidden);
        var handler = new GetTask.Handler(repository, new TestCurrentUser("dev-user"));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(Guid.NewGuid(), CancellationToken.None));
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(hidden.Id, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Administrator_CanReadAnyTask()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Hidden", null, "other-user", "other-user", null, Now);
        repository.Items.Add(task);
        var handler = new GetTask.Handler(repository, new TestCurrentUser("admin", isAdministrator: true));

        var found = await handler.Handle(task.Id, CancellationToken.None);

        Assert.Equal(task.Id, found.Id);
    }
}
