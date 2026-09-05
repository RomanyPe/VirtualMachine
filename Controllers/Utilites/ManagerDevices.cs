using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ControllersData;

namespace Kernel.Utilites;

public class ManagerDevices : IDisposable
{
    public const int MaxCountElements = 64;

    // Плотные массивы данных (Dense Arrays)
    private readonly RamSize[] _size = new RamSize[MaxCountElements];

    private readonly uint[] _sectorByDenseIndex = new uint[MaxCountElements];

    private readonly string?[] _name = new string[MaxCountElements];
    private readonly string?[] _nameProc = new string[MaxCountElements];
    private readonly string?[] _nameRam = new string[MaxCountElements];
    private readonly string?[] _namePortBus = new string[MaxCountElements];

    private readonly Device?[] _devices = new Device[MaxCountElements];

    private readonly int[] _denseToId = new int[MaxCountElements];  // Отображает внутренний индекс в ID
    private readonly int[] _sparse = new int[MaxCountElements];     // Отображает ID во внутренний индекс
    private readonly Queue<int> _freeIds = new();

    private int _count = 0;
    private int _nextId = 0;
    private bool _disposed;
    private readonly PortBus _portBus;

    public int Count => _count;
    public ManagerDevices(PortBus portBus)
    {
        _portBus = portBus;
        Array.Fill(_sparse, -1);
    }

    public DeviceInfo? GetDeviceInfo(int i)
    {
        var dev = _devices[i];
        if (dev != null) return null;

        return new(_denseToId[i], _devices[i]!, _size[i],
                   (SizePortOnDevice)_portBus.PortsOnDevice, _sectorByDenseIndex[i],
                   _name[i], _devices[i]!.CreatedAt);

    }
    public struct DeviceInfo(int id, Device device, RamSize ramSize, SizePortOnDevice portSize, uint sector, string? name, DateTime createdAt)
    {
        public int Id = id;
        public Device Device { get; } = device;
        public RamSize RamSize = ramSize;
        public SizePortOnDevice PortSize = portSize;
        public uint Sector = sector;
        public string? Name = name;
        public DateTime CreatedAt = createdAt;
    }

    public IEnumerable<DeviceInfo> GetAllDevices()
    {
        for (int i = 0; i < _count; i++)
        {
            var dev = _devices[i];
            if (dev != null)
            {
                yield return new(
                    _denseToId[i],
                    _devices[i]!,
                    _size[i],
                    (SizePortOnDevice)_portBus.PortsOnDevice,
                    _sectorByDenseIndex[i],
                    _name[i],
                    _devices[i]!.CreatedAt);
            }
        }
    }

    public Device? GetDevice(int id)
    {
        if (id < 0 || id >= MaxCountElements) return null;
        int denseIndex = _sparse[id];
        if (denseIndex == -1 || denseIndex >= _count) return null;
        return _devices[denseIndex];
    }

    public int CreateNewDevice(
        RamSize size,
        string? name,
        string? nameProc,
        string? nameRam,
        string? namePortBus,
        uint sector,
        byte[] biosFirmware = null!)
    {
        if (_count >= MaxCountElements) return -1;
        if (!_portBus.IsFreeSector(sector)) return -2;

        int deviceId = _freeIds.Count > 0 ? _freeIds.Dequeue() : _nextId++;
        if (deviceId >= MaxCountElements)
            return -3;

        int denseIndex = _count;

        // Запись конфигурации
        _size[denseIndex] = size;
        _name[denseIndex] = name;
        _nameProc[denseIndex] = nameProc;
        _nameRam[denseIndex] = nameRam;
        _namePortBus[denseIndex] = namePortBus;
        _sectorByDenseIndex[denseIndex] = sector;

        // Создаём само устройство
        
        var device = new Device(biosFirmware, _portBus, size, name, nameProc, nameRam);
        // Регистрируем в PortBus
        if (!_portBus.RegisterDevice(device, sector))
        {
            device.Dispose();
            return -4;
        }


        _devices[denseIndex] = device;

        // Связываем Sparse и Dense
        _denseToId[denseIndex] = deviceId;
        _sparse[deviceId] = denseIndex;

        _count++;
        return deviceId;
    }
    /// <summary>
    /// Удаляет устройство по его стабильному ID.
    /// Использует алгоритм Swap-And-Pop для O(1) удаления без сдвигов.
    /// </summary>
    /// <param name="id">ID устройства, которое нужно удалить.</param>
    public bool RemoveDevice(int id)
    {
        // 1. Проверка валидности ID
        if (id < 0 || id >= MaxCountElements)
            return false;

        int denseIndex = _sparse[id];
        if (denseIndex == -1 || denseIndex >= _count)
            return false; // устройство уже удалено или не существует

        // 2. Отключаем устройство от шины портов
        uint sector = _sectorByDenseIndex[denseIndex];
        _portBus.UnregisterDevice(sector);

        // 3. Освобождаем ресурсы самого устройства
        Device? device = _devices[denseIndex];
        if (device != null)
        {
            device.Dispose();
            _devices[denseIndex] = null;
        }

        // 4. Swap-And-Pop (если удаляем не последний элемент)
        int lastDenseIndex = _count - 1;
        if (denseIndex != lastDenseIndex)
        {
            // Копируем данные из последней записи в удаляемую позицию
            _size[denseIndex] = _size[lastDenseIndex];
            _name[denseIndex] = _name[lastDenseIndex];
            _nameProc[denseIndex] = _nameProc[lastDenseIndex];
            _nameRam[denseIndex] = _nameRam[lastDenseIndex];
            _namePortBus[denseIndex] = _namePortBus[lastDenseIndex];
            _sectorByDenseIndex[denseIndex] = _sectorByDenseIndex[lastDenseIndex];
            _devices[denseIndex] = _devices[lastDenseIndex];

            // Обновляем разреженный массив для перемещённого элемента
            int movedId = _denseToId[lastDenseIndex];
            _denseToId[denseIndex] = movedId;
            _sparse[movedId] = denseIndex;
        }

        // 5. Очищаем последнюю запись (для GC и предотвращения утечек)
        _size[lastDenseIndex] = default;
        _name[lastDenseIndex] = null;
        _nameProc[lastDenseIndex] = null;
        _nameRam[lastDenseIndex] = null;
        _namePortBus[lastDenseIndex] = null;
        _sectorByDenseIndex[lastDenseIndex] = 0;
        _devices[lastDenseIndex] = null;

        // 6. Инвалидируем разреженный индекс для удалённого ID
        _sparse[id] = -1;
        _freeIds.Enqueue(id);
        // 7. Уменьшаем счётчик
        _count--;
        return true;
    }

    public void Clear()
    {
        for (int i = _count - 1; i >= 0; i--)
        {
            _name[i] = null;
            _nameProc[i] = null;
            _nameRam[i] = null;
            _namePortBus[i] = null;
            _devices[i]?.Dispose();
            _devices[i] = null;
        }

        _freeIds.Clear();
        _count = 0;

        // 3. Важно для генерации новых ID (опционально, но логично для полного сброса)
        _nextId = 0;
    }

    public bool ChangeSector(int id, uint newSector)
    {
        if (id < 0 || id >= MaxCountElements) return false;
        int denseIndex = _sparse[id];
        if (denseIndex == -1 || denseIndex >= _count) return false;

        // Проверим, не занят ли новый сектор
        if (newSector >= _portBus.SectorCount) return false;
        var existing = _devices[denseIndex];
        // Попробуем зарегистрировать в новом секторе
        if (existing != null && _portBus.RegisterDevice(existing, newSector))
        {
            // Открепим старый сектор
            uint oldSector = _sectorByDenseIndex[denseIndex];
            _portBus.UnregisterDevice(oldSector);
            _sectorByDenseIndex[denseIndex] = newSector;
            return true;
        }
        return false;
    }


    public void Dispose()
    {
        if (_disposed) return;
        Clear(); // уже вызывает Dispose для всех устройств
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
