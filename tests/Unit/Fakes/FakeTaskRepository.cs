using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.UnitTests.Fakes;

internal sealed class FakeTaskRepository : ITaskRepository
{
    public List<TaskItem> Items { get; } = [];

    public int SaveCount { get; private set; }

    public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        Items.Add(task);
        return Task.CompletedTask;
    }

    public Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Items.SingleOrDefault(task => task.Id == id && !task.IsDeleted));

    public Task<PagedTasks> ListAsync(TaskListFilter filter, CancellationToken cancellationToken)
    {
        IEnumerable<TaskItem> query = Items.Where(task => !task.IsDeleted);
        if (filter.Status is not null)
        {
            query = query.Where(task => task.Status == filter.Status);
        }

        if (!string.IsNullOrWhiteSpace(filter.AssigneeId))
        {
            query = query.Where(task => task.AssigneeId == filter.AssigneeId);
        }

        var materialized = query
            .OrderByDescending(task => task.CreatedAt)
            .ThenByDescending(task => task.Id)
            .ToList();

        var page = materialized
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return Task.FromResult(new PagedTasks(page, filter.Page, filter.PageSize, materialized.Count));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveCount++;
        foreach (var task in Items.Where(task => task.Version == 0))
        {
            task.SetVersion(1);
        }

        return Task.CompletedTask;
    }
}
