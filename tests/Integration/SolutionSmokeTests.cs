namespace TaskManagement.IntegrationTests;

public class SolutionSmokeTests
{
    [Fact]
    public void Api_assembly_is_referenced()
    {
        Assert.Equal("TaskManagement.API", typeof(Program).Assembly.GetName().Name);
    }
}
