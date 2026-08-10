namespace Kernel.Common;

public enum RegType : byte
{
    rZ = 0x0,

    r0, r1, r2, r3, r4,
    r5, r6, r7, r8, r9,
    r10, r11, r12, r13, r14,
    r15, r16, r17, r18, r19,
    r20, r21,
    rTB = 0x17,
    rCD = 0x18,
    rFL = 0x19,
    rLP = 0x1A,
    rCL = 0x1B,
    rRT = 0x1C,
    rSP = 0x1D,
    rHP = 0x1E,
    rIP = 0x1F,

}
