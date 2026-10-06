using VMApplication.Project;

namespace VMApplication.Default;

public class RamFileService : IFileService
{
    private readonly Dictionary<string, string> _files = [];
    public string ProjectPath => "this";

    public string CombinePath(string path1, string path2) => Path.Combine(path1, path2);

    public bool Exist(string fileName) => _files.ContainsKey(fileName);

    public IEnumerable<string> GetSourceFiles() => _files.Keys;

    public string ReadFile(string fileName) => _files[fileName];

    public void SaveFile(string fileName, string content) => _files[fileName] = content;
}
