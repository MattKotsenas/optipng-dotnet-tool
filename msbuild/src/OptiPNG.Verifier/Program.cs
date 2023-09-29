using CliWrap.Buffered;

namespace OptiPNG.Verifier;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var files = args ?? Array.Empty<string>();

        var launcher = new Launcher.Launcher();
        var command = launcher.Create()
            .WithArguments(new[] { "-simulate" }.Union(files));

        var result = await command.ExecuteBufferedAsync();

        var exitCode = result.ExitCode;

        foreach (var file in files)
        {
            if (!result.StandardError.Contains($"{file} is already optimized."))
            {
                Console.Error.WriteLine($"'{file}' is not optimized. Run optipng to optimize and try again.");
                exitCode = -1;
            }
        }

        return exitCode;
    }
}
