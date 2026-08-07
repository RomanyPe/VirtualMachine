using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ASM_gen.Information_Window;

public class MemoryDataProvider(ReadOnlyMemory<byte> rawMemory) : IList
{
    private readonly ReadOnlyMemory<byte> _rawMemory = rawMemory;
    private const int BytesPerRow = 16;

    // Кэш для активных UI объектов строк, чтобы иметь к ним доступ из таймера
    private readonly Dictionary<int, RowViewModel> _activeRows = [];

    public int Count => (int)Math.Ceiling((double)_rawMemory.Length / BytesPerRow);

    public object? this[int index]
    {
        get
        {
            // Если объект для этой строки уже создан WPF, возвращаем его
            if (_activeRows.TryGetValue(index, out var existingRow))
            {
                return existingRow;
            }

            ulong address = (ulong)(index * BytesPerRow);
            var newRow = new RowViewModel(address, _rawMemory);

            if (_activeRows.Count > 500) _activeRows.Clear();

            _activeRows[index] = newRow;
            return newRow;
        }
        set => throw new NotSupportedException();
    }

    public RowViewModel? GetCachedRow(int index)
    {
        _activeRows.TryGetValue(index, out var row);
        return row;
    }

    public bool IsReadOnly => true;
    public bool IsFixedSize => true;
    public bool IsSynchronized => false;
    public object SyncRoot => this;
    public int Add(object? value) => -1;
    public void Clear() { }
    public bool Contains(object? value) => false;
    public int IndexOf(object? value) => -1;
    public void Insert(int index, object? value) { }
    public void Remove(object? value) { }
    public void RemoveAt(int index) { }
    public void CopyTo(Array array, int index) { }
    public IEnumerator GetEnumerator()
    {
        int totalRows = Count;
        for (int i = 0; i < totalRows; i++)
        {
            yield return this[i];
        }
    }
}


public class RowViewModel : INotifyPropertyChanged
{
    private readonly ulong _address;
    private readonly ReadOnlyMemory<byte> _memory;

    // Кэш для проверки реальных изменений данных
    private ulong _cachedHash1;
    private ulong _cachedHash2;

    public event PropertyChangedEventHandler? PropertyChanged;

    public RowViewModel(ulong address, ReadOnlyMemory<byte> memory)
    {
        _address = address;
        _memory = memory;
        UpdateCache();
    }

    public string AddressHex => _address.ToString("X8");
    public string BytesHex => new MemoryRow(_address, _memory).GetBytesHex();
    public string AsciiText => new MemoryRow(_address, _memory).GetAsciiText();

    // Быстрый хэш 16 байт без аллокаций для детекта изменений
    private (ulong, ulong) GetCurrentHash()
    {
        int length = Math.Min(16, _memory.Length - (int)_address);
        ReadOnlySpan<byte> span = _memory.Span.Slice((int)_address, length);
        ulong h1 = 0;
        ulong h2 = 0;

        if (span.Length >= 8) h1 = BitConverter.ToUInt64(span[..8]);
        if (span.Length == 16) h2 = BitConverter.ToUInt64(span[8..16]);
        // Если хвост меньше 8/16 байт, можно добить побайтово (для оптимизации опущено)

        return (h1, h2);
    }

    private void UpdateCache() => (_cachedHash1, _cachedHash2) = GetCurrentHash();

    // Метод вызывается по таймеру ТОЛЬКО для видимых строк
    public void RefreshIfChanged()
    {
        var (newH1, newH2) = GetCurrentHash();

        // Если хэш совпал — память не изменилась, выходим без аллокации строк!
        if (newH1 == _cachedHash1 && newH2 == _cachedHash2) return;

        // Память изменилась — обновляем кэш и дергаем UI
        _cachedHash1 = newH1;
        _cachedHash2 = newH2;

        OnPropertyChanged(nameof(BytesHex));
        OnPropertyChanged(nameof(AsciiText));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}