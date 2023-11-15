using System.Reflection;

namespace OptiPNG.MSBuild.IntegrationTests;

internal class PngResources
{
    public string OptimizedPngName => $"{GetType().Namespace}.Resources.optimized.png";
    public string UnoptimizedPngName => $"{GetType().Namespace}.Resources.unoptimized.png";

    public async Task CopyEmbeddedResourceToFileAsync(string resourceName, string target)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"Resource '{resourceName}' not found", nameof(resourceName));
        using Stream file = File.Open(target, new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.CreateNew, Options = FileOptions.Asynchronous });

        await stream.CopyToAsync(file);
    }
}
