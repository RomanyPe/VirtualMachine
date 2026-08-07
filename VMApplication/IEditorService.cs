namespace VMApplication;

public interface IEditorService
{
    void OpenTab(string fileName, string content);
    void CloseTab(string fileName);
    string GetCurrentText();
    string? GetCurrentFileName();
    string? GetText(string fileName);   // возвращает текст из открытой вкладки или null
    void SaveCurrentFile();
    void HighlightErrors(IEnumerable<int> errorLines);   // подсветить строки с ошибками
    void ClearHighlights();

    event EventHandler? TextChanged;

}

