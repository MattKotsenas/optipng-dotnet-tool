using System.Runtime.InteropServices;

namespace OptiPNG.Launcher;

internal class PlatformInspector
{
    public PlatformInfo Inspect()
    {
        var arch = RuntimeInformation.OSArchitecture;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.Windows,
                RuntimeCpuArchitecture = arch,
                EnvPathSeparator = ";",
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.Linux,
                RuntimeCpuArchitecture = arch,
                EnvPathSeparator = ":",
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
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