using CliWrap;
using Spectre.Console;

namespace OptiPNG.Runner;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
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
            var settings = new AnsiConsoleSettings
            {
                Out = new AnsiConsoleOutput(Console.Error)
            };
            var console = AnsiConsole.Create(settings);

            console.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        return -1;
    }
}