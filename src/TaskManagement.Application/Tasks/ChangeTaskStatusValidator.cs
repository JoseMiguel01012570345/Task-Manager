using FluentValidation;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public sealed class ChangeTaskStatusValidator : AbstractValidator<ChangeTaskStatus.Command>
{
    public ChangeTaskStatusValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Version).NotNull();
        RuleFor(command => command.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<TaskState>(status, true, out _))
            .WithMessage("Status is not a known value.");
    }
}
