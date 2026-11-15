using TaskManagement.Application.Tasks;
using TaskManagement.UnitTests.Fakes;

namespace TaskManagement.UnitTests.Tasks;

public class ValidatorTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_rejects_a_due_date_in_the_past()
    {
        var validator = new CreateTaskValidator(new TestClock(Now));
        var result = validator.Validate(new CreateTask.Command("Titulo", null, "dev-user", Now.AddMinutes(-1)));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void List_rejects_a_page_size_above_the_maximum()
    {
        var result = new ListTasksValidator().Validate(new ListTasks.Query(null, null, 1, Paging.MaxPageSize + 1));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Status_change_rejects_an_unknown_status()
    {
        var result = new ChangeTaskStatusValidator().Validate(new ChangeTaskStatus.Command(Guid.NewGuid(), "Later", 1));

        Assert.False(result.IsValid);
    }
}
