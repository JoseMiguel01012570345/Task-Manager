namespace TaskManagement.Application.Abstractions;

public interface IAuditLog
{
    Task WriteAsync(
        string action,
        Guid resourceId,
        string actorId,
        string summary,
        CancellationToken cancellationToken);
}
