using System.IO.Abstractions;

using CliWrap;
using CliWrap.Buffered;

using Microsoft.Build.Utilities.ProjectCreation;

using OptiPNG.Tool.IntegrationTests.AssemblyMetadata;

namespace OptiPNG.Tool.IntegrationTests;

public class When_running_the_tool_with_a_custom_lib : IClassFixture<EnvSeparatorFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly IFileSystem _fs = new FileSystem();
    private readonly string _envSeparator;
    private readonly Metadata _assemblyMetadata;

    public When_running_the_tool_with_a_custom_lib(EnvSeparatorFixture separatorFixture, ITestOutputHelper output)
    {
        _output = output;
        _envSeparator = separatorFixture.Separator;
        _assemblyMetadata = new AssemblyMetadataParser().Parse();

        _output.WriteLine($"Using NuGet package path '{_assemblyMetadata.PackagePath}'");
    }

    private string BuildPathToOptiPNGMock()
    {
        var path = Path.Combine(_assemblyMetadata.PublishPath.FullName, "OptiPNG.Mock", "debug");
        return Path.GetFullPath(path);
    }

    private static async Task<BufferedCommandResult> Install(string temp, string nuget)
    {
        IReadOnlyCollection<string> args = $"tool install optipng.tool --tool-path {temp} --prerelease --configfile {nuget}".Split(" ");

        return await Cli.Wrap("dotnet")
        .WithArguments(args)
        .ExecuteBufferedAsync();
    }

    private async Task<BufferedCommandResult> Run(string temp)
    {
        var mockPath = BuildPathToOptiPNGMock();
        _output.WriteLine("Using mock path `{0}`", mockPath);

        return await Cli.Wrap("dotnet")
            .WithArguments("optipng")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                ["PATH"] = Environment.GetEnvironmentVariable("PATH") + $"{_envSeparator}{temp}{_envSeparator}{mockPath}"
            })
            .ExecuteBufferedAsync();
    }

    [Fact]
    public async Task The_custom_lib_is_used()
    {
        using (_fs.CreateDisposableDirectory(out IDirectoryInfo temp))
        {
            _output.WriteLine($"Using temp directory '{temp.FullName}'");
            using (PackageRepository repo = PackageRepository.Create(temp.FullName, new Uri(_assemblyMetadata.PackagePath.FullName)))
            {
                await Install(temp.FullName, repo.NuGetConfigPath);
                BufferedCommandResult result = await Run(temp.FullName);

                result.StandardOutput.Trim().Should().Be("Hello, World!");
            }
        }
    }
}

public class When_running_the_default_tool : IClassFixture<EnvSeparatorFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly IFileSystem _fs = new FileSystem();
    private readonly string _envSeparator;
    private readonly Metadata _assemblyMetadata;

    public When_running_the_default_tool(EnvSeparatorFixture separatorFixture, ITestOutputHelper output)
    {
        _output = output;
        _envSeparator = separatorFixture.Separator;
        _assemblyMetadata = new AssemblyMetadataParser().Parse();

        _output.WriteLine($"Using NuGet package path '{_assemblyMetadata.PackagePath}'");
    }

    private static async Task<BufferedCommandResult> Install(string temp, string nuget)
    {
        IReadOnlyCollection<string> args = $"tool install optipng.tool --tool-path {temp} --prerelease --configfile {nuget}".Split(" ");

        return await Cli.Wrap("dotnet")
        .WithArguments(args)
        .ExecuteBufferedAsync();
    }

    private async Task<BufferedCommandResult> Run(string temp)
    {
        return await Cli.Wrap("dotnet")
            .WithArguments("optipng")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                ["PATH"] = Environment.GetEnvironmentVariable("PATH") + $"{_envSeparator}{temp}"
            })
            .ExecuteBufferedAsync();
    }

    [Fact]
    public async Task The_vendor_lib_is_used()
    {
        using (_fs.CreateDisposableDirectory(out IDirectoryInfo temp))
        {
            _output.WriteLine($"Using temp directory '{temp.FullName}'");
            using (PackageRepository repo = PackageRepository.Create(temp.FullName, new Uri(_assemblyMetadata.PackagePath.FullName)))
            {
                await Install(temp.FullName, repo.NuGetConfigPath);
                BufferedCommandResult result = await Run(temp.FullName);

                result.StandardOutput.Trim().Should().EndWith("Type \"optipng -h\" for extended help.");
            }
        }
    }
}
