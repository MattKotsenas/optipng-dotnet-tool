namespace OptiPNG.Tool.IntegrationTests.AssemblyMetadata;

internal record Metadata
{
    public required DirectoryInfo PackagePath { get; init; }
    public required DirectoryInfo PublishPath { get; init; }
}
