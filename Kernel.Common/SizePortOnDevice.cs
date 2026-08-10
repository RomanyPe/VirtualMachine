using System.ComponentModel;

namespace Kernel.Common;

public enum SizePortOnDevice : uint
{
    [Description("4 байта")] Size4B = 1U << 2,
    [Description("8 байт")] Size8B = 1U << 3,
    [Description("16 байт")] Size16B = 1U << 4,
    [Description("32 байта")] Size32B = 1U << 5,
}