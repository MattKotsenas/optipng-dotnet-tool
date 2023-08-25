using System.Reflection;

using CliWrap;
using CliWrap.Buffered;

namespace OptiPNG.Runner.Tests;

public class When_OptiPNG_is_discoverable
{
    private readonly record struct PlatformInfo(string RuntimeIdentifier, string EnvPathSeparator);

    private readonly ITestOutputHelper _output;

    public When_OptiPNG_is_discoverable(ITestOutputHelper output)
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
    public async Task It_run_and_returns_a_zero_exit_code()
    {
        var platformInfo = GetPlatformInfo();
        var mockPath = BuildPathToOptiPNGMock(platformInfo.RuntimeIdentifier);
        _output.WriteLine("Using mock path `{0}`", mockPath);

        var result = await Cli.Wrap("OptiPNGRunner")
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

public class When_OptiPNG_is_not_discoverable
{
    [Fact]
    public async Task It_returns_a_non_zero_exit_code()
    {
        var result = await Cli.Wrap("OptiPNGRunner")
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        result.ExitCode.Should().NotBe(0);

        result.StandardOutput.Should().BeEmpty();
        result.StandardError.Should().Contain("Failed to start a process");
    }
}