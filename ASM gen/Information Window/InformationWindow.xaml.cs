using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASM_gen.Information_Window;

/// <summary>
/// Логика взаимодействия для InformationWindow.xaml
/// </summary>
public partial class InformationWindow : Window
{
    private readonly DispatcherTimer _refreshTimer;
    private readonly MemoryDataProvider? _provider;
    private VirtualizingStackPanel? _panel;

    public InformationWindow(ReadOnlyMemory<byte> deviceMemory)
    {
        InitializeComponent();

        _provider = new MemoryDataProvider(deviceMemory);
        MemoryListView.ItemsSource = _provider;

        // Настраиваем таймер обновления (60 мс ≈ 15 FPS, оптимально для глаз и процессора)
        _refreshTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(60)
        };
        _refreshTimer.Tick += OnRefreshTick;
        _refreshTimer.Start();
    }

    private void OnRefreshTick(object? sender, EventArgs e)
    {
        if (_provider == null) return;

        // Находим VirtualizingStackPanel внутри ListView (достаточно сделать один раз)
        _panel ??= FindVisualChild<VirtualizingStackPanel>(MemoryListView);
        if (_panel == null) return;

        // Получаем индексы первой и последней видимой строки на экране
        int first = (int)Math.Floor(_panel.VerticalOffset);
        int last = (int)Math.Ceiling(_panel.VerticalOffset + _panel.ViewportHeight);

        // Ограничиваем диапазоны во избежание выхода за границы
        first = Math.Max(0, first);
        last = Math.Min(_provider.Count - 1, last);

        // Обновляем только те строки, на которые смотрит пользователь!
        for (int i = first; i <= last; i++)
        {
            if (_provider.GetCachedRow(i) is RowViewModel row)
            {
                row.RefreshIfChanged();
            }
        }
    }

    // Вспомогательный метод для обхода Visual Tree WPF
    private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child is T t) return t;

            T? childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null) return childOfChild;
        }
        return null;
    }

    protected override void OnClosed(EventArgs e)
    {
        _refreshTimer.Stop(); // Обязательно останавливаем таймер при закрытии окна IW
        _provider?.Clear();
        base.OnClosed(e);
    }
}
