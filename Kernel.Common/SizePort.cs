using System.ComponentModel;

namespace Kernel.Common;

public enum SizePort : ulong
{
    [Description("64 байта")] Size64B = 6,
    [Description("128 байт")] Size128B = 7,
    [Description("256 байт")] Size256B = 8,
    [Description("512 байт")] Size512B = 9,
    [Description("1 КБ")] Size1KB = 10,
    [Description("4 КБ")] Size4KB = 12,
    [Description("8 КБ")] Size8KB = 13,
    [Description("16 КБ")] Size16KB = 14,
    [Description("64 КБ")] Size64KB = 16,
    [Description("128 КБ")] Size128KB = 17,
    [Description("256 КБ")] Size256KB = 18,
    [Description("512 КБ")] Size512KB = 19,
}
