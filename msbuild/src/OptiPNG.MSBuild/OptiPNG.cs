using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OptiPNG.MSBuild;

/// <summary>
/// A task for invoking the OptiPNG tool.
/// </summary>
public class OptiPNG : ToolTask
{
    private static readonly string ExeName = "optipng"; // TODO: Replace with wrapper's name

    /// <summary>
    /// The current working directory for optipng.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Override the path to the optipng executable. When empty, the task assumes the tool is available on $PATH.
    /// </summary>
    public string? ExecutablePath { get; set; }

    protected override string ToolName => ExeName;

    [Required]
    public ITaskItem[] Files { get; set; } = Array.Empty<ITaskItem>();

    protected override string GenerateFullPathToTool()
    {
        if (!string.IsNullOrEmpty(ExecutablePath))
        {
            return ExecutablePath!;
        }

        return ExeName;
    }

    protected override string GenerateCommandLineCommands()
    {
        CommandLineBuilder builder = new CommandLineBuilder();

        // TODO: Fill in options based on wrapper's switches.

        return builder.ToString();
    }
}
