using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);

    Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedTasks> ListAsync(TaskListFilter filter, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
