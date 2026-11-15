using FluentValidation;

namespace TaskManagement.Application.Tasks;

public sealed class DeleteTaskValidator : AbstractValidator<DeleteTask.Command>
{
    public DeleteTaskValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Version).NotNull();
    }
}
