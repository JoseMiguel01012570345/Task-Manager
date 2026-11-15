using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class ListTasksHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Handle_CombinesStatusAndAssignee_AndKeepsOtherPagesOut()
    {
        var repository = new FakeTaskRepository();
        Seed(repository, "dev-user", TaskState.Pending, 3);
        Seed(repository, "dev-user", TaskState.Done, 1);
        Seed(repository, "other-user", TaskState.Pending, 2);

        var handler = new ListTasks.Handler(
            repository,
            new TestCurrentUser("admin", isAdministrator: true),
            new ListTasksValidator());

        var page = await handler.Handle(
            new ListTasks.Query("Pending", "dev-user", Page: 1, PageSize: 2),
            CancellationToken.None);

        Assert.Equal(3, page.TotalCount);
        Assert.Equal(2, page.Items.Count);
        Assert.All(page.Items, task =>
        {
            Assert.Equal(TaskState.Pending, task.Status);
            Assert.Equal("dev-user", task.AssigneeId);
        });

        var second = await handler.Handle(
            new ListTasks.Query("Pending", "dev-user", Page: 2, PageSize: 2),
            CancellationToken.None);

        Assert.Single(second.Items);
        Assert.DoesNotContain(second.Items, item => page.Items.Any(first => first.Id == item.Id));
    }

    [Fact]
    public async Task Handle_PageSizeAboveMaximum_IsRejected()
    {
        var handler = new ListTasks.Handler(
            new FakeTaskRepository(),
            new TestCurrentUser("dev-user"),
            new ListTasksValidator());

        await Assert.ThrowsAsync<RequestValidationException>(() =>
            handler.Handle(new ListTasks.Query(null, null, 1, 101), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonAdmin_DoesNotReceiveOtherPeoplesTasks()
    {
        var repository = new FakeTaskRepository();
        Seed(repository, "dev-user", TaskState.Pending, 1);
        Seed(repository, "other-user", TaskState.Pending, 1);

        var handler = new ListTasks.Handler(
            repository,
            new TestCurrentUser("dev-user"),
            new ListTasksValidator());

        var page = await handler.Handle(new ListTasks.Query(null, null, null, null), CancellationToken.None);

        Assert.Equal(1, page.TotalCount);
        Assert.Equal(Paging.DefaultPageSize, page.PageSize);
        Assert.Equal("dev-user", Assert.Single(page.Items).AssigneeId);
    }

    private static void Seed(FakeTaskRepository repository, string assignee, TaskState status, int count)
    {
        for (var index = 0; index < count; index++)
        {
            var task = TaskItem.Create($"Task {assignee} {index}", null, assignee, assignee, null, Now.AddMinutes(index));
            if (status != TaskState.Pending)
            {
                task.ChangeStatus(status, 0, Now);
            }

            repository.Items.Add(task);
        }
    }
}
