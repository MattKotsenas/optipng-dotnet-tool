using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

// TODO: Embed test files in project
// TODO: Add test for implicit file reference
// TODO: Add test for incremental build

public class When_creating_a_project_with_no_PNG_files : MSBuildTestBase
{
    [Fact]
    public void The_build_succeeds()
    {
        using IntegrationTestContext context = new();

        context.ProjectCreator
            .Save()
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }
}

public class Given_a_project_with_an_explicit_reference_to_a_PNG_file : MSBuildTestBase
{
    [Fact]
    public void When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();

        context.ProjectCreator
            .ItemInclude("PngFiles", @"C:\Users\mattkot\Downloads\test.png")
            .Save()
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public void When_the_file_is_not_optimized_the_build_fails()
    {
        using IntegrationTestContext context = new();

        context.ProjectCreator
            .ItemInclude("PngFiles", @"C:\Users\mattkot\Downloads\test-orig.png")
            .Save()
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().EndWith("is not optimized. Run optipng to optimize and try again.");
    }
}
