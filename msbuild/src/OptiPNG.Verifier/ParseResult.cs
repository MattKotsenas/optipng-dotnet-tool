namespace OptiPNG.Verifier;

public class ParseResult
{
    public bool Succeeded { get; }
    public bool Failed => !Succeeded;
    public IReadOnlyCollection<string> Output { get; }

    public ParseResult(bool succeeded, IReadOnlyCollection<string> output)
    {
        Succeeded = succeeded;
        Output = output;
    }
}