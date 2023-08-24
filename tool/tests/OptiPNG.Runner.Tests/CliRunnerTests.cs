using CliWrap;
using CliWrap.Buffered;

namespace OptiPNG.Runner.Tests;

public class When_OptiPNG_is_discoverable
{
    [Fact]
    public async Task It_run_and_returns_a_zero_exit_code()
    {
        var result = await Cli.Wrap("OptiPNGRunner")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                // TODO: Make this test portable
                { "PATH", $@"{Environment.GetEnvironmentVariable("PATH")};C:\optipng" }
            })
            .ExecuteBufferedAsync();

        result.ExitCode.Should().Be(0);

        result.StandardOutput.Should().NotBeEmpty();
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