namespace VMApplication.Project;

public interface IFileService
{
    string ProjectPath { get; }   // путь к папке проекта
    IEnumerable<string> GetSourceFiles();
    string ReadFile(string fileName);
    void SaveFile(string fileName, string content);
    void SaveProgramFile(byte[] prog);
    bool Exists(string fileName);
}

