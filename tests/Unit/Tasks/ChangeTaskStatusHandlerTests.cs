using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class ChangeTaskStatusHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_AllowedTransition_UpdatesStatus()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Work", null, "dev-user", "dev-user", null, Now);
        repository.Items.Add(task);
        var audit = new RecordingAuditLog();
        var handler = new ChangeTaskStatus.Handler(
            repository,
            new TestCurrentUser("dev-user"),
            new TestClock(Now),
            audit,
            new ChangeTaskStatusValidator());

        var updated = await handler.Handle(
            new ChangeTaskStatus.Command(task.Id, "InProgress", 0),
            CancellationToken.None);

        Assert.Equal(TaskState.InProgress, updated.Status);
        Assert.Equal("TaskStatusChanged", Assert.Single(audit.Entries).Action);
    }

    [Fact]
    public async Task Handle_InvalidTransition_ThrowsDomainException()
    {
        var repository = new FakeTaskRepository();
        var task = TaskItem.Create("Work", null, "dev-user", "dev-user", null, Now);
        task.ChangeStatus(TaskState.InProgress, 0, Now);
        repository.Items.Add(task);
        var handler = new ChangeTaskStatus.Handler(
            repository,
            new TestCurrentUser("dev-user"),
            new TestClock(Now),
            new RecordingAuditLog(),
            new ChangeTaskStatusValidator());

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(new ChangeTaskStatus.Command(task.Id, "Pending", 0), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownStatus_IsValidationError()
    {
        var handler = new ChangeTaskStatus.Handler(
            new FakeTaskRepository(),
            new TestCurrentUser("dev-user"),
            new TestClock(Now),
            new RecordingAuditLog(),
            new ChangeTaskStatusValidator());

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            handler.Handle(new ChangeTaskStatus.Command(Guid.NewGuid(), "Archived", 0), CancellationToken.None));
    }
}
