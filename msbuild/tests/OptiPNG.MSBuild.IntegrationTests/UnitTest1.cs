using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

public class When_creating_a_project_with_no_PNG_files : IntegrationTestBase
{
    [Fact]
    public void The_build_succeeds()
    {
        using IntegrationTestContext context = new();

        context.ProjectCreator
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }
}

public class Given_a_project_with_PNG_files_implicitly_included_via_targets_wildcard : IntegrationTestBase
{
    [Fact]
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));

        context.ProjectCreator
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
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().EndWith("is not optimized. Run optipng to optimize and try again.");
    }

    [Fact]
    public async Task When_the_project_is_incrementally_built_no_work_is_done()
    {
        using IntegrationTestContext context = new();

        string incrementalBuildMessage = "Skipping target \"OptiPNGValidation\" because all output files are up-to-date with respect to the input files.";

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));

        context.ProjectCreator
            .TryBuild(restore: true, out bool initialBuildResult, out BuildOutput output);

        initialBuildResult.Should().BeTrue();
        output.GetConsoleLog().Should().NotContain(incrementalBuildMessage);

        context.ProjectCreator.TryBuild(restore: false, out bool secondBuildResult, out BuildOutput output2);
        secondBuildResult.Should().BeTrue();
        output2.GetConsoleLog().Should().Contain(incrementalBuildMessage);
    }
}

public class Given_a_project_with_an_explicit_reference_to_a_PNG_file : IntegrationTestBase
{
    [Fact]
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
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
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().Be($"Error optimizing '{fileName}': Can't open the input file");
    }
}

public class Given_a_project_with_both_PNG_files_that_match_the_targets_wildcard_and_an_explicit_reference_to_a_PNG_file : IntegrationTestBase
{
    [Fact]
    public async Task The_explicit_defintion_of_the_ItemGroup_wins_and_an_unoptimized_file_fails_the_build()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.unoptimized.png", Path.Join(projectDir, "unoptimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"unoptimized.png")
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().EndWith("is not optimized. Run optipng to optimize and try again.");
    }

    [Fact]
    public async Task The_explicit_defintion_of_the_ItemGroup_wins_and_an_optimized_file_build_succeeds()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.unoptimized.png", Path.Join(projectDir, "unoptimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();

        string projectDir = new FileInfo(context.ProjectCreator.FullPath).Directory!.FullName;
        await CopyEmbeddedResourceToFileAsync($"{GetType().Namespace}.Resources.optimized.png", Path.Join(projectDir, "optimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
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
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeFalse();

        buildOutput.ErrorEvents.Should().HaveCount(1);

        string? message = buildOutput.ErrorEvents.Single().Message;
        message.Should().NotBeNull();
        message!.Trim().Should().Be($"Error optimizing '{fileName}': Can't open the input file");
    }
}