using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public static class ChangeTaskStatus
{
    public sealed record Command(Guid Id, string? Status, uint? Version);

    public sealed class Handler(
        ITaskRepository repository,
        ICurrentUser currentUser,
        IClock clock,
        IAuditLog auditLog,
        IValidator<Command> validator)
    {
        public async Task<TaskItem> Handle(Command command, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                throw new RequestValidationException(validation);
            }

            var task = await repository.FindAsync(command.Id, cancellationToken).ConfigureAwait(false);
            if (task is null || !TaskAccess.CanModify(task, currentUser))
            {
                throw new NotFoundException();
            }

            var next = Enum.Parse<TaskState>(command.Status!, ignoreCase: true);
            var previous = task.Status;
            task.ChangeStatus(next, command.Version!.Value, clock.UtcNow);
            await auditLog.WriteAsync(
                "TaskStatusChanged",
                task.Id,
                currentUser.UserId,
                $"{previous} -> {next}",
                cancellationToken).ConfigureAwait(false);
            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return task;
        }
    }
}
