using TaskManagement.Domain.Tasks;

namespace TaskManagement.Application.Tasks;

public sealed record TaskListFilter(TaskState? Status, string? AssigneeId, int Page, int PageSize);

public sealed record PagedTasks(IReadOnlyList<TaskItem> Items, int Page, int PageSize, int TotalCount);
