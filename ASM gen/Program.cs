using ASM_gen.ProjectManage.Managers.Static;
using ASM_gen.StartWindow;
using Serilog;

namespace ASM_gen;

internal static class Program
{
    [STAThread] // Критически важно для WPF
    public static void Main()
    {
        // 1. Инициализируем Serilog
        Log.Logger = new LoggerConfiguration().WriteTo.File("logs/app-log.txt").CreateLogger();
        DirectoryManager.InitializeDirectories();

        try
        {
            Log.Information("Приложение запускается...");

            // 2. Создаем экземпляр вашего WPF-приложения
            App app = new();

            // 4. Создаем и запускаем главное окно
            MainWindow mainWindow = new();
            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Критическая ошибка при запуске приложения ASM_gen.");
        }
        finally
        {
            Log.Information("Приложение завершило работу.");
            Log.CloseAndFlush();
        }
    }
}