using System.Reflection;

using CliWrap;
using CliWrap.Buffered;

namespace OptiPNG.Launcher.IntegrationTests;

public class When_a_custom_OptiPNG_is_available_on_the_users_PATH
{
    private readonly ITestOutputHelper _output;

    public When_a_custom_OptiPNG_is_available_on_the_users_PATH(ITestOutputHelper output)
    {
        _output = output;
    }

    private static string GetEnvSeparator()
    {
        var inspector = new PlatformInspector();
        return inspector.Inspect().EnvPathSeparator;
    }

    private string BuildPathToOptiPNGMock()
    {
        // TODO: Make this better, perhaps with MSBuild TargetOutputs -- https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-task?view=vs-2022
        var path = Path.Combine(Assembly.GetEntryAssembly()?.Location ?? string.Empty, "..", "..", "..", "..", "..", "OptiPNG.Mock", "bin", "Debug", "net7.0", "publish");
        return Path.GetFullPath(path);
    }

    [Fact]
    public async Task The_custom_executable_is_used()
    {
        var mockPath = BuildPathToOptiPNGMock();
        _output.WriteLine("Using mock path `{0}`", mockPath);

        // TODO: Replace `OptiPNGTool` with `dotnet optipng` and remove ProjectReference
        var result = await Cli.Wrap("OptiPNGTool")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                { "PATH", $@"{Environment.GetEnvironmentVariable("PATH")}{GetEnvSeparator()}{mockPath}" }
            })
            .ExecuteBufferedAsync();

        result.ExitCode.Should().Be(0);

        result.StandardOutput.Trim().Should().Be("Hello, World!");
        result.StandardError.Should().BeEmpty();
    }
}

public class When_there_is_no_custom_OptiPNG_available_on_the_users_PATH
{
    [Fact]
    public async Task The_vendor_implementation_is_used()
    {
        var result = await Cli.Wrap("OptiPNGTool")
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        result.ExitCode.Should().Be(0);

        result.StandardOutput.Should().Contain("Type \"optipng -h\" for extended help.");
        result.StandardError.Should().BeEmpty();
    }
}