namespace OptiPNG.Runner;

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

        // TODO: Refactor to avoid needing to pass in $PATH here
        path = _vendorMapper.Map(path);

        if (path is not null)
        {
            didModify = true;
            Environment.SetEnvironmentVariable("PATH", path);
        }

        return didModify;
    }
}
