using VMApplication.Project;

namespace VMApplication.Default;

public sealed class DefaultFileService(string projectPath) : IFileService
{

    public string ProjectPath { get; } = projectPath;

    public IEnumerable<string> GetSourceFiles()
    {
        return Directory.EnumerateFiles(ProjectPath, "*.*", SearchOption.AllDirectories);
    }

    public string ReadFile(string fileName)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        return File.ReadAllText(fullPath);
    }

    public void SaveFile(string fileName, string content)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        File.WriteAllText(fullPath, content);
    }

    public bool Exist(string fileName)
    {
        var fullPath = Path.Combine(ProjectPath, fileName);
        return File.Exists(fullPath);
    }

    public string CombinePath(string path1, string path2) => Path.Combine(path1, path2);
}
