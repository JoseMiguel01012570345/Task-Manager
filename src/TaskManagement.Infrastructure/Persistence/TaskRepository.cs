using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Tasks;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Infrastructure.Persistence;

internal sealed class TaskRepository(AppDbContext db) : ITaskRepository
{
    public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        db.Tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.Tasks.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

    public async Task<PagedTasks> ListAsync(TaskListFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<TaskItem> query = db.Tasks.AsNoTracking();

        if (filter.Status is not null)
        {
            query = query.Where(task => task.Status == filter.Status);
        }

        if (!string.IsNullOrWhiteSpace(filter.AssigneeId))
        {
            query = query.Where(task => task.AssigneeId == filter.AssigneeId);
        }

        if (!string.IsNullOrWhiteSpace(filter.VisibleToUserId))
        {
            var userId = filter.VisibleToUserId;
            query = query.Where(task => task.AssigneeId == userId || task.CreatedById == userId);
        }

        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await query
            .OrderByDescending(task => task.CreatedAt)
            .ThenByDescending(task => task.Id)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedTasks(items, filter.Page, filter.PageSize, total);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }
}
