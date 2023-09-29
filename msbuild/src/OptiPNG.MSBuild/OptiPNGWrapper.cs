using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace OptiPNG.MSBuild;

// TODO: Support response files to avoid max command length issues
// https://learn.microsoft.com/en-us/troubleshoot/windows-client/shell-experience/command-line-string-limitation

/// <summary>
/// A task for invoking the OptiPNG tool.
/// </summary>
public class OptiPNGWrapper : ToolTask
{
    private static readonly string ExeName = "OptiPNG.Verifier";

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

        // TODO: Pull wrapper from NuGet package.

        return ExeName;
    }

    protected override string GenerateCommandLineCommands()
    {
        var builder = new CommandLineBuilder();

        foreach (var file in Files)
        {
            builder.AppendFileNameIfNotNull(file);
        }

        return builder.ToString();
    }
}
