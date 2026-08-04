using ASM_gen.Highlight;
using ASM_gen.ProjectManage.Data;
using ASM_gen.ProjectManage.Managers.Static;
using ASM_gen.Utils;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Kernel.BiosSystem;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ASM_gen.ProjectManage.Managers;

public class TabService
{
    private readonly TabControl _tabEditor;
    private readonly FileService _fileService;

    public TabControl TabEditor => _tabEditor;
    public TabService(TabControl tabEditor, FileService fileService)
    {
        _tabEditor = tabEditor;
        _fileService = fileService;
        // Подписываемся на смену вкладки
        _tabEditor.SelectionChanged += OnTabSelectionChanged;
    }

    private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_tabEditor.SelectedItem is TabItem tabItem &&
            tabItem.Header is string fileName)
        {
            // Определяем язык по расширению файла
            var lang = fileName.EndsWith(".asm", StringComparison.OrdinalIgnoreCase)
                ? LanguageType.ASM
                : LanguageType.C;

            CastomHighlightingManager.ChoseLang(lang, GetCurrentTextEditor()!);
        }
    }

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
                ErrorFile error = await _fileService.TrySaveTextFile(headerText, editor.Text).ConfigureAwait(false);
                if (error.IsSucced())
                {
                    DeviceHelpers.LogFromSystem("Project Manager", error.ErrorFileMessage());
                }
            }
        }
    }

    public void SaveAllFilesSync()
    {
        foreach (var item in _tabEditor.Items)
        {
            if (item is TabItem tabItem && tabItem.Content is TextEditor editor && tabItem.Header is string headerText)
            {
                var error = _fileService.TrySaveTextFile(headerText, editor.Text).Result; // Осторожно с Result, но для простоты
                if (error != ErrorFile.None)
                    DeviceHelpers.LogFromSystem("Project Manager", error.ErrorFileMessage());
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

    public static void Configurate(TextEditor editor)
    {
        // Фон и основной текст
        editor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
        editor.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCDCDC"));

        // Шрифт
        editor.FontFamily = new FontFamily("Consolas");
        editor.FontSize = 12;

        // Внутренние отступы
        editor.Padding = new Thickness(5);

        // Выделение текста
        editor.TextArea.SelectionBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#264F78"));
        editor.TextArea.SelectionForeground = new SolidColorBrush(Colors.White);

        // Номера строк
        editor.ShowLineNumbers = true;

        // Ширина табуляции (4 пробела)
        editor.TextArea.TextView.Options.IndentationSize = 4;

        // Перенос строк (обычно выключен)
        editor.WordWrap = false;

        // Дополнительно: отключить гиперссылки и email-ссылки в тексте
        editor.TextArea.TextView.Options.EnableEmailHyperlinks = false;
        editor.TextArea.TextView.Options.EnableHyperlinks = false;
    }
    public async ValueTask AddNewTab(string fileName)
    {
        if (_fileService.TryGetFile(fileName, out var dataTab))
        {
            if (!dataTab.IsOpened)
            {
                TextEditor newEditor = new()
                {
                    Text = await _fileService.GetTextFile(fileName),
                };
                Configurate(newEditor);

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
        return _tabEditor.SelectedItem is TabItem tabItem && tabItem.Content is TextEditor editor ? editor : null;
    }

    public TextEditor? GetTextEditorByFileName(string fileName)
    {
        foreach (TabItem item in _tabEditor.Items)
        {
            if (item.Header is string header && header == fileName && item.Content is TextEditor editor)
                return editor;
        }
        return null;
    }

    public async Task RemoveCurrentTab()
    {
        if (_tabEditor.SelectedContent is TabItem tabItem && tabItem.Content is TextEditor editor && tabItem.Header is string headerText)
        {
            var error = await _fileService.TrySaveTextFile(headerText, editor.Text);
            if (error.IsSucced())
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
