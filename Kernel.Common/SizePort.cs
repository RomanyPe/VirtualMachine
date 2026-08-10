using System.ComponentModel;

namespace Kernel.Common;

public enum SizePort : ulong
{
    [Description("64 байта")] Size64B = 1U << 6,
    [Description("128 байт")] Size128B = 1U << 7,
    [Description("256 байт")] Size256B = 1U << 8,
    [Description("512 байт")] Size512B = 1U << 9,
    [Description("1 КБ")] Size1KB = 1U << 10,
    [Description("4 КБ")] Size4KB = 1U << 12,
    [Description("8 КБ")] Size8KB = 1U << 13,
    [Description("16 КБ")] Size16KB = 1U << 14,
    [Description("64 КБ")] Size64KB = 1U << 16,
    [Description("128 КБ")] Size128KB = 1U << 17,
    [Description("256 КБ")] Size256KB = 1U << 18,
    [Description("512 КБ")] Size512KB = 1U << 19,
}
