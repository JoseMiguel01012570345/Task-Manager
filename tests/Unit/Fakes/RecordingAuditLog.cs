using TaskManagement.Application.Abstractions;

namespace TaskManagement.UnitTests.Fakes;

internal sealed class RecordingAuditLog : IAuditLog
{
    public List<AuditRecord> Entries { get; } = [];

    public Task WriteAsync(
        string action,
        Guid resourceId,
        string actorId,
        string summary,
        CancellationToken cancellationToken)
    {
        Entries.Add(new AuditRecord(action, resourceId, actorId, summary));
        return Task.CompletedTask;
    }

    internal sealed record AuditRecord(string Action, Guid ResourceId, string ActorId, string Summary);
}
