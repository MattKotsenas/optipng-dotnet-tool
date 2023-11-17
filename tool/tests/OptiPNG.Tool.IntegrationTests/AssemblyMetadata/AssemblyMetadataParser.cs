using System.Reflection;

namespace OptiPNG.Tool.IntegrationTests.AssemblyMetadata;

internal class AssemblyMetadataParser
{
    private readonly Assembly _targetAssembly;

    public AssemblyMetadataParser() : this(typeof(AssemblyMetadataParser).Assembly)
    {
    }

    public AssemblyMetadataParser(Assembly targetAssemb)
    {
        _targetAssembly = targetAssemb;
    }

    public Metadata Parse()
    {
        // Pull the paths from the <AssemblyMetadata> MSBuild property in the project file
        IEnumerable<AssemblyMetadataAttribute> metadataAttributes = _targetAssembly.GetCustomAttributes<AssemblyMetadataAttribute>();

        string? packagePath = metadataAttributes.Single(attribute => attribute.Key == "PackagePath").Value;
        if (packagePath is null)
        {
            throw new InvalidDataException($"Assembly metadata attribute 'PackagePath' not found in the assembly '{_targetAssembly.FullName}'.");
        }
        if (!Directory.Exists(packagePath))
        {
            throw new InvalidDataException($"PackagePath '{packagePath}' does not exist.");
        }

        string? publishPath = metadataAttributes.Single(attribute => attribute.Key == "PublishPath").Value;
        if (publishPath is null)
        {
            throw new InvalidDataException($"Assembly metadata attribute 'PublishPath' not found in the assembly '{_targetAssembly.FullName}'.");
        }
        if (!Directory.Exists(publishPath))
        {
            throw new InvalidDataException($"PublishPath '{publishPath}' does not exist.");
        }

        return new Metadata
        {
            PackagePath = new DirectoryInfo(packagePath),
            PublishPath = new DirectoryInfo(publishPath),
        };
    }
}
