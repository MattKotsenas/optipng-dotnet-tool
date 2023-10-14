using System.IO.Abstractions;
using System.Reflection;

using CliWrap;
using CliWrap.Buffered;

namespace OptiPNG.Tool.IntegrationTests;

public class When_running_the_tool_with_a_custom_lib : IClassFixture<NuGetConfigFixture>, IClassFixture<EnvSeparatorFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly IFileSystem _fs = new FileSystem();
    private readonly string _nugetConfig;
    private readonly string _envSeparator;

    public When_running_the_tool_with_a_custom_lib(NuGetConfigFixture nugetFixture, EnvSeparatorFixture separatorFixture, ITestOutputHelper output)
    {
        _output = output;
        _nugetConfig = nugetFixture.NuGetConfig;
        _envSeparator = separatorFixture.Separator;

        _output.WriteLine($"Using NuGet package path '{nugetFixture.PackagePath}'");
    }

    private string BuildPathToOptiPNGMock()
    {
        // TODO: Make this better, perhaps with MSBuild TargetOutputs -- https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-task?view=vs-2022
        var path = Path.Combine(Assembly.GetEntryAssembly()?.Location ?? string.Empty, "..", "..", "..", "..", "..", "OptiPNG.Mock", "bin", "Debug", "net7.0", "publish");
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

            _fs.File.WriteAllText(nugetConfigPath, _nugetConfig);

            await Install(temp.FullName, nugetConfigPath);
            BufferedCommandResult result = await Run(temp.FullName);

            result.StandardOutput.Trim().Should().Be("Hello, World!");
        }
    }
}

public class When_running_the_default_tool : IClassFixture<NuGetConfigFixture>, IClassFixture<EnvSeparatorFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly IFileSystem _fs = new FileSystem();
    private readonly string _nugetConfig;
    private readonly string _envSeparator;

    public When_running_the_default_tool(NuGetConfigFixture nugetFixture, EnvSeparatorFixture separatorFixture, ITestOutputHelper output)
    {
        _output = output;
        _nugetConfig = nugetFixture.NuGetConfig;
        _envSeparator = separatorFixture.Separator;

        _output.WriteLine($"Using NuGet package path '{nugetFixture.PackagePath}'");
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

            _fs.File.WriteAllText(nugetConfigPath, _nugetConfig);

            await Install(temp.FullName, nugetConfigPath);
            BufferedCommandResult result = await Run(temp.FullName);

            result.StandardOutput.Trim().Should().EndWith("Type \"optipng -h\" for extended help.");
        }
    }
}
