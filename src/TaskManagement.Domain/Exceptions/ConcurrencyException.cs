namespace TaskManagement.Domain.Exceptions;

public sealed class ConcurrencyException : DomainException
{
    public ConcurrencyException()
        : base("The task was updated by someone else.")
    {
    }
}
