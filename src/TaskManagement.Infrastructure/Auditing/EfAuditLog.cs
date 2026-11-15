using System.Diagnostics;
using TaskManagement.Application.Abstractions;

namespace TaskManagement.Infrastructure.Auditing;

internal sealed class EfAuditLog(Persistence.AppDbContext db) : IAuditLog
{
    public Task WriteAsync(
        string action,
        Guid resourceId,
        string actorId,
        string summary,
        CancellationToken cancellationToken)
    {
        db.AuditRecords.Add(new AuditRecord
        {
            Id = Guid.NewGuid(),
            Action = action,
            ResourceId = resourceId,
            ActorId = actorId,
            OccurredAt = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.Id,
            Summary = summary.Length > 1000 ? summary[..1000] : summary,
        });
        return Task.CompletedTask;
    }
}
