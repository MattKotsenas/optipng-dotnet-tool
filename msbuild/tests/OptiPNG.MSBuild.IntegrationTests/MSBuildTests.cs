using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

public class When_creating_a_project_with_no_PNG_files : MSBuildTestBase
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

public class Given_a_project_with_PNG_files_implicitly_included_via_targets_wildcard : MSBuildTestBase
{
    [Fact]
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.OptimizedPngName, Path.Join(context.ProjectDir, "optimized.png"));

        context.ProjectCreator
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task When_the_file_is_not_optimized_the_build_fails()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.UnoptimizedPngName, Path.Join(context.ProjectDir, "unoptimized.png"));

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
        PngResources pngResources = new();

        string incrementalBuildMessage = "Skipping target \"ValidatePNGs\" because all output files are up-to-date with respect to the input files.";

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.OptimizedPngName, Path.Join(context.ProjectDir, "optimized.png"));

        context.ProjectCreator
            .TryBuild(restore: true, out bool initialBuildResult, out BuildOutput output);

        initialBuildResult.Should().BeTrue();
        output.GetConsoleLog().Should().NotContain(incrementalBuildMessage);

        context.ProjectCreator.TryBuild(restore: false, out bool secondBuildResult, out BuildOutput output2);
        secondBuildResult.Should().BeTrue();
        output2.GetConsoleLog().Should().Contain(incrementalBuildMessage);
    }
}

public class Given_a_project_with_an_explicit_reference_to_a_PNG_file : MSBuildTestBase
{
    [Fact]
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.OptimizedPngName, Path.Join(context.ProjectDir, "optimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task When_the_file_is_not_optimized_the_build_fails()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.UnoptimizedPngName, Path.Join(context.ProjectDir, "unoptimized.png"));

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

public class Given_a_project_with_both_PNG_files_that_match_the_targets_wildcard_and_an_explicit_reference_to_a_PNG_file : MSBuildTestBase
{
    [Fact]
    public async Task The_explicit_defintion_of_the_ItemGroup_wins_and_an_unoptimized_file_fails_the_build()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.OptimizedPngName, Path.Join(context.ProjectDir, "optimized.png"));
        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.UnoptimizedPngName, Path.Join(context.ProjectDir, "unoptimized.png"));

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
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.OptimizedPngName, Path.Join(context.ProjectDir, "optimized.png"));
        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.UnoptimizedPngName, Path.Join(context.ProjectDir, "unoptimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task When_the_file_is_optimized_the_build_succeeds()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.OptimizedPngName, Path.Join(context.ProjectDir, "optimized.png"));

        context.ProjectCreator
            .ItemInclude("PngFiles", @"optimized.png")
            .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task When_the_file_is_not_optimized_the_build_fails()
    {
        using IntegrationTestContext context = new();
        PngResources pngResources = new();

        await pngResources.CopyEmbeddedResourceToFileAsync(pngResources.UnoptimizedPngName, Path.Join(context.ProjectDir, "unoptimized.png"));

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