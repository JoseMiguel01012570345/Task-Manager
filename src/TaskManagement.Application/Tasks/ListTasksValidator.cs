using FluentValidation;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public sealed class ListTasksValidator : AbstractValidator<ListTasks.Query>
{
    public ListTasksValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1)
            .When(query => query.Page is not null);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, Paging.MaxPageSize)
            .When(query => query.PageSize is not null);

        RuleFor(query => query.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || Enum.TryParse<TaskState>(status, true, out _))
            .WithMessage("Status is not a known value.");

        RuleFor(query => query.AssigneeId)
            .MaximumLength(TaskItem.UserIdMaxLength);
    }
}
