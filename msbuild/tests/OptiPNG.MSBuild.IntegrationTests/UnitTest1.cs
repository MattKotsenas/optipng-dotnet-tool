using System.IO.Abstractions;
using System.Reflection;

using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

public class UnitTest1 : MSBuildTestBase
{
    [Fact]
    public void Test1()
    {
        string localFeed = GetPackagePath();

        IFileSystem fs = new FileSystem();

        using (fs.CreateDisposableDirectory(out IDirectoryInfo temp))
        {
            Uri[] feeds = new[]
            {
                new Uri(localFeed),
                new Uri("https://api.nuget.org/v3/index.json")
            };

            using (PackageRepository.Create(temp.FullName, feeds)
                .Package("OptiPNG.MSBuild", "1.1.16-beta-g22112f53c7", out Package package))
            {
                ProjectCreator.Templates.SdkCsproj()
    .ItemPackageReference(package)
    .Save(Path.Combine(temp.FullName, "ClassLibraryA", "ClassLibraryA.csproj"))
    .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

                result.Should().BeTrue();
            }
        }
    }

    private static string GetPackagePath()
    {
        // Pull the artifacts package path from the <AssemblyMetadata> MSBuild property in the project file
        IEnumerable<AssemblyMetadataAttribute> attributes = typeof(UnitTest1).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        string? packagePath = attributes.Single(attribute => attribute.Key == "PackagePath").Value;

        return Path.GetFullPath(packagePath!);
    }
}