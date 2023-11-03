namespace OptiPNG.Verifier;

public class OutputParser
{
    public ParseResult Parse(IReadOnlyCollection<string> files, string output)
    {
        List<string> outputs = new(files.Count);
        bool success = true;

        foreach (var file in files)
        {
            if (!output.Contains($"{file} is already optimized."))
            {
                outputs.Add($"'{file}' is not optimized. Run optipng to optimize and try again.");
                success = false;
            }
        }

        return new ParseResult(success, outputs);
    }
}
