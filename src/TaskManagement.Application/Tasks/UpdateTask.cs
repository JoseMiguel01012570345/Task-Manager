using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public static class UpdateTask
{
    public sealed record Command(
        Guid Id,
        string? Title,
        string? Description,
        string? AssigneeId,
        DateTimeOffset? DueDate,
        uint? Version);

    public sealed class Handler(
        ITaskRepository repository,
        ICurrentUser currentUser,
        IClock clock,
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

            if (!TaskAccess.CanAssignTo(currentUser, command.AssigneeId!))
            {
                throw new ForbiddenException("A user can only assign a task to themselves.");
            }

            task.Update(
                command.Title!,
                command.Description,
                command.AssigneeId!,
                command.DueDate,
                command.Version!.Value,
                clock.UtcNow);

            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return task;
        }
    }
}
