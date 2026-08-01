using Kernel.BiosSystem;
using Kernel.RamSystem;
using static Kernel.ProcessorSystem.Processor;

namespace Kernel.ProcessorSystem
{
    internal static class ProcessorHelpers
    {
        public static bool TryContinueAfterStatus(ResultInstruction dat, ulong ip, Lock regLock)
        {
            if (dat.BiosStatus == BiosStatus.Success) return true;
            if (dat.BiosStatus == BiosStatus.EndProgramm)
            {
                lock (regLock)
                {
                    LoggerProvider.Info($"\n [INFO] Программа успешно завершила работу (HALT). Ip [{ip}]");
                    return false;
                }
            }

            string errorMessage = dat.BiosStatus switch
            {
                BiosStatus.ReadViolation => $"Попытка чтения из защищенной области памяти, адрес [{dat.Adress}]",
                BiosStatus.AlignmentFault => $"Попытка прочесть целочисленные данные по невыравненной памяти, адрес [{dat.Adress}]",
                BiosStatus.SegmentationFault => $"Ошибка выхода за границы ОЗУ, по обращению, адрес [{dat.Adress}]",
                BiosStatus.NotImplementedOpCode => $"Неизвестный код операции OpCode [{dat.Adress}]",
                BiosStatus.NullDeviceOutput => $"Попытка записать данные в отсутствующий девайс, адрес обращения [{dat.Adress}],\n проверьте таблицу секторов портов, формула: [Adress / AdressPerSector]",
                BiosStatus.NullDeviceInput => $"Попытка прочесть данные из отсутсвующего девайса, адрес обращения [{dat.Adress}],\n проверьте таблицу секторов портов, формула: [Adress / AdressPerSector]",
                _ => $"НЕПРЕДВИДЕННАЯ ОШИБКА СИМУЛЯЦИИ адрес [{dat.Adress}]"
            };

            return OutputLog(dat.BiosStatus, regLock, errorMessage, ip);
        }


        public static NotificationType TypeNotification(BiosStatus status) => status switch
        {
            BiosStatus.Success => NotificationType.Log,
            BiosStatus.SegmentationFault => NotificationType.Error,
            BiosStatus.AlignmentFault => NotificationType.Error,
            BiosStatus.ReadViolation => NotificationType.Error,
            BiosStatus.EndProgramm => NotificationType.Log,
            BiosStatus.NotImplementedOpCode => NotificationType.Error,
            BiosStatus.NullDeviceInput => NotificationType.Warning,
            BiosStatus.NullDeviceOutput => NotificationType.Error,
            _ => NotificationType.Error
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
                NotificationType.Log => LogNotification(regLock, text, ip, can),
                NotificationType.Warning => WarningNotification(regLock, text, ip, can),
                NotificationType.Error => ErrorNotification(regLock, text, ip, can),
                _ => NoneNotification(regLock, text, can),
            };
        }
        public static bool ErrorNotification(Lock regLock,string errorMessage, ulong ip, bool canContinue)
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

    public enum NotificationType
    {
        Log,
        Warning,
        Error
    }
}