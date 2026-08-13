using ASM_gen.Information_Window;
using ASM_gen.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VMApplication.Emulator;
using VMApplication.Logger;

namespace ASM_gen;

public partial class DeviceManagerWindow : Window
{
    private const string NameSystem = "Manager Device UI Component";
    private readonly VMEmulator _host;
    private readonly IOutputView _outputView;
    private DeviceView? _selectedDevice;
    private DeviceData? _device;

    public ObservableCollection<DeviceView> Devices { get; set; } = [];

    public readonly Action<DeviceData> CurrentDevice;
    public readonly Action<LaunchModeDevice> CurrenLaunchModel;
    public DeviceManagerWindow(IOutputView outputView, VMEmulator host, Action<DeviceData> returned, Action<LaunchModeDevice> currenLaunchModel)
    {
        InitializeComponent();
        DeviceGrid.ItemsSource = Devices;

        Activated += UpdateTable!;
        _host = host;
        _outputView = outputView;

        CurrentDevice = returned;
        CurrenLaunchModel = currenLaunchModel;
    }

    public void SetDeviceData(DeviceData device) => _device = device;
    private void UpdateTable(object sender, EventArgs e) => RefreshDeviceList();

    private void Window_Loaded(object sender, RoutedEventArgs e) => RefreshDeviceList();
    public void RefreshDeviceList()
    {
        var arr = _host.GetAllDevices().ToList();

        Devices.Clear();
        foreach (var device in arr)
        {
            Devices.Add(device);
        }
    }


    private void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedDevice = DeviceGrid.SelectedItem as DeviceView?;
    }

    private void ShowMemory_Click(object sender, RoutedEventArgs e)
    {
        var selected = _selectedDevice;
        if (selected == null) return;
        ReadOnlyMemory<byte> memory = selected.Value.Ram;
        var infoWindow = new InformationWindow(memory)
        {
            Owner = this
        };
        infoWindow.Show();
    }

    private void LoadBin_Click(object sender, RoutedEventArgs e)
    {
        var selected = _selectedDevice;
        if (selected == null || _device == null)
        {
            _outputView.Append($" {NameSystem} Выберите устройство в списке.", LogLevel.Error);
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
            Title = "Выберите .bin файл программы"
        };

        if (dialog.ShowDialog() == true)
        {
            int deviceId = selected.Value.Id;
            byte[] program = System.IO.File.ReadAllBytes(dialog.FileName);
            LaunchModeDevice d = _device.LoadProgram(program);
            CurrentDevice.Invoke(_device);
            CurrenLaunchModel.Invoke(d);
            _outputView?.Append($" {NameSystem} Программа загружена в устройство {deviceId}.");
        }
    }

    private void ChangeSector_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedDevice == null)
        {
            _outputView.Append($" {NameSystem}Выберите устройство в списке.", LogLevel.Error);
            return;
        }

        if (!uint.TryParse(NewSectorBox.Text, out uint newSector))
        {
            _outputView.Append($" {NameSystem}Введите корректный номер сектора.", LogLevel.Error);
            return;
        }

        bool success = _host.ChangeDeviceSector(_selectedDevice.Value.Id, newSector);
        if (success)
        {
            _outputView.Append($" {NameSystem}Сектор изменён на {newSector}.");
            RefreshDeviceList();
        }
        else
        {
            _outputView.Append($" {NameSystem} Не удалось изменить сектор (возможно, занят).", LogLevel.Error);
        }

    }

    private void SetMainDevice_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedDevice == null)
        {
            _outputView.Append($" {NameSystem} Сначала выберите устройство.", LogLevel.Error);
            return;
        }
        
        _host.SetMainDevice(_selectedDevice.Value.Id);
        DeviceData? d = _host.GetDeviceData(_selectedDevice.Value.Id);
        _device = d;
        if (d == null)
        {
            _outputView.Append($" {NameSystem} Выбранное устройство не имеет технической логики в программе, возврат из API вернул null", LogLevel.Error);
            return;
        }
        
        CurrentDevice.Invoke(d);
        _outputView.Append($" {NameSystem} Устройство {_selectedDevice.Value.Id} теперь основное.");
    }

    private void BtnCreateDevice_Click(object sender, RoutedEventArgs e)
    {
        RefreshDeviceList();
        CreateDevice();
    }

    public void CreateDevice()
    {
        var dialog = new CreateDeviceDialog
        {
            Owner = this
        };
        if (dialog.ShowDialog() == true)
        {
            DeviceCreationResult? res = dialog.ViewModel.Result;
            if (res != null)
            {
                int deviceId = _host.CreateDevice(res.Bios, res.RamSize, res.Sector, 
                                                  res.DeviceName, res.ProcName,
                                                  res.RamName, res.PortBusName);
                if (deviceId != -1)
                {
                    _outputView.Append($" {NameSystem} Устройство создано с ID: {deviceId}");
                }
                else
                {
                    _outputView.Append($" {NameSystem} Не удалось создать устройство", LogLevel.Error);
                }
            }
        }
    }

    private void BtnUpdateGrid_Click(object sender, RoutedEventArgs e)
    {
        
    }
}
