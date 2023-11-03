using System.Reflection;

using Microsoft.Build.Utilities.ProjectCreation;

namespace OptiPNG.MSBuild.IntegrationTests;

public abstract class IntegrationTestBase : MSBuildTestBase
{
    protected async Task CopyEmbeddedResourceToFileAsync(string resourceName, string target)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"Resource '{resourceName}' not found", nameof(resourceName));
        using Stream file = File.Open(target, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.CreateNew, Options = FileOptions.Asynchronous });

        await stream.CopyToAsync(file);
    }
}
