using TaskManagement.Application.Abstractions;

namespace TaskManagement.Infrastructure.Time;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
