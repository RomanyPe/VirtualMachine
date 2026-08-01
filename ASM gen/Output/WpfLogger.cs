using Kernel.BiosSystem;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASM_gen.Output;


public class WpfLogger(RichTextBox outputBox) : ILogger
{
    private readonly Dispatcher _dispatcher = outputBox.Dispatcher;
    private readonly RichTextBox _outputBox = outputBox ?? throw new ArgumentNullException(nameof(outputBox));
    private readonly Paragraph _paragraph = new();

    private static readonly Action<(WpfLogger Arg1, string Arg2, Color Arg3)> LogDelegate =
    static state => state.Arg1.AppendInternal(state.Arg2, state.Arg3);

    private static readonly ConcurrentDictionary<Color, SolidColorBrush> BrushCache = new();

    public bool UseConsole { get; set; }
    public void Info(string message) => AppendMessage(message, Colors.WhiteSmoke);
    public void Warning(string message) => AppendMessage(message, Colors.Yellow);
    public void Error(string message) => AppendMessage(message, Colors.Red);

    private void AppendMessage(string message, Color color)
    {
        // Выполняем в потоке UI
        if (_dispatcher.CheckAccess())
        {
            AppendInternal(message, color);
        }
        else
        {
            _dispatcher.BeginInvoke(LogDelegate, (Arg1: this, Arg2: message, Arg3: color));
        }
    }

    private void AppendInternal(string message, Color color)
    {
        SolidColorBrush brush = BrushCache.GetOrAdd(color, ColorFactory);

        Run run = new(message)
        {
            Foreground = brush
        };
        _paragraph.Inlines.Add(run);

        _paragraph.Inlines.Add(new LineBreak());

        _outputBox.Document.Blocks.Add(_paragraph);
        _outputBox.ScrollToEnd();

        BlockCollection blocks = _outputBox.Document.Blocks;
        while (blocks.Count > 1000)
        {
            var firstBlock = blocks.FirstBlock;
            if (firstBlock != null)
            {
                blocks.Remove(firstBlock);
            }
        }
    }

    private static SolidColorBrush ColorFactory(Color c)
    {
        SolidColorBrush b = new(c);
        if (b.CanFreeze) b.Freeze();
        return b;
    }

    public void Clear()
    {
        if (_dispatcher.CheckAccess())
        {
            _outputBox.Document.Blocks.Clear();
        }
        else
        {
            _dispatcher.Invoke(_outputBox.Document.Blocks.Clear);
        }
    }
}
