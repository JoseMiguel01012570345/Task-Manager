namespace TaskManagement.Application.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException()
        : base("Task was not found.")
    {
    }
}
