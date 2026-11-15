using System.Reflection;

namespace TaskManagement.Domain;

public static class DomainAssembly
{
    public static Assembly Assembly { get; } = typeof(DomainAssembly).Assembly;
}
