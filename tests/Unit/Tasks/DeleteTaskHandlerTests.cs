using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class DeleteTaskHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_OwnTask_SoftDeletesAndAudits()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Gone", null, "dev-user", "dev-user", null, Now);
        repository.Items.Add(task);
        var audit = new RecordingAuditLog();
        var handler = new DeleteTask.Handler(
            repository,
            new TestCurrentUser("dev-user"),
            new TestClock(Now),
            audit,
            new DeleteTaskValidator());

        await handler.Handle(new DeleteTask.Command(task.Id, 0), CancellationToken.None);

        Assert.True(task.IsDeleted);
        Assert.Null(await repository.FindAsync(task.Id, CancellationToken.None));
        Assert.Equal("TaskDeleted", Assert.Single(audit.Entries).Action);
    }

    [Fact]
    public async Task Handle_MissingTask_ThrowsNotFound()
    {
        var handler = new DeleteTask.Handler(
            new FakeTaskRepository(),
            new TestCurrentUser("dev-user"),
            new TestClock(Now),
            new RecordingAuditLog(),
            new DeleteTaskValidator());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteTask.Command(Guid.NewGuid(), 0), CancellationToken.None));
    }
}
