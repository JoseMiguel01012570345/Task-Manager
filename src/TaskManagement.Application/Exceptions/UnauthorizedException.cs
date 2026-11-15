namespace TaskManagement.Application.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("Authentication is required.")
    {
    }
}
