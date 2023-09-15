using System.Reflection;

using CliWrap;
using CliWrap.Buffered;

namespace OptiPNG.Tool.IntegrationTests;

public class When_a_custom_OptiPNG_is_available_on_the_users_PATH
{
    private readonly record struct PlatformInfo(string RuntimeIdentifier, string EnvPathSeparator);

    private readonly ITestOutputHelper _output;

    public When_a_custom_OptiPNG_is_available_on_the_users_PATH(ITestOutputHelper output)
    {
        _output = output;
    }

    private static PlatformInfo GetPlatformInfo()
    {
        if (OperatingSystem.IsWindows())
        {
            return new PlatformInfo
            {
                RuntimeIdentifier = "win-x64",
                EnvPathSeparator = ";"
            };
        }
        else if (OperatingSystem.IsLinux())
        {
            return new PlatformInfo
            {
                RuntimeIdentifier = "linux-x64",
                EnvPathSeparator = ":"
            };
        }
        else if (OperatingSystem.IsMacOS())
        {
            return new PlatformInfo
            {
                RuntimeIdentifier = "osx-x64",
                EnvPathSeparator = ":"
            };
        }
        else
        {
            throw new PlatformNotSupportedException($"Platform with Runtime Identifier {Environment.OSVersion} not supported");
        }
    }

    private string BuildPathToOptiPNGMock(string runtimeIdentifier)
    {
        var path = Path.Combine(Assembly.GetEntryAssembly()?.Location ?? string.Empty, "..", "..", "..", "..", "..", "OptiPNG.Mock", "bin", "Debug", "net7.0", runtimeIdentifier, "publish");
        return Path.GetFullPath(path);
    }

    [Fact]
    public async Task The_custom_executable_is_used()
    {
        var platformInfo = GetPlatformInfo();
        var mockPath = BuildPathToOptiPNGMock(platformInfo.RuntimeIdentifier);
        _output.WriteLine("Using mock path `{0}`", mockPath);

        // TODO: Replace `OptiPNGTool` with `dotnet optipng` and remove ProjectReference
        var result = await Cli.Wrap("OptiPNGTool")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                { "PATH", $@"{Environment.GetEnvironmentVariable("PATH")}{platformInfo.EnvPathSeparator}{mockPath}" }
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
        // TODO: Replace `OptiPNGTool` with `dotnet optipng` and remove ProjectReference
        var result = await Cli.Wrap("OptiPNGTool")
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        result.ExitCode.Should().Be(0);

        result.StandardOutput.Should().Contain("Type \"optipng -h\" for extended help.");
        result.StandardError.Should().BeEmpty();
    }
}