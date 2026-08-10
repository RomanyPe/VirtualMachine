using VMApplication.Project;

namespace ASM_gen.Services;

public class ProjectService(IFileService fileService, IEditorService editorService, Action<string>? logCallback = null) : IProjectService
{
    private readonly IFileService _fileService = fileService;
    private readonly IEditorService _editorService = editorService;
    private readonly Action<string>? _logCallback = logCallback; // для логирования

    public IFileService FileService => _fileService;
    public IEditorService EditorService => _editorService;
    public string ProjectPath => _fileService.ProjectPath;

    public void OpenProject()
    {
        _logCallback?.Invoke($"Открытие проекта: {_fileService.ProjectPath}");
        // Проверяем наличие файлов .c и .asm, загружаем их в редактор
        var sourceFiles = _fileService.GetSourceFiles();
        foreach (var file in sourceFiles)
        {
            if (_fileService.Exists(file))
            {
                string content = _fileService.ReadFile(file);
                _editorService.OpenTab(file, content);
            }
        }
    }

    public void SaveAllFiles()
    {
        // Сохраняем текущий открытый файл, а также пробегаем по всем открытым вкладкам
        _editorService.SaveCurrentFile();
        // Дополнительно можно сохранить все изменённые файлы, зная список через IFileService
        foreach (var file in _fileService.GetSourceFiles())
        {
            string? content = _editorService.GetText(file);
            if (content != null)
                _fileService.SaveFile(file, content);
        }
        _logCallback?.Invoke("Все файлы сохранены.");
    }

    public IEnumerable<SourceFile> GetSourceFiles()
    {
        var files = new List<SourceFile>();
        foreach (var fileName in _fileService.GetSourceFiles())
        {
            string source = _editorService.GetText(fileName) ?? _fileService.ReadFile(fileName);
            SourceLanguage lang = fileName.EndsWith(".asm", StringComparison.OrdinalIgnoreCase)
                ? SourceLanguage.Asm : SourceLanguage.C;
            files.Add(new SourceFile(fileName, source, lang));
        }
        return files;
    }
}
