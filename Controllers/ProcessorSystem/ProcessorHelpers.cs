using Kernel.Common;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static Kernel.ProcessorSystem.Processor;

namespace Kernel.ProcessorSystem;

public static class ProcessorHelpers
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int TryContinueAfterStatus(ResultInstruction dat, ulong ip, Lock regLock)
    {
        if (dat.BiosStatus == BiosStatus.EndProgramm)
        {
            lock (regLock)
            {
                LoggerProvider.Info($"\n [INFO] Программа успешно завершила работу (HALT). Ip [{ip}]");
                return 0;
            }
        }

        string errorMessage = dat.BiosStatus switch
        {
            BiosStatus.ReadViolation => $"Попытка чтения из защищенной области памяти, адрес [{dat.Adress}]",
            BiosStatus.AlignmentFault => $"Попытка прочесть целочисленные данные по невыравненной памяти, адрес [{dat.Adress}]",
            BiosStatus.SegmentationFault => $"Ошибка выхода за границы ОЗУ, по обращению, адрес [{dat.Adress}]",
            BiosStatus.NotImplementedOpCode => $"Неизвестный код операции OpCode [{dat.Adress}]",
            BiosStatus.NullDeviceInput => $"Попытка записать данные в отсутствующий девайс, адрес обращения [{dat.Adress}],\n проверьте таблицу секторов портов, формула: [Adress / AdressPerSector]",
            BiosStatus.NullDeviceOutput => $"Попытка прочесть данные из отсутсвующего девайса, адрес обращения [{dat.Adress}],\n проверьте таблицу секторов портов, формула: [Adress / AdressPerSector]",
            _ => $"НЕПРЕДВИДЕННАЯ ОШИБКА СИМУЛЯЦИИ адрес [{dat.Adress}]"
        };

        return OutputLog(dat.BiosStatus, regLock, errorMessage, ip) ? 1 : 0;
    }


    public static LogLevel TypeNotification(BiosStatus status) => status switch
    {
        BiosStatus.Success => LogLevel.Log,
        BiosStatus.SegmentationFault => LogLevel.Error,
        BiosStatus.AlignmentFault => LogLevel.Error,
        BiosStatus.ReadViolation => LogLevel.Error,
        BiosStatus.EndProgramm => LogLevel.Log,
        BiosStatus.NotImplementedOpCode => LogLevel.Error,
        BiosStatus.NullDeviceInput => LogLevel.Warning,
        BiosStatus.NullDeviceOutput => LogLevel.Error,
        _ => LogLevel.Error
    };

    public static bool CanContinue(BiosStatus status) => status switch
    {
        BiosStatus.ReadViolation => false,
        BiosStatus.AlignmentFault => false,
        BiosStatus.SegmentationFault => false,
        BiosStatus.NotImplementedOpCode => false,
        BiosStatus.NullDeviceInput => true,
        BiosStatus.NullDeviceOutput => false,
        BiosStatus.EndProgramm => false,
        _ => false
    };

    public static bool OutputLog(BiosStatus status, Lock regLock, string text, ulong ip)
    {
        bool can = CanContinue(status);
        var logLevel = TypeNotification(status);
        return logLevel switch
        {
            LogLevel.Log => LogNotification(regLock, text, ip, can),
            LogLevel.Warning => WarningNotification(regLock, text, ip, can),
            LogLevel.Error => ErrorNotification(regLock, text, ip, can),
            _ => NoneNotification(regLock, text, can),
        };
    }
    public static bool ErrorNotification(Lock regLock, string errorMessage, ulong ip, bool canContinue)
    {
        lock (regLock)
        {
            LoggerProvider.Error($"\n [КРИТИЧЕСКАЯ ОШИБКА ПРОЦЕССОРА] {errorMessage}, Ip [{ip}]");
        }
        return canContinue;
    }

    public static bool WarningNotification(Lock regLock, string errorMessage, ulong ip, bool canContinue)
    {
        lock (regLock)
        {
            LoggerProvider.Warning($"\n [ПРЕДУПРЕЖДЕНИЕ РАБОТЫ ПРОГРАММЫ] {errorMessage}, Ip [{ip}]");
        }
        return canContinue;
    }

    public static bool LogNotification(Lock regLock, string errorMessage, ulong ip, bool canContinue)
    {
        lock (regLock)
        {
            LoggerProvider.Info($"\n [ВЫПОЛНЕНИЕ УСПЕШНО] {errorMessage}, Ip [{ip}]");
        }
        return canContinue;
    }

    public static bool NoneNotification(Lock regLock, string text, bool canContinue)
    {
        lock (regLock)
        {
            LoggerProvider.Info(text);
        }
        return canContinue;
    }
}