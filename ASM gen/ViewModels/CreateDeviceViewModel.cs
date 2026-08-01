using ASM_gen.Utils;
using Kernel.RamSystem;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ASM_gen.ViewModels;

public class CreateDeviceViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Списки для ComboBox (только RamSize, так как порты задаются в эмуляторе)
    public static IEnumerable<EnumItem> RamSizes => EnumExtensions.GetEnumItems<RamSize>();

    private RamSize _selectedRamSize = RamSize.Size1MB;
    private uint _sector = 0;
    private string _deviceName = "Device";
    private string _procName = "CPU";
    private string _ramName = "RAM";
    private string _portBusName = "PortBus";
    private string _biosPath = string.Empty;

    public RamSize SelectedRamSize
    {
        get => _selectedRamSize;
        set { _selectedRamSize = value; OnPropertyChanged(); }
    }

    public uint Sector
    {
        get => _sector;
        set { _sector = value; OnPropertyChanged(); }
    }

    public string DeviceName
    {
        get => _deviceName;
        set { _deviceName = value; OnPropertyChanged(); }
    }

    public string ProcName
    {
        get => _procName;
        set { _procName = value; OnPropertyChanged(); }
    }

    public string RamName
    {
        get => _ramName;
        set { _ramName = value; OnPropertyChanged(); }
    }

    public string PortBusName
    {
        get => _portBusName;
        set { _portBusName = value; OnPropertyChanged(); }
    }

    public string BiosPath
    {
        get => _biosPath;
        set { _biosPath = value; OnPropertyChanged(); }
    }

    // Результат после создания
    public DeviceCreationResult? Result { get; private set; }

    // Команды
    public ICommand BrowseBiosCommand { get; }
    public ICommand CreateCommand { get; }
    public ICommand CancelCommand { get; }

    // События для закрытия окна
    public event EventHandler<DeviceCreationResult>? DeviceCreated;
    public event EventHandler? Cancelled;

    public CreateDeviceViewModel()
    {
        BrowseBiosCommand = new RelayCommand(_ => BrowseBios());
        CreateCommand = new RelayCommand(_ => CreateDevice());
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    private void BrowseBios()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
            Title = "Select BIOS firmware"
        };
        if (dialog.ShowDialog() == true)
        {
            BiosPath = dialog.FileName;
        }
    }

    private void CreateDevice()
    {
        byte[] bios = [];
        if (!string.IsNullOrEmpty(BiosPath) && File.Exists(BiosPath))
        {
            bios = File.ReadAllBytes(BiosPath);
        }

        Result = new DeviceCreationResult
        {
            RamSize = SelectedRamSize,
            Sector = Sector,
            DeviceName = DeviceName,
            ProcName = ProcName,
            RamName = RamName,
            PortBusName = PortBusName,
            Bios = bios
        };
        DeviceCreated?.Invoke(this, Result);
    }

    private void Cancel() => Cancelled?.Invoke(this, EventArgs.Empty);

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class DeviceCreationResult : EventArgs
{
    public RamSize RamSize { get; init; }
    public uint Sector { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string ProcName { get; init; } = string.Empty;
    public string RamName { get; init; } = string.Empty;
    public string PortBusName { get; init; } = string.Empty;
    public byte[] Bios { get; init; } = [];
}

public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) : ICommand
{
    private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Predicate<object?>? _canExecute = canExecute;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _execute(parameter);
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}