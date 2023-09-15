using CliWrap;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace OptiPNG.Tool;

internal record ErrorConsoleHolder(IAnsiConsole Console);

internal class Program
{
    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton<PlatformInspector>();
        services.AddSingleton<VendorMapper>();
        services.AddSingleton<VendorPathAppender>();

        services.AddSingleton<ErrorConsoleHolder>(sp =>
        {
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Out = new AnsiConsoleOutput(Console.Error)
            });

            return new ErrorConsoleHolder(console);
        });

        return services.BuildServiceProvider();
    }

    private static async Task<int> Main(string[] args)
    {
        var provider = BuildServiceProvider();

        try
        {
            provider.GetRequiredService<VendorPathAppender>().TryAppend();

            var result = await Cli.Wrap("optipng")
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput(), autoFlush: true))
                .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError(), autoFlush: true))
                .ExecuteAsync();

            return result.ExitCode;
        }
        catch (Exception ex)
        {
            var console = provider.GetRequiredService<ErrorConsoleHolder>().Console;

            console.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        return -1;
    }
}
