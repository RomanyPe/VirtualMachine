using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen.StartWindow;

public readonly struct FileReadResult(string fileName, string finalFilePath, ErrorFile errorFile = ErrorFile.None)
{
    public string FileName { get; } = fileName;
    public string FinalFilePath { get; } = finalFilePath;
    public ErrorFile Error { get; } = errorFile;
}

public enum ErrorFile
{
    None = 0,               // Ошибок нет, операция успешна
    UnknownError,           // Непредвиденная или неклассифицированная ошибка

    // --- Ошибки существования и путей ---
    FileNotFound,           // Файл проекта (.vmproj) не найден
    FileNotFoundInProject,  // Файл есть на диске, но не находится в проекте
    DirectoryNotFound,      // Папка с проектами или метаданными удалена или отсутствует
    AlreadyExists,          // Файл или папка с таким именем уже существуют (при создании нового)
    InvalidPathCharacters,  // Путь содержит запрещенные операционной системой символы

    // --- Ошибки доступа и прав ---
    AccessDenied,           // Нет прав администратора на запись/чтение (например, в C:\Program Files)
    FileLocked,             // Файл занят другим процессом (открыт в блокноте, другой IDE или антивирусом)

    // --- Ошибки структуры и парсинга (Специфика IDE) ---
    EmptyFile,              // Файл проекта пустой (нечего читать)
    CorruptedData,          // Нарушена структура метаданных (битый JSON/XML или неверный формат)
    InvalidVersion,         // Версия файла проекта (.vmproj) не поддерживается текущей версией IDE

    // --- Ошибки ограничений ОС ---
    PathTooLong,            // Путь к файлу превышает лимит Windows (обычно 260 символов)
    DiskFull                // На диске закончилось свободное место при попытке сохранения
}

public static class DirectoryManager
{
    private const string ExtensionProj = "*.vmproj";

    private const string BinPath = "bin";
    private const string SysDataPath = "sysData";
    private const string TemplatesPath = "templates";
    private const string LocalDataPath = "localData";
    private const string MetaDataPath = "metaData";
    private const string ProjectsDataPath = "projectsData";
    private const string UserProjectsPath = "userProjects";

    /// <summary> Базовая директория приложения </summary>
    private readonly static string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;

    public readonly static string CurrentBinPath = Path.Combine(CurrentDir, BinPath);

    public readonly static string CurrentSysDataPath = Path.Combine(CurrentDir, SysDataPath);
    public readonly static string CurrentTemplatesPath = Path.Combine(CurrentSysDataPath, TemplatesPath);

    public readonly static string CurrentLocalDataPath = Path.Combine(CurrentDir, LocalDataPath);
    public readonly static string CurrentMetaDataPath = Path.Combine(CurrentLocalDataPath, MetaDataPath);
    public readonly static string CurrentProjectsDataPath = Path.Combine(CurrentMetaDataPath, ProjectsDataPath);

    public readonly static string CurrentUserProjectsPath = Path.Combine(CurrentDir, UserProjectsPath);

    /// <summary>
    /// Создает всю необходимую структуру папок для работы IDE при старте.
    /// </summary>
    public static void InitializeDirectories()
    {
        CreateDir(CurrentTemplatesPath);
        CreateDir(CurrentProjectsDataPath);
        CreateDir(CurrentUserProjectsPath);
    }

    public static async Task<Dictionary<string, FileReadResult>> SearchProjects()
    {
        return await Task.Run(GetProjectFilesHeader);
    }


    private static Dictionary<string, FileReadResult> GetProjectFilesHeader()
    {
        CreateDir(CurrentProjectsDataPath);

        string[] filePaths = Directory.GetFiles(CurrentProjectsDataPath, ExtensionProj);

        Dictionary<string, FileReadResult> projectData = new(filePaths.Length);

        foreach (string path in filePaths)
        {
            string projectName = Path.GetFileNameWithoutExtension(path);
            using var reader = new StreamReader(path);
            string firstLine = reader.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(projectName))
            {
                projectName = Path.GetFileName(path);
            }

            projectData[path] = new(projectName, firstLine);
        }

        return projectData;
    }

    public static ErrorFile TryLoadProjectFirstLine(string path, out string firstLine)
    {
        firstLine = string.Empty;

        if (!File.Exists(path))
            return ErrorFile.FileNotFound;

        try
        {
            using var reader = new StreamReader(path);
            firstLine = reader.ReadLine() ?? string.Empty;
            return firstLine.Length == 0 ? ErrorFile.EmptyFile : ErrorFile.None;
        }
        catch (UnauthorizedAccessException)
        {
            return ErrorFile.AccessDenied;
        }
        catch (IOException ex) when (IsFileLocked(ex))
        {
            return ErrorFile.FileLocked;
        }
        catch (Exception)
        {
            return ErrorFile.UnknownError;
        }
    }

    // Вспомогательный метод для проверки блокировки файла
    public static bool IsFileLocked(IOException exception)
    {
        int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
        return errorCode == 32 || errorCode == 33; // Коды ошибок Windows: Sharing violation / Lock violation
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CreateDir(string path) => Directory.CreateDirectory(path);

    public static void OpenFileProjMetadata(string path)
    {
        if (!TryOpenFile(path, out ErrorFile error, out string pathProj))
        {
            string errorMes = ErrorFileMessage(error);

            // Формируем детальное описание путей, красиво разбивая на строки
            string details = pathProj is null
                ? $"Path: {path}"
                : $"Path: {path}\nProject Path: {pathProj}";

            MessageBox.Show($"{errorMes}\n\n{details}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        OpenFile(pathProj);
    }
    private static void OpenFile(string path)
    {
        using var reader = new StreamReader(path);
        int lenghtPathsFiles = GetLenghtPathsFiles(reader.ReadLine());
    }

    private static int GetLenghtPathsFiles(string? line)
    {
        if (line != null && int.TryParse(line.AsSpan(), out int res))
        {
            return res;
        }
        return 0;
    }

    private static void OpenScripts(string path, ProjectManager projectManager)
    {
        
    }


    public static bool TryOpenFile(string path, out ErrorFile error, out string pathProj)
    {
        pathProj = null!;

        try
        {
            if (!File.Exists(path))
            {
                error = ErrorFile.FileNotFound;
                return false;
            }

            FileInfo fileInfo = new(path);
            if (fileInfo.Length == 0)
            {
                error = ErrorFile.EmptyFile;
                return false;
            }

            using var reader = new StreamReader(path);

            string? pathProjFile = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(pathProjFile))
            {
                error = ErrorFile.CorruptedData;
                return false;
            }
            if (!TryIdentifyPathProject(pathProjFile, out error)) return false;
            pathProj = pathProjFile;
            string? versionFile = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(versionFile))
            {
                error = ErrorFile.CorruptedData;
                return false;
            }

            // .AsSpan() преобразует строку в ReadOnlySpan<char> без выделения памяти
            if (!TryIdentifyVersion(versionFile.AsSpan(), out error)) return false;
        }
        catch (UnauthorizedAccessException)
        {
            error = ErrorFile.AccessDenied;
            return false;
        }
        catch (IOException ex) when (IsFileLocked(ex))
        {
            error = ErrorFile.FileLocked;
            return false;
        }
        catch
        {
            error = ErrorFile.UnknownError;
            return false;
        }

        error = ErrorFile.None;
        return true;
    }

    private static bool TryIdentifyVersion(ReadOnlySpan<char> version, out ErrorFile error)
    {
        error = ErrorFile.None;
        Span<Range> componentRanges = stackalloc Range[4];
        int componentsCount = version.Split(componentRanges, '.');

        if (componentsCount < 1 || componentsCount > 4)
        {
            error = ErrorFile.InvalidVersion;
            return false;
        }

        Span<int> versionNumbers = stackalloc int[4];
        versionNumbers.Clear();

        for (int i = 0; i < componentsCount; i++)
        {
            ReadOnlySpan<char> componentSpan = version[componentRanges[i]];

            if (!int.TryParse(componentSpan, out int number) || number < 0)
            {
                error = ErrorFile.CorruptedData;
                return false;
            }

            versionNumbers[i] = number;
        }

        // Структура успешно создается на стеке
        VersionData parsedVersion = new(versionNumbers[0], versionNumbers[1], versionNumbers[2], versionNumbers[3]);
        return true;
    }

    private static bool TryIdentifyPathProject(ReadOnlySpan<char> path, out ErrorFile error)
    {
        // Вместо обращения к жесткому диску (File.Exists) просто проверяем валидность строки пути
        if (path.IsWhiteSpace() || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            error = ErrorFile.CorruptedData;
            return false;
        }

        error = ErrorFile.None;
        return true;
    }

    private readonly ref struct VersionData(int major, int minor, int build = 0, int revision = 0)
    {
        public readonly int Major = major;
        public readonly int Minor = minor;
        public readonly int Build = build;
        public readonly int Revision = revision;
    }

    public static bool IsCompilted(this ErrorFile error) => error == ErrorFile.None;

    public static string ErrorFileMessage(this ErrorFile error) => error switch
    {
        ErrorFile.None => "[Code Error: 0] Operation is Complited",
        ErrorFile.UnknownError => "[Code Error: 1] Неизвестная ошибка",
        ErrorFile.FileNotFound => "[Code Error: 2] Файл отсутствует или путь неверный",
        ErrorFile.FileNotFoundInProject => "[Code Error: 3] Файл есть на диске, но не находится в проекте",
        ErrorFile.DirectoryNotFound => "[Code Error: 4] Папка с проектами или метаданными удалена или отсутствует",
        ErrorFile.AlreadyExists => "[Code Error: 5] Файл или папка с таким именем уже существуют",
        ErrorFile.InvalidPathCharacters => "[Code Error: 6] Путь содержит запрещенные операционной системой символы",
        ErrorFile.AccessDenied => "[Code Error: 7] Недостаточно прав на чтение/запись файла",
        ErrorFile.FileLocked => "[Code Error: 8] Файл занят другим процессом",
        ErrorFile.EmptyFile => "[Code Error: 9] Файл пуст",
        ErrorFile.CorruptedData => "[Code Error: 10] Файл поврежден",
        ErrorFile.InvalidVersion => "[Code Error: 11] Версия проекта не поддерживается текущей системой",
        ErrorFile.PathTooLong => "[Code Error: 12] Путь к файлу превышает лимит символов",
        ErrorFile.DiskFull => "[Code Error: 13] На диске закончилось свободное место при попытке сохранения",
        _ => "[Code Error: 14] Ошибка не идентифицирована, невозможно определить проблему",
    };
}