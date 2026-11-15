using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public static class GetTask
{
    public sealed class Handler(ITaskRepository repository, ICurrentUser currentUser)
    {
        public async Task<TaskItem> Handle(Guid id, CancellationToken cancellationToken)
        {
            var task = await repository.FindAsync(id, cancellationToken).ConfigureAwait(false);
            if (task is null || !TaskAccess.CanView(task, currentUser))
            {
                throw new NotFoundException();
            }

            return task;
        }
    }
}
