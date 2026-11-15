namespace TaskManagement.ArchitectureTests;

public class SolutionSmokeTests
{
    [Fact]
    public void Layer_assemblies_load()
    {
        Assert.NotNull(typeof(TaskManagement.Domain.DomainAssembly).Assembly);
        Assert.NotNull(typeof(TaskManagement.Application.DependencyInjection).Assembly);
        Assert.NotNull(typeof(TaskManagement.Infrastructure.DependencyInjection).Assembly);
        Assert.NotNull(typeof(Program).Assembly);
    }
}
