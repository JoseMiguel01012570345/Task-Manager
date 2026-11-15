using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class CreateTaskHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_ValidCommand_PersistsPendingTask()
    {
        var repository = new FakeTaskRepository();
        var handler = CreateHandler(repository);

        var result = await handler.Handle(
            new CreateTask.Command("  Ship API  ", "notes", "dev-user", Now.AddDays(1)),
            CancellationToken.None);

        Assert.Equal("Ship API", result.Task.Title);
        Assert.Equal(1, repository.SaveCount);
        Assert.Equal(1u, result.Task.Version);
    }

    [Fact]
    public async Task Handle_InvalidTitle_DoesNotInsert()
    {
        var repository = new FakeTaskRepository();
        var handler = CreateHandler(repository);

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            handler.Handle(new CreateTask.Command(" ", null, "dev-user", null), CancellationToken.None));

        Assert.Empty(repository.Items);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task Handle_UserAssigningSomeoneElse_IsForbidden()
    {
        var repository = new FakeTaskRepository();
        var handler = CreateHandler(repository);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(new CreateTask.Command("Title", null, "other-user", null), CancellationToken.None));

        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task Handle_Administrator_CanAssignAnotherUser()
    {
        var repository = new FakeTaskRepository();
        var handler = new CreateTask.Handler(
            repository,
            new TestCurrentUser("admin-user", isAdministrator: true),
            new TestClock(Now),
            new CreateTaskValidator(new TestClock(Now)));

        var result = await handler.Handle(
            new CreateTask.Command("Title", null, "other-user", null),
            CancellationToken.None);

        Assert.Equal("other-user", result.Task.AssigneeId);
        Assert.Equal("admin-user", result.Task.CreatedById);
    }

    private static CreateTask.Handler CreateHandler(FakeTaskRepository repository) =>
        new(
            repository,
            new TestCurrentUser("dev-user"),
            new TestClock(Now),
            new CreateTaskValidator(new TestClock(Now)));
}
