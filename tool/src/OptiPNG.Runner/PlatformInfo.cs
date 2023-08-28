﻿using System.Runtime.InteropServices;

namespace OptiPNG.Runner;

public readonly struct PlatformInfo
{
    public OperatingSystemId OsId { get; init; }
    public Architecture RuntimeCpuArchitecture { get; init; }
    public string EnvPathSeparator { get; init; }
}