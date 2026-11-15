using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Domain.Tasks;

public sealed class TaskItem
{
    public const int TitleMaxLength = 200;
    public const int DescriptionMaxLength = 4000;
    public const int UserIdMaxLength = 128;

    private TaskItem()
    {
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public TaskState Status { get; private set; }

    public string AssigneeId { get; private set; } = string.Empty;

    public string CreatedById { get; private set; } = string.Empty;

    public DateTimeOffset? DueDate { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public uint Version { get; private set; }

    public static TaskItem Create(
        string title,
        string? description,
        string assigneeId,
        string createdById,
        DateTimeOffset? dueDate,
        DateTimeOffset utcNow)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Status = TaskState.Pending,
            CreatedById = RequireUserId(createdById, nameof(createdById)),
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
        };

        task.ApplyDetails(title, description, assigneeId, dueDate, utcNow, isCreate: true);
        return task;
    }

    public void Update(
        string title,
        string? description,
        string assigneeId,
        DateTimeOffset? dueDate,
        uint expectedVersion,
        DateTimeOffset utcNow)
    {
        EnsureNotDeleted();
        EnsureExpectedVersion(expectedVersion);
        ApplyDetails(title, description, assigneeId, dueDate, utcNow, isCreate: false);
        UpdatedAt = utcNow;
    }

    public void ChangeStatus(TaskState next, uint expectedVersion, DateTimeOffset utcNow)
    {
        EnsureNotDeleted();
        EnsureExpectedVersion(expectedVersion);

        if (!CanTransition(Status, next))
        {
            throw new DomainException($"Cannot transition from {Status} to {next}.");
        }

        Status = next;
        UpdatedAt = utcNow;
    }

    public void Delete(uint expectedVersion, DateTimeOffset utcNow)
    {
        EnsureNotDeleted();
        EnsureExpectedVersion(expectedVersion);
        IsDeleted = true;
        UpdatedAt = utcNow;
    }

    public void EnsureExpectedVersion(uint expectedVersion)
    {
        if (Version != expectedVersion)
        {
            throw new ConcurrencyException();
        }
    }

    public static bool CanTransition(TaskState from, TaskState to) => (from, to) switch
    {
        (TaskState.Pending, TaskState.InProgress) => true,
        (TaskState.Pending, TaskState.Done) => true,
        (TaskState.InProgress, TaskState.Done) => true,
        (TaskState.Done, TaskState.InProgress) => true,
        _ => false,
    };

    internal void SetVersion(uint version) => Version = version;

    private void ApplyDetails(
        string title,
        string? description,
        string assigneeId,
        DateTimeOffset? dueDate,
        DateTimeOffset utcNow,
        bool isCreate)
    {
        var normalizedTitle = NormalizeRequired(title, TitleMaxLength, nameof(title));
        var normalizedDescription = (description ?? string.Empty).Trim();
        if (normalizedDescription.Length > DescriptionMaxLength)
        {
            throw new DomainException($"Description cannot exceed {DescriptionMaxLength} characters.");
        }

        if (dueDate is not null && dueDate.Value < utcNow)
        {
            throw new DomainException("Due date cannot be in the past.");
        }

        Title = normalizedTitle;
        Description = normalizedDescription;
        AssigneeId = RequireUserId(assigneeId, nameof(assigneeId));
        DueDate = dueDate;

        if (!isCreate)
        {
            UpdatedAt = utcNow;
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new DomainException("Deleted tasks cannot be changed.");
        }
    }

    private static string RequireUserId(string value, string name)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (normalized.Length == 0 || normalized.Length > UserIdMaxLength)
        {
            throw new DomainException($"{name} must be between 1 and {UserIdMaxLength} characters.");
        }

        return normalized;
    }

    private static string NormalizeRequired(string value, int maxLength, string name)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (normalized.Length == 0 || normalized.Length > maxLength)
        {
            throw new DomainException($"{name} must be between 1 and {maxLength} characters.");
        }

        return normalized;
    }
}
