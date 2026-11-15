using TaskManagement.Application.Abstractions;

namespace TaskManagement.Infrastructure.Auditing;

internal sealed class NoOpAuditLog : IAuditLog
{
    public Task WriteAsync(
        string action,
        Guid resourceId,
        string actorId,
        string summary,
        CancellationToken cancellationToken) => Task.CompletedTask;
}
