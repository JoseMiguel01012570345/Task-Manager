using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class UpdateTaskHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_CurrentVersion_UpdatesEditableFieldsAndKeepsStatus()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Old", "d", "dev-user", "dev-user", null, Now);
        task.ChangeStatus(TaskState.InProgress, 0, Now);
        repository.Items.Add(task);
        var handler = HandlerFor(repository, "dev-user");

        var updated = await handler.Handle(
            new UpdateTask.Command(task.Id, "New", "notes", "dev-user", Now.AddDays(3), 0),
            CancellationToken.None);

        Assert.Equal("New", updated.Title);
        Assert.Equal(TaskState.InProgress, updated.Status);
        Assert.Equal("dev-user", updated.CreatedById);
    }

    [Fact]
    public async Task Handle_StaleVersion_DoesNotOverwrite()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Old", null, "dev-user", "dev-user", null, Now);
        task.SetVersion(4);
        repository.Items.Add(task);
        var handler = HandlerFor(repository, "dev-user");

        await Assert.ThrowsAsync<ConcurrencyException>(() =>
            handler.Handle(new UpdateTask.Command(task.Id, "New", null, "dev-user", null, 3), CancellationToken.None));

        Assert.Equal("Old", task.Title);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task Handle_HiddenTask_IsNotFound()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Old", null, "other-user", "other-user", null, Now);
        repository.Items.Add(task);
        var handler = HandlerFor(repository, "dev-user");

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new UpdateTask.Command(task.Id, "New", null, "dev-user", null, 0), CancellationToken.None));
    }

    private static UpdateTask.Handler HandlerFor(FakeTaskRepository repository, string userId) =>
        new(repository, new TestCurrentUser(userId), new TestClock(Now), new RecordingAuditLog(), new UpdateTaskValidator(new TestClock(Now)));
}
