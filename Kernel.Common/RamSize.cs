using System.ComponentModel;

namespace Kernel.Common;

public enum RamSize : ulong
{
    Size128B = 1U << 7,
    Size256B = 1U << 8,
    Size512B = 1U << 9,

    Size1KB = 1U << 10,
    Size4KB = 1U << 12,
    Size8KB = 1U << 13,
    Size16KB = 1U << 14,
    Size64KB = 1U << 16,
    Size128KB = 1U << 17,
    Size256KB = 1U << 18,
    Size512KB = 1U << 19,

    Size1MB = 1U << 20,
    Size4MB = 1U << 22,
    Size8MB = 1U << 23,
    Size16MB = 1U << 24,
    Size32MB = 1U << 25,
    Size64MB = 1U << 26,
    Size128MB = 1U << 27,
}
