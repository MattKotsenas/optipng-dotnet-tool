using System.IO.Abstractions;
using System.Reflection;

using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

internal class IntegrationTestContext : IDisposable
{
    private bool _disposed;
    private readonly IDisposable _tempDirectoryHandle;
    private readonly IDisposable _packageRepositoryHandle;

    public ProjectCreator ProjectCreator { get; private set; }
    public string ProjectDir => new FileInfo(ProjectCreator.FullPath).Directory!.FullName;

    public IntegrationTestContext()
    {
        string localFeed = GetPackagePath();

        IFileSystem fs = new FileSystem();

        _tempDirectoryHandle = fs.CreateDisposableDirectory(out IDirectoryInfo temp);

        Uri[] feeds = new[]
        {
                new Uri(localFeed),
                new Uri("https://api.nuget.org/v3/index.json")
        };

        _packageRepositoryHandle = PackageRepository.Create(temp.FullName, feeds);
        string version = GetLatestPackageVersionFromFeed(localFeed, "OptiPNG.MSBuild");

        ProjectCreator = ProjectCreator.Templates.SdkCsproj()
            .ItemPackageReference("OptiPNG.MSBuild", version)
            .Property("GenerateAssemblyInfo", "false") // Double-building results in duplicate defintions of assembly info. Since our tests don't rely on assembly info, skip generating it
            .Save(Path.Combine(temp.FullName, "ClassLibraryA", "ClassLibraryA.csproj")); // Do the initial save here while we have the temp path. Future updates can call .Save() with no path to update the exiting project
    }

    private static string GetPackagePath()
    {
        // Pull the artifacts package path from the <AssemblyMetadata> MSBuild property in the project file
        IEnumerable<AssemblyMetadataAttribute> attributes = typeof(When_creating_a_project_with_no_PNG_files).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        string? packagePath = attributes.Single(attribute => attribute.Key == "PackagePath").Value;

        return Path.GetFullPath(packagePath!);
    }

    private static string GetLatestPackageVersionFromFeed(string feed, string packageName)
    {
        if (!Directory.Exists(feed))
        {
            throw new ArgumentException($"Directory for package feed '{feed}' does not exist. Ensure you build prior to running integration tests.", nameof(feed));
        }

        // Search for file that begin with our package name
        string[] files = Directory.GetFiles(feed, $"{packageName}*", SearchOption.TopDirectoryOnly);

        // Find the most recently modified
        FileInfo file = files.Select(f => new FileInfo(f)).OrderByDescending(f => f.LastWriteTimeUtc).First();

        // Extract the version from the name
        string version = file.Name.Replace($"{packageName}.", string.Empty).Replace(file.Extension, string.Empty);

        return version;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _packageRepositoryHandle.Dispose();
                _tempDirectoryHandle.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
