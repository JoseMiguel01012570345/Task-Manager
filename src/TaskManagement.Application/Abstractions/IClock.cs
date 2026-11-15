namespace TaskManagement.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
