# OptiPNG dotnet tool & MSBuild task

[![Build](https://img.shields.io/github/actions/workflow/status/MattKotsenas/optipng-dotnet-tool/ci.yml?branch=main)](https://github.com/MattKotsenas/optipng-dotnet-tool/actions)

[OptiPNG] is a PNG optimizer that recompresses image files to a smaller size, without losing any information. It also
converts external formats (BMP, GIF, PNM and TIFF) to optimized PNG, and performs PNG integrity checks and corrections.

This project packages the `optipng` utility as a dotnet tool so it can be easily installed, either globally or locally
to a project using [dotnet tool manifests][dotnet-tool-manifest]. Libraries for Windows, Linux, and macOS are provided.

This project also publishes a custom MSBuild task to make it easy to ensure all images are optimized. In order to support
deterministic and incremental builds, the MSBuild task follows the [read only source tree][google-read-only-source]
guidance. As a result, the task will not optimize images on-the-fly, but rather verify the images are optimized and
fail the build if not.

## Install

Install the tool as a .NET CLI tool like this:

```powershell
dotnet tool install optipng.tool
```

## Usage

### dotnet tool

Run the tool like this:

`dotnet optipng [options] files ...`

where all arguments are passed directly to the optipng tool.

#### Using a custom OptiPNG

By default, the tool includes precompiled versions of optipng for Windows, Linux, and macOS operating systems and x86
and amd64/x64 architectures. If one of the packaged versions doesn't work for your platform / architecture, you can
put a compatible version on `$PATH` and the tool will use that version instead. If this happens to you, also file an
issue with your platform details so I can provide precompiled versions for popular platforms.

### MSBuild task

`// TODO`

## Licenses

OptiPNG available under the zlib license; this code is available under the [MIT license][license].

[optipng]: https://optipng.sourceforge.net/
[dotnet-tool-manifest]: https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use
[google-read-only-source]: https://android.googlesource.com/platform/build/soong/+/master/docs/best_practices.md#read-only-source-tree
[license]: ./LICENSE
