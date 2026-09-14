using ASM_gen.ProjectManage.Managers.Static;
using ASM_gen.StartWindow;
using Serilog;
using System.IO;
using VMApplication;
using VMApplication.Project;

namespace ASM_gen;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.File("logs/app-log.txt").CreateLogger();
        KernelWarmup.WarmupAll();
        try
        {
            DirManager.InitializeDirectories();
            Log.Information("Приложение запускается...");

            App app = new();

            MainWindow mainWindow = new();
            app.Run(mainWindow);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Критическая ошибка при запуске приложения");
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

    private class PathSystem(string projectPath, string include) : IProjectFilesConfig
    {
        public string ProjectPath => projectPath;
        public string IncludePath => include;

        public string[] ExtensionsAsm => [".asm", ".soe", ".vma"];

        public string[] ExtensionsMiniC => [".c", ".mic", ".cll"];
    }

    public static IProjectFilesConfig ProjectSystemPaths(string path) => new PathSystem(path, sharedIncludePath);
    /// <summary> Базовая директория приложения </summary>
    private static readonly string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;

    private static readonly string currentBinPath = Path.Combine(CurrentDir, "bin");

    private static readonly string currentSysDataPath = Path.Combine(CurrentDir, "sysData");
    private static readonly string currentTemplatesPath = Path.Combine(CurrentSysDataPath, "templates");

    private static readonly string currentLocalDataPath = Path.Combine(CurrentDir, "localData");
    private static readonly string currentMetaDataPath = Path.Combine(CurrentLocalDataPath, "metaData");
    private static readonly string currentProjectsDataPath = Path.Combine(CurrentMetaDataPath, "pojectsData");

    private static readonly string currentUserProjectsPath = Path.Combine(CurrentDir, "userProjects");
    private static readonly string sharedIncludePath = Path.Combine(CurrentLocalDataPath, "include");

    public static string CurrentBinPath => currentBinPath;
    public static string CurrentSysDataPath => currentSysDataPath;
    public static string CurrentTemplatesPath => currentTemplatesPath;
    public static string CurrentLocalDataPath => currentLocalDataPath;
    public static string CurrentMetaDataPath => currentMetaDataPath;
    public static string CurrentProjectsDataPath => currentProjectsDataPath;
    public static string CurrentUserProjectsPath => currentUserProjectsPath;
    public static string SharedIncludePath => sharedIncludePath;
}
