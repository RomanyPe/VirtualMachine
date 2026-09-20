namespace VMApplication.Project;

public interface IFileService
{
    string ProjectPath { get; }   // путь к папке проекта
    IEnumerable<string> GetSourceFiles();
    string ReadFile(string fileName);
    void SaveFile(string fileName, string content);
    bool Exist(string fileName);
    string CombinePath(string path1, string path2);

}