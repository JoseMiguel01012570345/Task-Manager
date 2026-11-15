using FluentValidation;
using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Exceptions;

namespace TaskManagement.Application.Tasks;

public static class ListTasks
{
    public sealed record Query(string? Status, string? AssigneeId, int? Page, int? PageSize);

    public sealed class Handler(
        ITaskRepository repository,
        ICurrentUser currentUser,
        IValidator<Query> validator)
    {
        public async Task<PagedTasks> Handle(Query query, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(query, cancellationToken).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                throw new RequestValidationException(validation);
            }

            var page = query.Page ?? Paging.DefaultPage;
            var pageSize = query.PageSize ?? Paging.DefaultPageSize;
            Domain.Tasks.TaskState? status = string.IsNullOrWhiteSpace(query.Status)
                ? null
                : Enum.Parse<Domain.Tasks.TaskState>(query.Status, ignoreCase: true);

            var filter = new TaskListFilter(
                status,
                string.IsNullOrWhiteSpace(query.AssigneeId) ? null : query.AssigneeId.Trim(),
                currentUser.IsAdministrator ? null : currentUser.UserId,
                page,
                pageSize);

            return await repository.ListAsync(filter, cancellationToken).ConfigureAwait(false);
        }
    }
}
