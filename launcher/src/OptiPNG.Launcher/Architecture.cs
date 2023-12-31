﻿namespace OptiPNG.Launcher;

/// <summary>
/// Enum for CPU architecture. Should be kept in sync with
/// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/InteropServices/Architecture.cs.
/// </summary>
public enum Architecture
{
    /// <summary>
    /// An Intel-based 32-bit processor architecture.
    /// </summary>
    X86,
    /// <summary>
    /// An Intel-based 64-bit processor architecture.
    /// </summary>
    X64,
    /// <summary>
    /// A 32-bit ARM processor architecture.
    /// </summary>
    /// <remarks>
    /// This value indicates ARMv7 base instructions, VFPv3 floating point support and registers, and Thumb2 compact instruction set.
    /// </remarks>
    Arm,
    /// <summary>
    /// A 64-bit ARM processor architecture.
    /// </summary>
    Arm64,
    /// <summary>
    /// The WebAssembly platform.
    /// </summary>
    Wasm,
    /// <summary>
    /// A S390x platform architecture.
    /// </summary>
    S390x,
    /// <summary>
    /// A LoongArch64 processor architecture.
    /// </summary>
    LoongArch64,
    /// <summary>
    /// A 32-bit ARMv6 processor architecture.
    /// </summary>
    /// <remarks>
    /// This value indicates ARMv6 base instructions, VFPv2 floating point support and registers, hard-float ABI, and no compact instruction set.
    /// </remarks>
    Armv6,
    /// <summary>
    /// A PowerPC 64-bit (little-endian) processor architecture.
    /// </summary>
    Ppc64le,
    /// <summary>
    /// A RiscV 64-bit processor architecture.
    /// </summary>
    /// <remarks>
    /// This value indicates RV64GC set of extensions.
    /// </remarks>
    RiscV64,
}
