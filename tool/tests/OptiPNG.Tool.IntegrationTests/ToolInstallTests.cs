using System.IO.Abstractions;
using System.Reflection;

using CliWrap;
using CliWrap.Buffered;

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

            string nugetConfigPath = Path.Combine(temp.FullName, "nuget.config");

            _fs.File.WriteAllText(nugetConfigPath, new NuGetCreator().Create(_assemblyMetadata.PackagePath));

            await Install(temp.FullName, nugetConfigPath);
            BufferedCommandResult result = await Run(temp.FullName);

            result.StandardOutput.Trim().Should().Be("Hello, World!");
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

            string nugetConfigPath = Path.Combine(temp.FullName, "nuget.config");

            _fs.File.WriteAllText(nugetConfigPath, new NuGetCreator().Create(_assemblyMetadata.PackagePath));

            await Install(temp.FullName, nugetConfigPath);
            BufferedCommandResult result = await Run(temp.FullName);

            result.StandardOutput.Trim().Should().EndWith("Type \"optipng -h\" for extended help.");
        }
    }
}
