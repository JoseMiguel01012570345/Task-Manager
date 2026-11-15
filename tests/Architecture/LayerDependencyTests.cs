namespace TaskManagement.ArchitectureTests;

public class LayerDependencyTests
{
    [Fact]
    public void Domain_does_not_reference_outer_layers()
    {
        var names = Names(typeof(TaskManagement.Domain.DomainAssembly));

        Assert.DoesNotContain("TaskManagement.Application", names);
        Assert.DoesNotContain("TaskManagement.Infrastructure", names);
        Assert.DoesNotContain("TaskManagement.API", names);
    }

    [Fact]
    public void Application_does_not_reference_infrastructure_or_the_api()
    {
        var names = Names(typeof(TaskManagement.Application.DependencyInjection));

        Assert.DoesNotContain("TaskManagement.Infrastructure", names);
        Assert.DoesNotContain("TaskManagement.API", names);
    }

    [Fact]
    public void Infrastructure_does_not_reference_the_api()
    {
        var names = Names(typeof(TaskManagement.Infrastructure.DependencyInjection));

        Assert.DoesNotContain("TaskManagement.API", names);
    }

    private static IEnumerable<string?> Names(Type marker) =>
        marker.Assembly.GetReferencedAssemblies().Select(assembly => assembly.Name);
}
