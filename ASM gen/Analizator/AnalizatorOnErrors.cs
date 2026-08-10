using VMApplication;
using VMApplication.Logger;
using VMApplication.Project;

namespace ASM_gen.Analizator;

public class AnalizatorOnErrors : IDisposable
{
    private const string _nameSystem = "Analizator On Errors";
    private readonly IEditorService _editorService;
    private readonly IOutputView _outputView;

    private bool _haveError;
    private CancellationTokenSource? _cts;
    private readonly TimeSpan _validationDelay = TimeSpan.FromMilliseconds(500);
    private readonly Lock _lock = new();

    public bool HaveError => _haveError;

    public AnalizatorOnErrors(IEditorService editorService, IOutputView outputView)
    {
        _editorService = editorService;
        _outputView = outputView;
        _editorService.TextChanged += OnTextChanged;
    }

    public void Dispose()
    {
        _editorService.TextChanged -= OnTextChanged;
        CancelPendingValidation();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
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

        // Берём текст активной вкладки (вызывается из UI-потока, т.к. событие TextChanged в UI)
        string textToCheck = _editorService.GetCurrentText();

        Task.Delay(_validationDelay, cts.Token).ContinueWith(async _ =>
        {
            if (cts.Token.IsCancellationRequested) return;
            var errorLines = await Task.Run(() => CheckTextForErrors(textToCheck), cts.Token);

            // Применяем подсветку в UI-потоке
            if (!cts.Token.IsCancellationRequested)
            {
                _editorService.ClearHighlights();
                if (errorLines.Count > 0)
                    _editorService.HighlightErrors(errorLines);
                _haveError = errorLines.Count > 0;
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

    private List<int> CheckTextForErrors(string text)
    {
        if (text.StartsWith('\uFEFF'))
            text = text[1..];
        var errorLines = new List<int>();
        try
        {
            VMHostHelper.LaunchUnsafeParse(text);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _outputView.Append($"[{_nameSystem}] {ex.Message}", LogLevel.Error);
            int line = ExtractLineFromException(ex);
            if (line > 0) errorLines.Add(line);
        }
        return errorLines;
    }
}