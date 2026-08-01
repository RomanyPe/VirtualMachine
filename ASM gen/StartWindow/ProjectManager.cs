using ASM_gen.Analizator;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Kernel.BiosSystem;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace ASM_gen.StartWindow;

public class TabDataEditor(string path)
{
    public string Path { get; } = path;
    public bool IsOpened { get; set; } = false;
}

public class FileService : IFileService
{
    private readonly Dictionary<string, TabDataEditor> _paths = [];

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
    public async ValueTask<string> GetTextFile(string fileName)
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

    public bool TryGetFile(string fileName, out TabDataEditor dataTab)
    {
        return _paths.TryGetValue(fileName, out dataTab!);
    }
}

public class TabService(TabControl tabEditor, IFileService fileService) : ITabService
{
    private readonly TabControl _tabEditor = tabEditor;
    private readonly IFileService _fileService = fileService;

    public void AddForAllTab(EventHandler action)
    {
        foreach (var item in _tabEditor.Items)
        {
            if (item is TabItem tabItem && tabItem.Content is TextEditor editor)
            {
                editor.TextChanged += action;
            }
        }
    }

    public void InitVisualTextEditors(IVisualLineTransformer errorColorizer, IHighlightingDefinition syntax, SolidColorBrush color, string foreGround = "#DCDCDC", string backGround = "#1E1E1E")
    {
        foreach (var item in _tabEditor.Items)
        {
            if (item is TabItem tabItem && tabItem.Content is TextEditor editor)
            {
                editor.SyntaxHighlighting = syntax;
                editor.TextArea.TextView.LineTransformers.Add(errorColorizer);
                editor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backGround));
                editor.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foreGround));
                editor.BorderBrush = color;
            }
        }
    }

    public async Task SaveAllFiles()
    {
        foreach (var item in _tabEditor.Items)
        {
            if (item is TabItem tabItem && tabItem.Content is TextEditor editor && tabItem.Header is string headerText)
            {
                var error = await _fileService.TrySaveTextFile(headerText, editor.Text);
                if (error != ErrorFile.None)
                {
                    DeviceHelpers.LogFromSystem("Project Manager", error.ErrorFileMessage());
                }
            }
        }
    }

    public void RemoveForAllTab(EventHandler action)
    {
        foreach (var item in _tabEditor.Items)
        {
            if (item is TabItem tabItem && tabItem.Content is TextEditor editor)
            {
                editor.TextChanged -= action;
            }
        }
    }

    public async ValueTask AddNewTab(string fileName)
    {
        if (_fileService.TryGetFile(fileName, out var dataTab))
        {
            if (!dataTab.IsOpened)
            {
                TextEditor newEditor = new()
                {
                    FontFamily = new FontFamily("Consolas"),
                    Text = await _fileService.GetTextFile(fileName),
                    FontSize = 10,
                    ShowLineNumbers = true
                };

                TabItem newTab = new()
                {
                    Header = fileName,
                    Content = newEditor
                };

                _tabEditor.Items.Add(newTab);
                _tabEditor.SelectedItem = newTab;

                dataTab.IsOpened = true;
            }
            else
            {
                foreach (var item in _tabEditor.Items)
                {
                    if (item is TabItem tabItem && tabItem.Header is string headerText && headerText == fileName)
                    {
                        _tabEditor.SelectedItem = tabItem;
                        break;
                    }
                }
            }
        }
    }

    public TextEditor? GetCurrentTextEditor()
    {
        return _tabEditor.SelectedItem is TextEditor currentTab ? currentTab : null;
    }

    public async Task RemoveCurrentTab()
    {
        if (_tabEditor.SelectedContent is TabItem tabItem && tabItem.Content is TextEditor editor && tabItem.Header is string headerText)
        {
            var error = await _fileService.TrySaveTextFile(headerText, editor.Text);
            if (error != ErrorFile.None)
            {
                DeviceHelpers.LogFromSystem("Project Manager", DirectoryManager.ErrorFileMessage(error));
            }
            _tabEditor.Items.Remove(tabItem);
        }
    }

    public void SelectFirstTab()
    {
        if (_tabEditor.Items.Count > 0)
        {
            _tabEditor.SelectedIndex = 0;
        }
    }

    public string GetCurrentEditorText()
    {
        if (_tabEditor.SelectedItem is TabItem currentTab && currentTab.Content is TextEditor editor)
        {
            return editor.Text;
        }
        return string.Empty;
    }
}

public class ProjectManager
{
    private readonly FileService _fileService;
    private readonly TabService _tabService;
    public ProjectManager(TabControl tabEditor)
    {
        _fileService = new FileService();
        _tabService = new TabService(tabEditor, _fileService);
    }

    public void AddForAllTab(EventHandler action) => _tabService.AddForAllTab(action);

    public string GetAllText() => _fileService.GetAllText().Result;

    public string GetCurrentEditorText() => _tabService.GetCurrentEditorText();

    public TextEditor? GetCurrentTextEditor() => _tabService.GetCurrentTextEditor();

    public void InitVisualTextEditors(ErrorLineColorizer errorColorizer,
                                      IHighlightingDefinition highlightingDefinition,
                                      SolidColorBrush white)
    {
        _tabService.InitVisualTextEditors(errorColorizer, highlightingDefinition, white);
    }

    public void RemoveForAllTab(EventHandler action) => _tabService.RemoveForAllTab(action);
}