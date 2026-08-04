using ASM_gen.ProjectManage.Data;
using ASM_gen.ProjectManage.Managers.Static;
using Kernel.BiosSystem;
using System.IO;
using System.Text;

namespace ASM_gen.ProjectManage.Managers;

public class FileService
{
    private readonly Dictionary<string, TabDataEditor> _paths = [];

    public IEnumerable<string> GetAllFileNames() => _paths.Keys;
    private async ValueTask<string> ReadTextFile(string fileName)
    {
        if (TryGetFile(fileName, out var dataTab))
        {
            return await File.ReadAllTextAsync(dataTab.Path);
        }
        return ErrorFile.FileNotFoundInProject.ErrorFileMessage();
    }
    private static async Task SaveTextFile(string path, string text) => await File.WriteAllTextAsync(path, text);


    public void AddFilePath(string? path)
    {
        if (path == null)
        {
            DeviceHelpers.LogFromSystem("Project Manager", "Path is empty");
            return;
        }

        string fileName = Path.GetFileName(path);

        if (!_paths.TryAdd(fileName, new(path)))
        {
            DeviceHelpers.LogFromSystem("Project Manager", $"File {fileName} with path {path} is enabled in system");
        }
    }
    public void RemoveFile(string fileName) => _paths.Remove(fileName);
    public async Task<string> GetTextFile(string fileName)
    {
        try
        {
            return await ReadTextFile(fileName);
        }
        catch (UnauthorizedAccessException)
        {
            return ErrorFile.AccessDenied.ErrorFileMessage();
        }
        catch (IOException ex) when (DirectoryManager.IsFileLocked(ex))
        {
            return ErrorFile.FileLocked.ErrorFileMessage();
        }
        catch (DirectoryNotFoundException)
        {
            return ErrorFile.DirectoryNotFound.ErrorFileMessage();
        }
        catch
        {
            return ErrorFile.UnknownError.ErrorFileMessage();
        }
    }
    public async Task<ErrorFile> TrySaveTextFile(string fileName, string text)
    {
        if (!TryGetFile(fileName, out var dataTab)) return ErrorFile.FileNotFoundInProject;

        try
        {
            await SaveTextFile(dataTab.Path, text);
            return ErrorFile.None;
        }
        catch (UnauthorizedAccessException)
        {
            return ErrorFile.AccessDenied;
        }
        catch (IOException ex) when (DirectoryManager.IsFileLocked(ex))
        {
            return ErrorFile.FileLocked;
        }
        catch (DirectoryNotFoundException)
        {
            return ErrorFile.DirectoryNotFound;
        }
        catch
        {
            return ErrorFile.UnknownError;
        }
    }

    public async Task<string> GetAllText()
    {
        StringBuilder sb = new(700);

        foreach (string item in _paths.Keys)
        {
            string text = await GetTextFile(item);
            sb.Append(text);
        }

        return sb.ToString();
    }

    public string ReadFileSync(string fileName)
    {
        if (_paths.TryGetValue(fileName, out var dataTab))
            return File.ReadAllText(dataTab.Path);
        throw new FileNotFoundException($"File {fileName} not found in project");
    }

    public bool TryGetFile(string fileName, out TabDataEditor dataTab)
    {
        return _paths.TryGetValue(fileName, out dataTab!);
    }
    public void AddFile(string path)  // открывает существующий файл по полному пути
    {
        string fileName = Path.GetFileName(path);
        if (!_paths.ContainsKey(fileName))
            _paths[fileName] = new TabDataEditor(path);
    }
}
