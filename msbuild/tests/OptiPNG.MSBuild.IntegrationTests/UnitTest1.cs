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

            using (PackageRepository.Create(temp.FullName, feeds))
            {
                string version = GetLatestPackageVersionFromFeed(localFeed, "OptiPNG.MSBuild");

                ProjectCreator.Templates.SdkCsproj()
                    .ItemPackageReference("OptiPNG.MSBuild", version)
                    //.ItemInclude("PngFiles", @"C:\Users\mattkot\Downloads\sample.png")
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

    private static string GetLatestPackageVersionFromFeed(string feed, string packageName)
    {
        // Search for file that begin with our package name
        string[] files = Directory.GetFiles(feed, $"{packageName}*", SearchOption.TopDirectoryOnly);

        // Find the most recently modified
        FileInfo file = files.Select(f => new FileInfo(f)).OrderByDescending(f => f.LastWriteTimeUtc).First();

        // Extract the version from the name
        string version = file.Name.Replace($"{packageName}.", string.Empty).Replace(file.Extension, string.Empty);

        return version;
    }
}