using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public static class CreateTask
{
    public sealed record Command(string? Title, string? Description, string? AssigneeId, DateTimeOffset? DueDate);

    public sealed record Result(TaskItem Task);

    public sealed class Handler(
        ITaskRepository repository,
        ICurrentUser currentUser,
        IClock clock,
        IAuditLog auditLog,
        IValidator<Command> validator)
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                throw new RequestValidationException(validation);
            }

            if (!TaskAccess.CanAssignTo(currentUser, command.AssigneeId!))
            {
                throw new ForbiddenException("A user can only assign a new task to themselves.");
            }

            var now = clock.UtcNow;
            var task = TaskItem.Create(
                command.Title!,
                command.Description,
                command.AssigneeId!,
                currentUser.UserId,
                command.DueDate,
                now);

            await repository.AddAsync(task, cancellationToken).ConfigureAwait(false);
            await auditLog.WriteAsync("TaskCreated", task.Id, currentUser.UserId, task.Title, cancellationToken).ConfigureAwait(false);
            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return new Result(task);
        }
    }
}
