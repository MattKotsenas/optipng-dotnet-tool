using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;

namespace OptiPNG.MSBuild;

// TODO: Support response files to avoid max command length issues
// https://learn.microsoft.com/en-us/troubleshoot/windows-client/shell-experience/command-line-string-limitation

/// <summary>
/// A task for invoking the OptiPNG tool.
/// </summary>
public class OptiPNG : ToolTask
{
    protected override string ToolName => "OptiPNG.Verifier.exe";

    [Required]
    public ITaskItem[] Files { get; set; } = Array.Empty<ITaskItem>();

    public OptiPNG()
    {
        LogStandardErrorAsError = true;
    }

    protected override string GenerateFullPathToTool()
    {
        var assembly = typeof(OptiPNG).Assembly.Location;
        var dir = new FileInfo(assembly).Directory!.FullName;

        var path = Path.Combine(dir, ToolExe);
        path = Path.GetFullPath(path);

        return path;
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
