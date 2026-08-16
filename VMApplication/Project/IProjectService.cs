namespace VMApplication.Project;

public interface IProjectService
{
    void OpenProject();
    void SaveAllFiles();
    IEnumerable<SourceFile> GetSourceFiles();
    string ProjectPath { get; }
    IFileService FileService { get; }
    IEditorService EditorService { get; }
}