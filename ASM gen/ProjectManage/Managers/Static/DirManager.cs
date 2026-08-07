using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using VMApplication;

namespace ASM_gen.ProjectManage.Managers.Static;

public static class DirManager
{


    /// <summary>
    /// Создает всю необходимую структуру папок для работы IDE при старте.
    /// </summary>
    public static void InitializeDirectories()
    {
        CreateDir(AppPaths.CurrentTemplatesPath);
        CreateDir(AppPaths.CurrentProjectsDataPath);
        CreateDir(AppPaths.CurrentUserProjectsPath);
        CreateDir(AppPaths.SharedIncludePath);
    }

    public static void NewFile(this IDEPage page, string projectPath, IProjectService projectService)
    {
        var dialog = new NewFileDialog
        {
            Owner = Window.GetWindow(page)
        };
        if (dialog.ShowDialog() == true && dialog.Result != null)
        {
            string ext = dialog.Result.Value.language == SourceLanguage.C ? ".c" : ".asm";
            string fileName = dialog.Result.Value.fileName + ext;
            string filePath = Path.Combine(projectPath, fileName);

            // Создаём файл на диске с базовым шаблоном
            string template = dialog.Result.Value.language == SourceLanguage.C
                ? "int main() {\n    return 0;\n}\n"
                : "// Программа на ассемблере\nLDI r0, 0\nHALT\n";
            File.WriteAllText(filePath, template);

            // Добавляем в FileService и открываем вкладку
            string? result = projectService.EditorService.GetText(fileName);
            string content = result ?? string.Empty;
            projectService.EditorService.OpenTab(fileName, content);
        }
    }


    public static async Task<Dictionary<string, FileReadResult>> SearchProjects()
    {
        return await Task.Run(GetProjectFilesHeader);
    }

    private static Dictionary<string, FileReadResult> GetProjectFilesHeader()
    {
        CreateDir(AppPaths.CurrentUserProjectsPath);
        var results = new Dictionary<string, FileReadResult>();
        var projFiles = Directory.GetFiles(
            AppPaths.CurrentUserProjectsPath, AppPaths.ExtensionProj, SearchOption.AllDirectories);
        foreach (string projFile in projFiles)
        {
            string projectName = Path.GetFileNameWithoutExtension(projFile);
            string firstLine = File.ReadLines(projFile).FirstOrDefault() ?? string.Empty;
            results[projFile] = new FileReadResult(projectName, firstLine);
        }
        // Также можно добавить старую папку, если нужно
        return results;
    }

    public static string CreateNewProject(string projectName, bool useAsm)
    {
        string projectDir = Path.Combine(AppPaths.CurrentUserProjectsPath, projectName);
        if (Directory.Exists(projectDir))
            throw new InvalidOperationException("Проект с таким именем уже существует.");

        Directory.CreateDirectory(projectDir);

        // Создаём файл проекта (.vmproj) – простой текстовый контейнер с метаинформацией
        string projFilePath = Path.Combine(projectDir, $"{projectName}.vmproj");
        File.WriteAllLines(projFilePath, [
            $"ProjectName:{projectName}",
        "Version:1.0",
        "Language:" + (useAsm ? "ASM" : "C"),
        "Files:"
        ]);

        // Создаём начальный исходный файл
        string ext = useAsm ? ".asm" : ".c";
        string sourceFileName = "main" + ext;
        string sourceFilePath = Path.Combine(projectDir, sourceFileName);

        string template = useAsm
            ? "; main.asm\nLDI r0, 0\nHALT\n"
            : "int main() {\n    return 0;\n}\n";

        File.WriteAllText(sourceFilePath, template);

        // Возвращаем путь к файлу .vmproj (или к папке проекта, решай сам)
        return projFilePath;
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

    public readonly ref struct VersionData(int major, int minor, int build = 0, int revision = 0)
    {
        public readonly int Major = major;
        public readonly int Minor = minor;
        public readonly int Build = build;
        public readonly int Revision = revision;
    }

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