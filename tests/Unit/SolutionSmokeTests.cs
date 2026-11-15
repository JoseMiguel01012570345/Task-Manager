namespace TaskManagement.UnitTests;

public class SolutionSmokeTests
{
    [Fact]
    public void Domain_assembly_is_the_domain_project()
    {
        Assert.Equal("TaskManagement.Domain", TaskManagement.Domain.DomainAssembly.Assembly.GetName().Name);
    }
}
