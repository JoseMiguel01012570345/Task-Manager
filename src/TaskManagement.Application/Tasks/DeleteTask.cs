using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.Application.Tasks;

public static class DeleteTask
{
    public sealed record Command(Guid Id, uint? Version);

    public sealed class Handler(
        ITaskRepository repository,
        ICurrentUser currentUser,
        IClock clock,
        IAuditLog auditLog,
        IValidator<Command> validator)
    {
        public async Task Handle(Command command, CancellationToken cancellationToken)
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

            task.Delete(command.Version!.Value, clock.UtcNow);
            await auditLog.WriteAsync(
                "TaskDeleted",
                task.Id,
                currentUser.UserId,
                "Soft deleted",
                cancellationToken).ConfigureAwait(false);
            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
