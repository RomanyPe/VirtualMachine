using System.IO;

namespace ASM_gen.ProjectManage.Data;

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

    /// <summary> Базовая директория приложения </summary>
    private static readonly string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;

    public static readonly string CurrentBinPath = Path.Combine(CurrentDir, BinPath);

    public static readonly string CurrentSysDataPath = Path.Combine(CurrentDir, SysDataPath);
    public static readonly string CurrentTemplatesPath = Path.Combine(CurrentSysDataPath, TemplatesPath);

    public static readonly string CurrentLocalDataPath = Path.Combine(CurrentDir, LocalDataPath);
    public static readonly string CurrentMetaDataPath = Path.Combine(CurrentLocalDataPath, MetaDataPath);
    public static readonly string CurrentProjectsDataPath = Path.Combine(CurrentMetaDataPath, ProjectsDataPath);

    public static readonly string CurrentUserProjectsPath = Path.Combine(CurrentDir, UserProjectsPath);
    public static readonly string SharedIncludePath = Path.Combine(CurrentLocalDataPath, IncludePath);
}
