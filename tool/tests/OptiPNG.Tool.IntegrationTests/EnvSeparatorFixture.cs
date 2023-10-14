using OptiPNG.Launcher;

namespace OptiPNG.Tool.IntegrationTests;

public class EnvSeparatorFixture
{
    public string Separator { get; init; }

    public EnvSeparatorFixture()
    {
        var inspector = new PlatformInspector();
        Separator = inspector.Inspect().EnvPathSeparator;
    }
}
