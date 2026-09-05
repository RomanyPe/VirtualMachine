using System.IO;
using VMApplication.Logger;
using VMApplication.Project;

namespace ASM_gen.ProjectManage.Managers;

public class WpfFileService(
    string projectPath,
    IOutputView outputView) : IFileService
{
    private readonly string _binDir = Path.Combine(projectPath, "bin");
    private readonly IOutputView _outputView = outputView;

    public string ProjectPath { get; set; } = projectPath;


    public IEnumerable<string> GetSourceFiles()
    {
        return Directory.GetFiles(ProjectPath, "*.c")
            .Concat(Directory.GetFiles(ProjectPath, "*.asm"))
            .Select(Path.GetFileName)!;
    }

    public string ReadFile(string fileName)
    {
        string fullPath = Path.Combine(ProjectPath, fileName);
        return File.ReadAllText(fullPath);
    }

    public void SaveFile(string fileName, string content)
    {
        string fullPath = Path.Combine(ProjectPath, fileName);
        File.WriteAllText(fullPath, content);
    }

    public bool Exist(string fileName)
    {
        string fullPath = Path.Combine(ProjectPath, fileName);
        return File.Exists(fullPath);
    }

    public void SaveBinaryFile(string path, byte[] prog)
    {
        Directory.CreateDirectory(_binDir);
        string filePath = Path.Combine(_binDir, "program.bin");
        File.WriteAllBytes(filePath, prog);
        _outputView.AppendLine($"[SaveBinary] Программа сохранена в {filePath}");
    }

    public string CombinePath(string path1, string path2) => Path.Combine(path1, path2);

}