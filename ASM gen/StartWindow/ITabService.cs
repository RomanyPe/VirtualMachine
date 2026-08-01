using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace ASM_gen.StartWindow;

public interface ITabService
{
    void AddForAllTab(EventHandler action);
    ValueTask AddNewTab(string fileName);
    string GetCurrentEditorText();
    TextEditor? GetCurrentTextEditor();
    void InitVisualTextEditors(IVisualLineTransformer errorColorizer, IHighlightingDefinition syntax, SolidColorBrush color, string foreGround = "#DCDCDC", string backGround = "#1E1E1E");
    Task RemoveCurrentTab();
    void RemoveForAllTab(EventHandler action);
    Task SaveAllFiles();
    void SelectFirstTab();
}