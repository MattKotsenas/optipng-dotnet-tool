using System.Runtime.InteropServices;

namespace OptiPNG.Launcher;

public class PlatformInspector
{
    public PlatformInfo Inspect()
    {
#if NETFRAMEWORK
        return new PlatformInfo
        {
            OsId = OperatingSystemId.Windows,
            RuntimeCpuArchitecture = Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86,
            EnvPathSeparator = ";",
        };
#elif NETCOREAPP
        var arch = RuntimeInformation.OSArchitecture;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.Windows,
                RuntimeCpuArchitecture = Enum.Parse<Architecture>(arch.ToString()),
                EnvPathSeparator = ";",
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.Linux,
                RuntimeCpuArchitecture = Enum.Parse<Architecture>(arch.ToString()),
                EnvPathSeparator = ":",
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return new PlatformInfo
            {
                OsId = OperatingSystemId.MacOS,
                RuntimeCpuArchitecture = Enum.Parse<Architecture>(arch.ToString()),
                EnvPathSeparator = ":"
            };
        }
        else
        {
            throw new PlatformNotSupportedException($"Platform with Operating System information `{Environment.OSVersion}` not supported");
        }
#endif
    }
}