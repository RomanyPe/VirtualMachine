using ASM_gen.Analizator;
using ASM_gen.Highlight;
using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMApplication;
using VMApplication.Project;

namespace ASM_gen.ProjectManage.Managers;

public class WpfEditorService : IEditorService
{
    private readonly TabControl _tabControl;
    private readonly Dictionary<string, TabItem> _openTabs = [];
    private readonly Dictionary<string, ErrorLineColorizer> _errorColorizers = [];
    private readonly IFileService _fileService;

    public event EventHandler? TextChanged;

    public WpfEditorService(TabControl tabControl, IFileService fileService)
    {
        _tabControl = tabControl;
        _fileService = fileService;
        _tabControl.SelectionChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OpenTab(string fileName, string content)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_openTabs.TryGetValue(fileName, out var existingTab))
            {
                _tabControl.SelectedItem = existingTab;
                return;
            }

            var editor = new TextEditor
            {
                Text = content,
                // Базовая настройка внешнего вида (как в TabService.Configurate)
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                ShowLineNumbers = true
            };
            editor.ChoseLang(LanguageType.C);
            editor.TextChanged += (s, e) => TextChanged?.Invoke(this, e);

            var errorColorizer = new ErrorLineColorizer();
            editor.TextArea.TextView.LineTransformers.Add(errorColorizer);
            _errorColorizers[fileName] = errorColorizer;

            var tabItem = new TabItem { Header = fileName, Content = editor };
            _openTabs[fileName] = tabItem;
            _tabControl.Items.Add(tabItem);
            _tabControl.SelectedItem = tabItem;
        });
    }

    public void CloseTab(string fileName)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_openTabs.TryGetValue(fileName, out var tab))
            {
                _tabControl.Items.Remove(tab);
                _openTabs.Remove(fileName);
                _errorColorizers.Remove(fileName);
            }
        });
    }

    public string GetCurrentText()
    {
        // Этот метод должен вызываться из UI-потока, или мы можем маршалировать
        if (Application.Current.Dispatcher.CheckAccess())
        {
            return GetTextInternal();
        }
        else
        {
            return Application.Current.Dispatcher.Invoke(GetTextInternal);
        }
    }

    private string GetTextInternal()
    {
        if (_tabControl.SelectedItem is TabItem tab && tab.Content is TextEditor editor)
            return editor.Text;
        return string.Empty;
    }

    public void SaveCurrentFile()
    {
        string? text = null;
        string? fileName = null;
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (_tabControl.SelectedItem is TabItem tab && tab.Header is string header)
            {
                fileName = header;
                if (tab.Content is TextEditor editor)
                    text = editor.Text;
            }
        });

        if (fileName != null && text != null)
            _fileService.SaveFile(fileName, text);
    }

    public string? GetCurrentFileName()
    {
        return Application.Current.Dispatcher.Invoke(() =>
            (_tabControl.SelectedItem as TabItem)?.Header as string);
    }

    public string? GetText(string fileName)
    {
        if (Application.Current.Dispatcher.CheckAccess())
            return GetTextInternal(fileName);
        else
            return Application.Current.Dispatcher.Invoke(() => GetTextInternal(fileName));
    }

    private string? GetTextInternal(string fileName)
    {
        if (_openTabs.TryGetValue(fileName, out var tab) && tab.Content is TextEditor editor)
            return editor.Text;
        return null;
    }

    public void HighlightErrors(IEnumerable<int> errorLines)
    {
        var fileName = GetCurrentFileName();
        if (fileName == null || !_errorColorizers.TryGetValue(fileName, out var colorizer))
            return;

        colorizer.ErrorLines = [.. errorLines];
        // Принудительно перерисовываем текущий редактор
        if (_tabControl.SelectedItem is TabItem tab && tab.Content is TextEditor editor)
            editor.TextArea.TextView.Redraw();
    }

    public void ClearHighlights()
    {
        var fileName = GetCurrentFileName();
        if (fileName == null || !_errorColorizers.TryGetValue(fileName, out var colorizer))
            return;

        colorizer.ErrorLines.Clear();
        if (_tabControl.SelectedItem is TabItem tab && tab.Content is TextEditor editor)
            editor.TextArea.TextView.Redraw();
    }
}