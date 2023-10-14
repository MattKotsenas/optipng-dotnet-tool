using System.IO.Abstractions;
using System.Reflection;

using CliWrap;
using CliWrap.Buffered;

using OptiPNG.Launcher;

namespace OptiPNG.Tool.IntegrationTests;

public class When_running_the_default_tool
{
    private readonly ITestOutputHelper _output;
    private readonly IFileSystem _fs = new FileSystem();
    private readonly string _nugetConfig;

    public When_running_the_default_tool(ITestOutputHelper output)
    {
        _output = output;
        _nugetConfig = GenerateNuGetConfig();
    }

    private string GenerateNuGetConfig()
    {
        // Pull the artifacts package path from the <AssemblyMetadata> MSBuild property in the project file
        IEnumerable<AssemblyMetadataAttribute> attributes = typeof(When_running_the_default_tool).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        string? packagePath = attributes.Single(attribute => attribute.Key == "PackagePath").Value;

        ArgumentNullException.ThrowIfNull(packagePath);
        _output.WriteLine($"Using NuGet package path '{packagePath}'");

        return
@$"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""Local"" value=""{packagePath}"" />
  </packageSources>
</configuration>
";
    }

    private static string GetEnvSeparator()
    {
        var inspector = new PlatformInspector();
        return inspector.Inspect().EnvPathSeparator;
    }

    private static async Task<BufferedCommandResult> Install(string temp, string nuget)
    {
        IReadOnlyCollection<string> args = $"tool install optipng.tool --tool-path {temp} --prerelease --configfile {nuget}".Split(" ");

        return await Cli.Wrap("dotnet")
        .WithArguments(args)
        .ExecuteBufferedAsync();
    }

    private static async Task<BufferedCommandResult> Run(string temp)
    {
        return await Cli.Wrap("dotnet")
            .WithArguments("optipng")
            .WithEnvironmentVariables(new Dictionary<string, string?>
            {
                ["PATH"] = Environment.GetEnvironmentVariable("PATH") + $"{GetEnvSeparator()}{temp}"
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
