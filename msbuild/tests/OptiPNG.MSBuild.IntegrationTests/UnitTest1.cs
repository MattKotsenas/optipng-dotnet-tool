using System.Reflection;

using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

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
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
            .Save()
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task When_the_file_is_not_optimized_the_build_fails()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.unoptimized.png", Path.Join(projectDir, "unoptimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"unoptimized.png")
            .Save()
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().EndWith("is not optimized. Run optipng to optimize and try again.");
    }

    [Fact]
    public void When_the_file_does_not_exist_the_build_fails()
    {
        using IntegrationTestContext context = new();

        string fileName = @"C:\file that should not exist.png";

        context.ProjectCreator
            .ItemInclude("PngFiles", fileName)
            .Save()
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().Be($"Error optimizing '{fileName}': Can't open the input file");
    }

    private async Task CopyEmbeddedResourceToFileAsync(string resourceName, string target)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"Resource '{resourceName}' not found", nameof(resourceName));
        using Stream file = File.Open(target, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.CreateNew, Options = FileOptions.Asynchronous });

        await stream.CopyToAsync(file);
    }
}
