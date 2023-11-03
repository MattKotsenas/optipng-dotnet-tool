using System.Text.RegularExpressions;

namespace OptiPNG.Verifier;

public class OutputParser
{
    private static readonly Regex RecordDelimiter = new Regex(@"^\*\* ", RegexOptions.Multiline);
    private static readonly string[] NewLines = new[] { Environment.NewLine };

    public string NotOptimizedErrorMessageFormatter(string file)
    {
        return $"'{file}' is not optimized. Run optipng to optimize and try again.";
    }

    public string UnknownErrorMessageFormatter(string file, string message)
    {
        return $"Error optimizing '{file}': {message}";
    }

    public ParseResult Parse(IReadOnlyCollection<string> files, string output)
    {
        List<string> outputs = new(files.Count);
        bool success = true;

        IReadOnlyCollection<ParseRecord> records = SplitOutputIntoRecords(output);

        foreach (string file in files)
        {
            ParseRecord? record = records.SingleOrDefault(r => r.Header == $"Processing: {file}");

            if (record is null)
            {
                throw new InvalidDataException($"Unable to parse output for file '{file}'");
            }

            if (record.Body.Contains("No output: simulation mode."))
            {
                success = false;
                outputs.Add(NotOptimizedErrorMessageFormatter(file));
            }
            else if (record.Body[0].StartsWith("Error:"))
            {
                success = false;

                string message = record.Body[0].Replace("Error: ", string.Empty);
                outputs.Add(UnknownErrorMessageFormatter(file, message));
            }
            else if (record.Body.Contains($"{file} is already optimized."))
            {
                // Happy path; do nothing.
            }
            else
            {
                throw new InvalidDataException($"Unexpected {nameof(ParseRecord)} format. Data is: '{string.Join(Environment.NewLine, record.Header, record.Body)}'");
            }
        }

        return new ParseResult(success, outputs);
    }

    private IReadOnlyCollection<ParseRecord> SplitOutputIntoRecords(string output)
    {
        List<ParseRecord> results = new();
        string[] records = RecordDelimiter.Split(output).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        foreach (string record in records)
        {
            string[] lines = record.Split(NewLines, StringSplitOptions.None);

            results.Add(new ParseRecord(lines[0], lines.Skip(1).ToArray()));
        }

        return results;
    }

    private record ParseRecord(string Header, IReadOnlyList<string> Body);
}
