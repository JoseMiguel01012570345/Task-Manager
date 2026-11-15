using TaskManagement.Domain.Tasks;

namespace TaskManagement.API.Contracts;

public sealed record CreateTaskRequest(string? Title, string? Description, string? AssigneeId, DateTimeOffset? DueDate);

public sealed record UpdateTaskRequest(
    string? Title,
    string? Description,
    string? AssigneeId,
    DateTimeOffset? DueDate,
    uint? Version);

public sealed record TaskPageResponse(
    IReadOnlyList<TaskResponse> Items,
    int Page,
    int PageSize,
    int TotalCount);

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string Description,
    string Status,
    string AssigneeId,
    string CreatedById,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    uint Version)
{
    public static TaskResponse From(TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status.ToString(),
        task.AssigneeId,
        task.CreatedById,
        task.DueDate,
        task.CreatedAt,
        task.UpdatedAt,
        task.Version);
}
