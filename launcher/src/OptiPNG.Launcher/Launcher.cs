using CliWrap;
using CliWrap.Buffered;

using Microsoft.Extensions.DependencyInjection;

namespace OptiPNG.Launcher;

public class Launcher
{
    private readonly IServiceProvider _serviceProvider;

    public Launcher()
    {
        var services = new ServiceCollection();

        services.AddSingleton<PlatformInspector>();
        services.AddSingleton<VendorMapper>();
        services.AddSingleton<VendorPathAppender>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public Command Create()
    {
        _serviceProvider.GetRequiredService<VendorPathAppender>().TryAppend();

        var command = Cli.Wrap("optipng")
            .WithValidation(CommandResultValidation.None);

        return command;
    }
}
