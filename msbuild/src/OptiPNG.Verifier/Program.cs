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

        ParseResult results = new OutputParser().Parse(files, result.StandardError);
        foreach(string output in results.Output)
        {
            Console.Error.WriteLine(output);
        }

        return results.Failed ? -1 : result.ExitCode;
    }
}
