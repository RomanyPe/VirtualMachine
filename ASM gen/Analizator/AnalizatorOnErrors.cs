using ASM_gen.StartWindow;
using Compiller.C;
using ICSharpCode.AvalonEdit.Highlighting;
using Kernel.BiosSystem;
using Kernel.ProcessorSystem;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASM_gen.Analizator;

public class AnalizatorOnErrors : IDisposable
{
    private const string code = @"
int a = 3;
int b = 4;
int result;

int sum(int x, int y) {
    int s = 0;
    int i = 0;

    while (i < x) {
        s = s + y;
        i = i + 1;
    }
    return s;
}

void m(){
    a = 90;
}

int main() {
    a = a + 1;
    m();
    result = sum(a, b);
    return result;
}
";
    private const string _nameSystem = "Analizator On Errors";

    private readonly ProjectManager _projectManager;
    private readonly ErrorLineColorizer _errorColorizer = new();
    private bool _haveError;
    private CancellationTokenSource? _cts;
    private readonly TimeSpan _validationDelay = TimeSpan.FromMilliseconds(500); // можно настроить
    private readonly Lock _lock = new();

    public bool HaveError => _haveError;
    public ErrorLineColorizer ErrorColorizer => _errorColorizer;
    public string Code => _projectManager.GetCurrentEditorText();

    public AnalizatorOnErrors(ProjectManager editor)
    {
        _projectManager = editor;
        InitializeEditor();
        _projectManager.AddForAllTab(TextEditor_TextChanged!);
    }

    public void Dispose()
    {

        _projectManager.RemoveForAllTab(TextEditor_TextChanged!);
        CancelPendingValidation();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void InitializeEditor()
    {
        _projectManager.InitVisualTextEditors(ErrorColorizer, HighlightingManager.Instance.GetDefinition("C++"), Brushes.White);
    }

    private void TextEditor_TextChanged(object sender, EventArgs e)
    {
        // При изменении текста отменяем текущую проверку и запускаем новую с задержкой
        CancelPendingValidation();
        ScheduleValidation();
    }

    private void CancelPendingValidation()
    {
        lock (_lock)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void ScheduleValidation()
    {
        CancellationTokenSource cts = new();
        lock (_lock)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = cts;
        }

        // Читаем текст в UI-потоке (здесь он вызывается из TextChanged, т.е. уже в UI)
        string textToCheck = _projectManager.GetCurrentEditorText();

        Task.Delay(_validationDelay, cts.Token).ContinueWith(async _ =>
        {
            if (cts.Token.IsCancellationRequested) return;
            // Используем заранее сохранённую копию текста
            var errorLines = await Task.Run(() => CheckTextForErrors(textToCheck), cts.Token);
            var editor = _projectManager.GetCurrentTextEditor();
            if (editor != null)
            {
                await editor.Dispatcher.InvokeAsync(() =>
                {
                    _haveError = UpdateErrors(errorLines);
                }, DispatcherPriority.Background, cts.Token);
            }
        }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
    }


    private static int ExtractLineFromException(Exception ex)
    {
        string msg = ex.Message;
        int atIndex = msg.IndexOf(" at ", StringComparison.Ordinal);
        if (atIndex != -1)
        {
            ReadOnlySpan<char> span = msg.AsSpan(atIndex + 4);
            int colon = span.IndexOf(':');
            if (colon != -1 && int.TryParse(span[..colon], out int line))
                return line;
        }
        return 0;
    }
    private static List<int> CheckTextForErrors(string text)
    {
        // Удаляем BOM, если есть
        if (text.StartsWith('\uFEFF'))
            text = text[1..];
        var errorLines = new List<int>();
        try
        {
            var lexer = new Lexer(text);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens);
            parser.Parse();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Извлекаем номер строки из сообщения (после добавления "at ..." в Expect)
            DeviceHelpers.ClearLog();
            DeviceHelpers.LogFromSystem(_nameSystem, ex.Message, NotificationType.Error);
            int line = ExtractLineFromException(ex);
            if (line > 0) errorLines.Add(line);
        }
        return errorLines;
    }


    private bool UpdateErrors(List<int> newErrorLines)
    {
        _errorColorizer.ErrorLines.Clear();
        foreach (int line in newErrorLines)
            _errorColorizer.ErrorLines.Add(line);
        var editor = _projectManager.GetCurrentTextEditor();
        if (editor == null) return false;
        editor.TextArea.TextView.Redraw();
        return newErrorLines.Count > 0;
    }

    public bool TryCompile(out string text)
    {
        text = _projectManager.GetCurrentEditorText();
        List<int> errors = CheckTextForErrors(text);
        return !UpdateErrors(errors);
    }
}