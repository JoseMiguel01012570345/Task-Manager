namespace TaskManagement.Infrastructure.Auditing;

public sealed class AuditRecord
{
    public Guid Id { get; set; }

    public string ActorId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public Guid ResourceId { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public string? CorrelationId { get; set; }

    public string Summary { get; set; } = string.Empty;
}
