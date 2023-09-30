using System.Reflection;
using System.Runtime.InteropServices;

namespace OptiPNG.Launcher;

internal class VendorMapper
{
    private readonly PlatformInspector _inspector;

    public VendorMapper(PlatformInspector inspector)
    {
        _inspector = inspector;
    }

    public string? Map()
    {
        try
        {
            var info = _inspector.Inspect();

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (baseDirectory is null)
            {
                // We won't be able to find the packaged vendor binaries, thus we can't provide a vendor path
                return null;
            }

            var newPath = new List<string>
            {
                baseDirectory,
                "vendor",
            };

            if (info.OsId == OperatingSystemId.Windows)
            {
                newPath.Add("win");
            }
            else if (info.OsId == OperatingSystemId.Linux)
            {
                newPath.Add("linux");

                if (info.RuntimeCpuArchitecture == Architecture.X64)
                {
                    newPath.Add("x64");
                }
                else
                {
                    newPath.Add("x86");
                }
            }
            else if (info.OsId == OperatingSystemId.MacOS)
            {
                newPath.Add("osx");
            }
            else
            {
                // Unknown platform, thus we can't provide a vendor path
                return null;
            }

            return $"{info.EnvPathSeparator}{Path.Combine(newPath.ToArray())}";
        }
        catch
        {
            // Unknown platform / error inspecting platform, thus we can't provide a vendor path
            return null;
        }
    }
}
