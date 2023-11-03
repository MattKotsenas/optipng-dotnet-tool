namespace OptiPNG.Verifier.UnitTests;

public class Given_an_optimized_file
{
    private readonly OutputParser _outputParser = new();
    private const string _fileName = @"C:\optimized.png";
    private const string _output =
@$"** Processing: {_fileName}
400x250 pixels, 3x8 bits/pixel, RGB
Input IDAT size = 161908 bytes
Input file size = 164675 bytes

Trying:
  zc = 9  zm = 8  zs = 0  f = 0
  zc = 9  zm = 8  zs = 1  f = 0
  zc = 1  zm = 8  zs = 2  f = 0
  zc = 9  zm = 8  zs = 3  f = 0
  zc = 9  zm = 8  zs = 0  f = 5
  zc = 9  zm = 8  zs = 1  f = 5		IDAT size = 161908
  zc = 1  zm = 8  zs = 2  f = 5
  zc = 9  zm = 8  zs = 3  f = 5
                               

{_fileName} is already optimized.

";


    [Fact]
    public void The_OutputParser_returns_success_and_no_output()
    {
        ParseResult result = _outputParser.Parse(new[] { _fileName }, _output);

        result.Succeeded.Should().BeTrue();
        result.Output.Should().BeEmpty();
    }
}

public class Given_an_unoptimized_file
{
    private readonly OutputParser _outputParser = new();
    private const string _fileName = @"C:\unoptimized.png";
    private const string _output =
@$"** Processing: {_fileName}
400x250 pixels, 4x8 bits/pixel, RGB+alpha
Reducing image to 3x8 bits/pixel, RGB
Input IDAT size = 204268 bytes
Input file size = 207071 bytes

Trying:
  zc = 9  zm = 8  zs = 0  f = 0
  zc = 9  zm = 8  zs = 1  f = 0
  zc = 1  zm = 8  zs = 2  f = 0
  zc = 9  zm = 8  zs = 3  f = 0
  zc = 9  zm = 8  zs = 0  f = 5		IDAT size = 170020
  zc = 9  zm = 8  zs = 1  f = 5		IDAT size = 161908
  zc = 1  zm = 8  zs = 2  f = 5
  zc = 9  zm = 8  zs = 3  f = 5
                               

Selecting parameters:
  zc = 9  zm = 8  zs = 1  f = 5		IDAT size = 161908

No output: simulation mode.

";


    [Fact]
    public void The_OutputParser_returns_failure_and_outputs_the_file_name()
    {
        ParseResult result = _outputParser.Parse(new[] { _fileName }, _output);

        result.Failed.Should().BeTrue();

        result.Output.Count.Should().Be(1);
        result.Output.Single().Should().StartWith($"'{_fileName}' is not optimized");
    }
}