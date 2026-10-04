using Kernel.Common;
using Kernel.Contracts;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Project;
using VMApplication.Project.IO;

const string sourceText1 =
    """
    main:
        LDI  r1, 0x41          // r1 = данные ('A')
        LDI  r2, 0x10          // r2 = адрес порта Data
        OUT  r1, r2            // OUT regSrc=r1, regPort=r2

        LDI  r1, 1             // данные для Flush
        LDI  r2, 0x12          // адрес порта Flush
        OUT  r1, r2

        END
    """;
const string sourceText2 =
    """
    main:
        LDI  r1, 0x48          // 'H'  — данные
        LDI  r2, 0x10          // адрес Data
        OUT  r1, r2

        LDI  r1, 0x65          // 'e'
        OUT  r1, r2            // адрес уже в r2, переиспользуем

        LDI  r1, 0x6C          // 'l'
        OUT  r1, r2

        LDI  r1, 0x6C          // 'l'
        OUT  r1, r2

        LDI  r1, 0x6F          // 'o'
        OUT  r1, r2

        LDI  r1, 0x21          // '!'
        OUT  r1, r2

        LDI  r1, 0x0A          // '\n'
        OUT  r1, r2

        LDI  r1, 1             // flush
        LDI  r2, 0x12
        OUT  r1, r2

        END
    """;
const string sourceText3 =
    """
    main:
        LDI  r3, 0x00          // r3 = адрес input Data (сектор 0)
        LDI  r5, 0x10          // r5 = адрес console Data (сектор 1)

    loop:
        IN   r4, r3            // IN regDest=r4, regPort=r3  → r4 = port[r3]
        LDI  r6, 0
        CMP  r4, r6
        JZ   done              // если 0 — на выход

        OUT  r4, r5            // OUT regSrc=r4 (данные), regPort=r5 (адрес)  ← ключ!
        JMP  loop

    done:
        LDI  r1, 1
        LDI  r2, 0x12
        OUT  r1, r2
        END
    """;
const string sourceText4 =
    """
    main:
        LDI  r1, 0x10          // console Data
        LDI  r2, 0x58          // 'X'
        OUT  r2, r1

    wait:
        LDI  r1, 0x11          // console Status
        IN   r1, r2            // r2 = Status (0 = пусто, 1 = есть данные)
        LDI  r3, 0
        CMP  r2, r3
        JNZ  wait              // ждём, пока очередь опустеет

        LDI  r1, 0x12          // Flush (на всякий случай)
        LDI  r2, 1
        OUT  r2, r1

        END
    """;

const string sourceText5 =
    """
        LDI  r2, 0x10;
        LDI  r3, 0x12;

        LDI  r1, 0x41;
        OUT  r1, r2;

        LDI  r4, after_jmp;
        JMP_IND r4;

        LDI  r1, 0x58;
        OUT  r1, r2;
        LDI  r1, 0x58;
        OUT  r1, r2;

    after_jmp:
        LDI  r1, 0x42;
        OUT  r1, r2;

        LDI  r5, print_C;
        CALL_IND r5;

        LDI  r1, 0x44;
        OUT  r1, r2;

        LDI  r1, 1;
        OUT  r1, r3;
        END

    print_C:
        LDI  r1, 0x43;
        OUT  r1, r2;

        RET;
    """;

const string bug =
    """
    LDI r0, 18446744073709551;
    JMP_IND r0;
    END;
    """;

using var host = VMHostFactory.CreateDefault();
var lfPolicy = new LoggingFaultPolicy(Console.Out);

var device = host.Emulator.CreateDevice(
    ramSize: RamSize.Size128MB,
    biosSize: RamSize.Size128B, 
    processorFaultPolicy: lfPolicy,
    name: "ConsoleVM"
);

var iostream = new QueuedIOStream(Console.Out, PortCharEncoding.Utf8);
host.Emulator.AddDevice(device, 0);
host.Emulator.AddDevice(iostream, "console", 1);

Span<string> sourseCodes = [bug];

for (int i = 0; i < sourseCodes.Length; i++)
{
    var source = sourseCodes[i];
    var resIl = host.Project.Compiler.CompileToIL(source, 0, true, SourceLanguage.Asm);
    var resCompile = host.Project.Compiler.Compile(resIl);

    if (!resCompile.Success)
    {
        foreach (var err in resCompile.Errors!)
        {
            Console.WriteLine(err);
        }
        return;
    }


    LaunchModeDevice? load = device.TryFastLoadProgram(resCompile.Program, 0, out var error);
    if (load == null)
    {
        Console.WriteLine(error!);
        return;
    }
    load.Launch();
}

class LoggingFaultPolicy(TextWriter output) : IProcessorFaultPolicy
{
    private readonly TextWriter _out = output;

    public bool ShouldContinue(in ProcessorFault fault)
    {
        _out.WriteLine(
            $"[FAULT] status={fault.Status}, " +
            $"data=0x{fault.FaultData:X}, " +
            $"ip=0x{fault.InstructionPointer:X}, " +
            $"op={fault.OpCode}");
        _out.Flush();

        return false; // останавливаем симуляцию
    }
}