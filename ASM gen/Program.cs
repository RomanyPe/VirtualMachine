using ASM_gen.StartWindow;
using Serilog;
using System.IO;
using VMApplication;

namespace ASM_gen;

internal static class Program
{
    [STAThread] // Критически важно для WPF
    public static void Main()
    {
        // 1. Инициализируем Serilog
        Log.Logger = new LoggerConfiguration().WriteTo.File("logs/app-log.txt").CreateLogger();

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

public static class AppPaths
{
    public const string ExtensionProj = "*.vmproj";

    private const string BinPath = "bin";
    private const string SysDataPath = "sysData";
    private const string TemplatesPath = "templates";
    private const string LocalDataPath = "localData";
    private const string MetaDataPath = "metaData";
    private const string ProjectsDataPath = "projectsData";
    private const string UserProjectsPath = "userProjects";
    private const string IncludePath = "include";

    private class PathSystem(string projectPath, string include) : IProjectPaths
    {
        public string ProjectPath => projectPath;

        public string IncludePath => include;
    }

    public static IProjectPaths ProjectSystemPaths(string path) => new PathSystem(path, sharedIncludePath);
    /// <summary> Базовая директория приложения </summary>
    private static readonly string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;

    private static readonly string currentBinPath = Path.Combine(CurrentDir, BinPath);

    private static readonly string currentSysDataPath = Path.Combine(CurrentDir, SysDataPath);
    private static readonly string currentTemplatesPath = Path.Combine(CurrentSysDataPath, TemplatesPath);

    private static readonly string currentLocalDataPath = Path.Combine(CurrentDir, LocalDataPath);
    private static readonly string currentMetaDataPath = Path.Combine(CurrentLocalDataPath, MetaDataPath);
    private static readonly string currentProjectsDataPath = Path.Combine(CurrentMetaDataPath, ProjectsDataPath);

    private static readonly string currentUserProjectsPath = Path.Combine(CurrentDir, UserProjectsPath);
    private static readonly string sharedIncludePath = Path.Combine(CurrentLocalDataPath, IncludePath);

    public static string CurrentBinPath => currentBinPath;
    public static string CurrentSysDataPath => currentSysDataPath;
    public static string CurrentTemplatesPath => currentTemplatesPath;
    public static string CurrentLocalDataPath => currentLocalDataPath;
    public static string CurrentMetaDataPath => currentMetaDataPath;
    public static string CurrentProjectsDataPath => currentProjectsDataPath;
    public static string CurrentUserProjectsPath => currentUserProjectsPath;
    public static string SharedIncludePath => sharedIncludePath;
}
