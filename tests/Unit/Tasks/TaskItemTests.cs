using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.UnitTests.Tasks;

public class TaskItemTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidData_StartsPending()
    {
        var task = CreateValid();

        Assert.Equal(TaskState.Pending, task.Status);
        Assert.Equal("Write docs", task.Title);
        Assert.Equal("dev-user", task.AssigneeId);
        Assert.Equal("dev-user", task.CreatedById);
        Assert.False(task.IsDeleted);
        Assert.Equal(0u, task.Version);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_RejectsEmptyTitle(string? title)
    {
        var error = Assert.Throws<DomainException>(() =>
            TaskItem.Create(title!, "body", "dev-user", "dev-user", null, Now));

        Assert.Contains("title", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_RejectsTitleLongerThan200()
    {
        var title = new string('a', TaskItem.TitleMaxLength + 1);

        Assert.Throws<DomainException>(() =>
            TaskItem.Create(title, null, "dev-user", "dev-user", null, Now));
    }

    [Fact]
    public void Create_RejectsDescriptionLongerThan4000()
    {
        var description = new string('b', TaskItem.DescriptionMaxLength + 1);

        Assert.Throws<DomainException>(() =>
            TaskItem.Create("Title", description, "dev-user", "dev-user", null, Now));
    }

    [Fact]
    public void Create_RejectsDueDateInThePast()
    {
        Assert.Throws<DomainException>(() =>
            TaskItem.Create("Title", null, "dev-user", "dev-user", Now.AddMinutes(-1), Now));
    }

    [Theory]
    [InlineData(TaskState.Pending, TaskState.InProgress)]
    [InlineData(TaskState.Pending, TaskState.Done)]
    [InlineData(TaskState.InProgress, TaskState.Done)]
    [InlineData(TaskState.Done, TaskState.InProgress)]
    public void ChangeStatus_AllowsDocumentedTransitions(TaskState from, TaskState to)
    {
        var task = TaskIn(from);

        task.ChangeStatus(to, expectedVersion: 0, Now);

        Assert.Equal(to, task.Status);
    }

    [Theory]
    [InlineData(TaskState.Pending, TaskState.Pending)]
    [InlineData(TaskState.InProgress, TaskState.Pending)]
    [InlineData(TaskState.Done, TaskState.Pending)]
    [InlineData(TaskState.Done, TaskState.Done)]
    [InlineData(TaskState.InProgress, TaskState.InProgress)]
    public void ChangeStatus_RejectsInvalidTransitions(TaskState from, TaskState to)
    {
        var task = TaskIn(from);

        Assert.Throws<DomainException>(() => task.ChangeStatus(to, 0, Now));
    }

    [Fact]
    public void Update_WithStaleVersion_ThrowsConcurrencyException()
    {
        var task = CreateValid();
        task.SetVersion(3);

        Assert.Throws<ConcurrencyException>(() =>
            task.Update("New", "desc", "dev-user", null, expectedVersion: 2, Now));
    }

    [Fact]
    public void Update_WithCurrentVersion_ChangesEditableFields()
    {
        var task = CreateValid();
        var due = Now.AddDays(2);

        task.Update("Renamed", "notes", "other-user", due, expectedVersion: 0, Now.AddMinutes(5));

        Assert.Equal("Renamed", task.Title);
        Assert.Equal("notes", task.Description);
        Assert.Equal("other-user", task.AssigneeId);
        Assert.Equal(due, task.DueDate);
        Assert.Equal(TaskState.Pending, task.Status);
    }

    [Fact]
    public void Delete_MarksTaskDeleted_AndBlocksFurtherChanges()
    {
        var task = CreateValid();

        task.Delete(0, Now);

        Assert.True(task.IsDeleted);
        Assert.Throws<DomainException>(() => task.ChangeStatus(TaskState.InProgress, 0, Now));
        Assert.Throws<DomainException>(() => task.Delete(0, Now));
    }

    private static TaskItem CreateValid() =>
        TaskItem.Create("  Write docs  ", " details ", "dev-user", "dev-user", Now.AddDays(1), Now);

    private static TaskItem TaskIn(TaskState status)
    {
        var task = CreateValid();
        if (status == TaskState.Pending)
        {
            return task;
        }

        task.ChangeStatus(TaskState.InProgress, 0, Now);
        if (status == TaskState.Done)
        {
            task.ChangeStatus(TaskState.Done, 0, Now);
        }

        return task;
    }
}
