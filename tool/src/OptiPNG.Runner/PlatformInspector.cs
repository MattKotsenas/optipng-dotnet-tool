using System.Runtime.InteropServices;

namespace OptiPNG.Runner;

public class PlatformInspector
{
    public PlatformInfo Inspect()
    {
        var arch = RuntimeInformation.OSArchitecture;

        if (OperatingSystem.IsWindows())
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.Windows,
                RuntimeCpuArchitecture = arch,
                EnvPathSeparator = ";",
            };
        }
        else if (OperatingSystem.IsLinux())
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.Linux,
                RuntimeCpuArchitecture = arch,
                EnvPathSeparator = ":",
            };
        }
        else if (OperatingSystem.IsMacOS())
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.MacOS,
                RuntimeCpuArchitecture = arch,
                EnvPathSeparator = ":"
            };
        }
        else
        {
            throw new PlatformNotSupportedException($"Platform with Operating System information `{Environment.OSVersion}` not supported");
        }
    }
}