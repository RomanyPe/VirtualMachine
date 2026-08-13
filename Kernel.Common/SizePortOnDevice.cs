using System.ComponentModel;

namespace Kernel.Common;

public enum SizePortOnDevice : uint
{
    [Description("4 байта")] Size4B = 2,
    [Description("8 байт")] Size8B = 3,
    [Description("16 байт")] Size16B = 4,
    [Description("32 байта")] Size32B = 5,
}