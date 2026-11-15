using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public sealed class UpdateTaskValidator : AbstractValidator<UpdateTask.Command>
{
    public UpdateTaskValidator(IClock clock)
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(TaskItem.TitleMaxLength);
        RuleFor(command => command.Description).MaximumLength(TaskItem.DescriptionMaxLength);
        RuleFor(command => command.AssigneeId).NotEmpty().MaximumLength(TaskItem.UserIdMaxLength);
        RuleFor(command => command.Version).NotNull();
        RuleFor(command => command.DueDate)
            .Must(due => due is null || due.Value >= clock.UtcNow)
            .WithMessage("Due date cannot be in the past.");
    }
}
