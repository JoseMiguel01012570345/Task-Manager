using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public sealed class CreateTaskValidator : AbstractValidator<CreateTask.Command>
{
    public CreateTaskValidator(IClock clock)
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(TaskItem.TitleMaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(TaskItem.DescriptionMaxLength);

        RuleFor(command => command.AssigneeId)
            .NotEmpty()
            .MaximumLength(TaskItem.UserIdMaxLength);

        RuleFor(command => command.DueDate)
            .Must(due => due is null || due.Value >= clock.UtcNow)
            .WithMessage("Due date cannot be in the past.");
    }
}
