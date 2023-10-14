using System.Reflection;

namespace OptiPNG.Tool.IntegrationTests;

public class NuGetConfigFixture
{
    public string PackagePath { get; init; }
    public string NuGetConfig { get; init; }

    public NuGetConfigFixture()
    {
        // Pull the artifacts package path from the <AssemblyMetadata> MSBuild property in the project file
        IEnumerable<AssemblyMetadataAttribute> attributes = typeof(When_running_the_default_tool).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        string? packagePath = attributes.Single(attribute => attribute.Key == "PackagePath").Value;

        ArgumentNullException.ThrowIfNull(packagePath);

        PackagePath = packagePath;
        NuGetConfig =
@$"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""Local"" value=""{packagePath}"" />
  </packageSources>
</configuration>
";
    }
}
