using ASM_gen.Analizator;
using ASM_gen.ProjectManage.Data;
using ASM_gen.ProjectManage.Managers;
using ASM_gen.ProjectManage.Managers.Static;
using Compiller.Emulation;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace ASM_gen.ProjectManage;

public class ProjectService
{
    private readonly FileService _fileService;
    private readonly TabService _tabService;
    public ProjectService(TabControl tabEditor)
    {
        _fileService = new FileService();
        _tabService = new TabService(tabEditor, _fileService);
    }

    public void AddFile(string path) => _fileService.AddFilePath(path);
    public async void OpenFile(string fileName) => await _tabService.AddNewTab(fileName);
    public void SaveAllFiles() => _ = _tabService.SaveAllFiles();
    public void AddForAllTab(EventHandler action) => _tabService.AddForAllTab(action);

    private void OpenAllTabs()
    {
        foreach (var file in _fileService.GetAllFileNames())
        {
            OpenFile(file);
        }
    }

    private void OpenAllFiles(string projectPath)
    {
        foreach (string file in Directory.GetFiles(projectPath, "*.c"))
        {
            AddFile(file);
        }

        foreach (string file in Directory.GetFiles(projectPath, "*.asm"))
        {
            AddFile(file);
        }
    }

    public void LoadProjectFiles(string projectPath)
    {
        if (!Directory.Exists(projectPath)) return;
        OpenAllFiles(projectPath);
        OpenAllTabs();
    }
    public string GetCurrentEditorText() => _tabService.GetCurrentEditorText();

    public TextEditor? GetCurrentTextEditor() => _tabService.GetCurrentTextEditor();

    public void InitVisualTextEditors(ErrorLineColorizer errorColorizer, IHighlightingDefinition highlightingDefinition, SolidColorBrush white)
    {
        _tabService.InitVisualTextEditors(errorColorizer, highlightingDefinition, white);
    }

    public void RemoveForAllTab(EventHandler action) => _tabService.RemoveForAllTab(action);

    public byte[] BuildProject(string path) => ProjectBuilder.BuildProject(_fileService, _tabService.TabEditor, path);

    /// <summary>
    /// Возвращает список всех исходных файлов проекта (C и ASM)
    /// в виде, готовом для передачи в ProjectBuilder.Build.
    /// </summary>
    public List<SourceFile> GetAllSourceFiles()
    {
        var result = new List<SourceFile>();

        foreach (string fileName in _fileService.GetAllFileNames())
        {
            if (!_fileService.TryGetFile(fileName, out var dataTab))
                continue;

            var editor = _tabService.GetTextEditorByFileName(fileName);

            string source = editor != null? editor.Text : _fileService.ReadFileSync(fileName);

            result.Add(new SourceFile
            {
                Name = fileName,
                Content = source,
                Language = dataTab.Language == SourceLanguage.C
                    ? SourceLanguage.C
                    : SourceLanguage.Asm
            });
        }

        return result;
    }
}