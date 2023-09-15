using System.Reflection;
using System.Runtime.InteropServices;

namespace OptiPNG.Tool;

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

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is null)
            {
                // We won't be able to find the packaged vendor binaries, thus we can't provide a vendor path
                return null;
            }

            var parent = new FileInfo(entryAssembly.Location).DirectoryName;
            if (parent is null)
            {
                // We won't be able to find the packaged vendor binaries, thus we can't provide a vendor path
                return null;
            }


            var newPath = new List<string>
            {
                parent,
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

            return $"{info.EnvPathSeparator}{Path.Join(newPath.ToArray())}";
        }
        catch
        {
            // Unknown platform / error inspecting platform, thus we can't provide a vendor path
            return null;
        }
    }
}
