using System.Diagnostics;

namespace Kernel.Common;

/// <summary>
/// Total port address space size, stored as log₂(bytes).
/// All instances are created via static factory properties; the only
/// way to obtain an invalid instance is <c>default(PortSize)</c>.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PortSize : IEquatable<PortSize>, IComparable<PortSize>
{
    public const byte MinLog2 = 6;   // 64 B
    public const byte MaxLog2 = 19;  // 512 KiB

    /// <summary>log₂ of the size in bytes.</summary>
    public byte Log2 { get; }

    /// <summary>Size in bytes (2^Log2).</summary>
    public ulong Bytes => 1UL << Log2;

    /// <summary>False for <c>default(PortSize)</c>.</summary>
    public bool IsValid => Log2 is >= MinLog2 and <= MaxLog2;

    private PortSize(byte log2) => Log2 = log2;

    public static PortSize B64 => new(6);
    public static PortSize B128 => new(7);
    public static PortSize B256 => new(8);
    public static PortSize B512 => new(9);
    public static PortSize KB1 => new(10);
    public static PortSize KB4 => new(12);
    public static PortSize KB8 => new(13);
    public static PortSize KB16 => new(14);
    public static PortSize KB64 => new(16);
    public static PortSize KB128 => new(17);
    public static PortSize KB256 => new(18);
    public static PortSize KB512 => new(19);

    public bool Equals(PortSize other) => Log2 == other.Log2;
    public override bool Equals(object? obj) => obj is PortSize p && Equals(p);
    public override int GetHashCode() => Log2;
    public int CompareTo(PortSize other) => Log2.CompareTo(other.Log2);
    public override string ToString() => IsValid ? $"{Bytes} B (2^{Log2})" : "PortSize(invalid)";

    public static bool operator ==(PortSize a, PortSize b) => a.Equals(b);
    public static bool operator !=(PortSize a, PortSize b) => !a.Equals(b);
    public static bool operator <(PortSize a, PortSize b) => a.Log2 < b.Log2;
    public static bool operator >(PortSize a, PortSize b) => a.Log2 > b.Log2;
    public static bool operator <=(PortSize a, PortSize b) => a.Log2 <= b.Log2;
    public static bool operator >=(PortSize a, PortSize b) => a.Log2 >= b.Log2;
}

/// <summary>
/// Size of a single device's port block, stored as log₂(bytes).
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct DevicePortSize : IEquatable<DevicePortSize>, IComparable<DevicePortSize>
{
    public const byte MinLog2 = 3;   // 8 B
    public const byte MaxLog2 = 6;   // 64 B

    public byte Log2 { get; }
    public ulong Bytes => 1UL << Log2;
    public bool IsValid => Log2 is >= MinLog2 and <= MaxLog2;

    private DevicePortSize(byte log2) => Log2 = log2;

    public static DevicePortSize B8 => new(3);
    public static DevicePortSize B16 => new(4);
    public static DevicePortSize B32 => new(5);
    public static DevicePortSize B64 => new(6);

    public bool Equals(DevicePortSize other) => Log2 == other.Log2;
    public override bool Equals(object? obj) => obj is DevicePortSize p && Equals(p);
    public override int GetHashCode() => Log2;
    public int CompareTo(DevicePortSize other) => Log2.CompareTo(other.Log2);
    public override string ToString() => IsValid ? $"{Bytes} B (2^{Log2})" : "DevicePortSize(invalid)";

    public static bool operator ==(DevicePortSize a, DevicePortSize b) => a.Equals(b);
    public static bool operator !=(DevicePortSize a, DevicePortSize b) => !a.Equals(b);
    public static bool operator <(DevicePortSize a, DevicePortSize b) => a.Log2 < b.Log2;
    public static bool operator >(DevicePortSize a, DevicePortSize b) => a.Log2 > b.Log2;
    public static bool operator <=(DevicePortSize a, DevicePortSize b) => a.Log2 <= b.Log2;
    public static bool operator >=(DevicePortSize a, DevicePortSize b) => a.Log2 >= b.Log2;
}


[DebuggerDisplay("{ToString(),nq}")]
public readonly struct RamSize : IEquatable<RamSize>, IComparable<RamSize>
{
    public const byte MinLog2 = 6;   // 64 B
    public const byte MaxLog2 = 29;  // 512 MiB

    /// <summary>log₂ of the size in bytes.</summary>
    public byte Log2 { get; }

    /// <summary>Size in bytes (2^Log2).</summary>
    public ulong Bytes => 1UL << Log2;

    /// <summary>False for <c>default(RamSize)</c>.</summary>
    public bool IsValid => Log2 is >= MinLog2 and <= MaxLog2;

    private RamSize(byte log2) => Log2 = log2;

    public static RamSize B64 => new(6);
    public static RamSize B128 => new(7);
    public static RamSize B256 => new(8);
    public static RamSize B512 => new(9);
    public static RamSize KB1 => new(10);
    public static RamSize KB4 => new(12);
    public static RamSize KB8 => new(13);
    public static RamSize KB16 => new(14);
    public static RamSize KB64 => new(16);
    public static RamSize KB128 => new(17);
    public static RamSize KB256 => new(18);
    public static RamSize KB512 => new(19);
    public static RamSize MB1 => new(20);
    public static RamSize MB4 => new(22);
    public static RamSize MB8 => new(23);
    public static RamSize MB16 => new(24);
    public static RamSize MB32 => new(25);
    public static RamSize MB64 => new(26);
    public static RamSize MB128 => new(27);
    public static RamSize MB256 => new(28);
    public static RamSize MB512 => new(29);

    public bool Equals(RamSize other) => Log2 == other.Log2;
    public override bool Equals(object? obj) => obj is RamSize r && Equals(r);
    public override int GetHashCode() => Log2;
    public int CompareTo(RamSize other) => Log2.CompareTo(other.Log2);
    public override string ToString() =>
        IsValid ? $"{Bytes} B (2^{Log2})" : "RamSize(invalid)";

    public static bool operator ==(RamSize a, RamSize b) => a.Equals(b);
    public static bool operator !=(RamSize a, RamSize b) => !a.Equals(b);
    public static bool operator <(RamSize a, RamSize b) => a.Log2 < b.Log2;
    public static bool operator >(RamSize a, RamSize b) => a.Log2 > b.Log2;
    public static bool operator <=(RamSize a, RamSize b) => a.Log2 <= b.Log2;
    public static bool operator >=(RamSize a, RamSize b) => a.Log2 >= b.Log2;
}