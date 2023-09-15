namespace OptiPNG.Tool;

internal class VendorPathAppender
{
    private readonly VendorMapper _vendorMapper;

    public VendorPathAppender(VendorMapper vendorMapper)
    {
        _vendorMapper = vendorMapper;
    }

    public bool TryAppend()
    {
        var didModify = false;

        var path = Environment.GetEnvironmentVariable("PATH");

        var vendorPath = _vendorMapper.Map();

        if (vendorPath is not null)
        {
            didModify = true;

            path = $"{path}{vendorPath}";
            Environment.SetEnvironmentVariable("PATH", path);
        }

        return didModify;
    }
}
