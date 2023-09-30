using System.Threading.Tasks;
using CliWrap.Buffered;
using FluentAssertions;
using Xunit;

namespace OptiPNG.Launcher.IntegrationTests
{
    public class When_using_the_default_Launcher
    {
        // The main value of these tests is validating that we have dependecies correct on both
        // .NET Core and .NET Framework
        [Fact]
        public async Task The_vendor_version_is_used()
        {
            var launcher = new Launcher();
            var command = launcher.Create();

            var result = await command.ExecuteBufferedAsync();

            result.ExitCode.Should().Be(0);

            result.StandardOutput.Should().Contain("Type \"optipng -h\" for extended help.");
            result.StandardError.Should().BeEmpty();
        }
    }
}
