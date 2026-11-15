using TaskManagement.Application.Abstractions;

namespace TaskManagement.UnitTests.Fakes;

internal sealed class TestCurrentUser(string userId, bool isAdministrator = false, bool isAuthenticated = true) : ICurrentUser
{
    public bool IsAuthenticated { get; } = isAuthenticated;

    public string UserId { get; } = userId;

    public bool IsAdministrator { get; } = isAdministrator;
}

internal sealed class TestClock(DateTimeOffset utcNow) : IClock
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}
