using Kernel.Common;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using VMApplication.Logger;

namespace ASM_gen.Output;

public class WpfOutputView : IOutputView   // ILogger оставлен для совместимости с Kernel
{
    private readonly Dispatcher _dispatcher;
    private readonly RichTextBox _outputBox;
    private readonly Paragraph _paragraph;
    private static readonly ConcurrentDictionary<Color, SolidColorBrush> BrushCache = new();

    public WpfOutputView(RichTextBox outputBox)
    {
        _outputBox = outputBox ?? throw new ArgumentNullException(nameof(outputBox));
        _dispatcher = outputBox.Dispatcher;
        _paragraph = new Paragraph();

        _outputBox.Document.Blocks.Clear();
        _outputBox.Document.Blocks.Add(_paragraph);
    }

    public void AppendLine(string message, LogLevel level = LogLevel.Log)
    {
        Color color = level switch
        {
            LogLevel.Log => Colors.WhiteSmoke,
            LogLevel.Warning => Colors.Yellow,
            LogLevel.Error => Colors.Red,
            _ => Colors.Gray
        };
        AppendMessage(message, color, true);
    }

    public void Append(char message)
    {
        AppendMessage(message, Colors.Gray, false);
    }
    public void Clear()
    {
        if (_dispatcher.CheckAccess())
            ClearInternal();
        else
            _dispatcher.Invoke(ClearInternal);
    }

    private void AppendMessage(char message, Color color, bool newLine)
    {
        if (_dispatcher.CheckAccess())
            AppendInternal(message.AsText, color, newLine);
        else
            _dispatcher.BeginInvoke(new Action(() => AppendInternal(message.AsText, color, newLine)));
    }

    private void AppendMessage(string message, Color color, bool newLine)
    {
        if (_dispatcher.CheckAccess())
            AppendInternal(message, color, newLine);
        else
            _dispatcher.BeginInvoke(new Action(() => AppendInternal(message, color, newLine)));
    }

    private void AppendInternal(string message, Color color, bool newLine)
    {
        var brush = BrushCache.GetOrAdd(color, c =>
        {
            var b = new SolidColorBrush(c);
            if (b.CanFreeze) b.Freeze();
            return b;
        });
        _paragraph.Inlines.Add(new Run(message) { Foreground = brush });
        if (newLine)
        _paragraph.Inlines.Add(new LineBreak());
        // Удаляем старые блоки, если нужно
        if (_outputBox.Document.Blocks.Count == 0)
            _outputBox.Document.Blocks.Add(_paragraph);
        _outputBox.ScrollToEnd();
    }

    private void ClearInternal()
    {
        _outputBox.Document.Blocks.Clear();
        _paragraph.Inlines.Clear();
        _outputBox.Document.Blocks.Add(_paragraph);
    }
}