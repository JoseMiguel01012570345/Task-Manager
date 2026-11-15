namespace TaskManagement.Application.Abstractions;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    string UserId { get; }

    bool IsAdministrator { get; }
}
