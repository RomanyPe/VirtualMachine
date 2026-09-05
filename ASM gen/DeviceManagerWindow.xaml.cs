using ASM_gen.Information_Window;
using ASM_gen.ViewModels;
using Kernel.Common;
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
    private InformationWindow? infoWindow;
    private DeviceView? _selectedDevice;
    private DeviceContext? _device;

    public ObservableCollection<DeviceView> Devices { get; set; } = [];

    public readonly Action<DeviceContext> CurrentDevice;
    public readonly Action<LaunchModeDevice> CurrenLaunchModel;
    public DeviceManagerWindow(IOutputView outputView, VMEmulator host, Action<DeviceContext> returned, Action<LaunchModeDevice> currenLaunchModel)
    {
        InitializeComponent();
        DeviceGrid.ItemsSource = Devices;

        Activated += UpdateTable!;
        _host = host;
        _outputView = outputView;

        CurrentDevice = returned;
        CurrenLaunchModel = currenLaunchModel;
    }

    public void SetDeviceData(DeviceContext device) => _device = device;
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
        infoWindow?.Close();
        infoWindow = null;
        infoWindow = new InformationWindow(memory)
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
            _outputView.AppendLine($" {NameSystem} Выберите устройство в списке.", LogLevel.Error);
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
            _outputView?.AppendLine($" {NameSystem} Программа загружена в устройство {deviceId}.");
        }
    }

    private void ChangeSector_Click(object sender, RoutedEventArgs e)
    {
        if (!_selectedDevice.HasValue)
        {
            _outputView.AppendLine($" {NameSystem}Выберите устройство в списке.", LogLevel.Error);
            return;
        }
        var device = _selectedDevice.Value;

        if (!uint.TryParse(NewSectorBox.Text, out uint newSector))
        {
            _outputView.AppendLine($"{NameSystem} Введите корректный номер сектора.", LogLevel.Error);
            return;
        }

        bool success = _host.ChangeDeviceSector(device.Id, newSector);
        if (success)
        {
            _outputView.AppendLine($" {NameSystem}Сектор изменён на {newSector}.");
            RefreshDeviceList();
        }
        else
        {
            _outputView.AppendLine($" {NameSystem} Не удалось изменить сектор (возможно, занят).", LogLevel.Error);
        }

    }

    private void SetMainDevice_Click(object sender, RoutedEventArgs e)
    {
        if (!_selectedDevice.HasValue)
        {
            _outputView.AppendLine($" {NameSystem} Сначала выберите устройство.", LogLevel.Error);
            return;
        }
        var device = _selectedDevice.Value;

        _host.SetMainDevice(device.Id);
        DeviceContext? d = _host.GetDeviceData(device.Id);
        _device = d;
        if (d == null)
        {
            _outputView.AppendLine($" {NameSystem} Выбранное устройство не имеет технической логики в программе, возврат из API вернул null", LogLevel.Error);
            return;
        }

        CurrentDevice.Invoke(d);
        _outputView.AppendLine($" {NameSystem} Устройство {device.Id} теперь основное.");
    }

    private void BtnCreateDevice_Click(object sender, RoutedEventArgs e)
    {
        RefreshDeviceList();
        CreateDevice();
        infoWindow?.Close();
        infoWindow = null;
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
                    _outputView.AppendLine($" {NameSystem} Устройство создано с ID: {deviceId}");
                    RefreshDeviceList();
                }
                else
                {
                    _outputView.AppendLine($" {NameSystem} Не удалось создать устройство", LogLevel.Error);
                }
            }
        }
    }

    private void BtnUpdateGrid_Click(object sender, RoutedEventArgs e)
    {
        RefreshDeviceList();
    }

    private void UpdateBios_Click(object sender, RoutedEventArgs e)
    {
        if (!_selectedDevice.HasValue)
        {
            _outputView.AppendLine("Выберите устройство в списке.", LogLevel.Error);
            return;
        }
        var device = _selectedDevice.Value;

        var dialog = new OpenFileDialog
        {
            Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
            Title = "Выберите новый BIOS"
        };
        if (dialog.ShowDialog() == true)
        {
            byte[] bios = System.IO.File.ReadAllBytes(dialog.FileName);
            bool success = _host.UpdateDeviceBios(device.Id, bios);
            _outputView.AppendLine(success
                ? $"BIOS устройства {device.Id} обновлён."
                : "Не удалось обновить BIOS.", success ? LogLevel.Log : LogLevel.Error);
            RefreshDeviceList();
        }
    }

    private void RemoveDevice_Click(object sender, RoutedEventArgs e)
    {
        if (!_selectedDevice.HasValue)
        {
            _outputView.AppendLine("Выберите устройство в списке.", LogLevel.Error);
            return;
        }
        var device = _selectedDevice.Value;
        MessageBoxResult confirm = MessageBox.Show(
            $"Удалить устройство {device.Id}?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        infoWindow?.Close();
        infoWindow = null;

        bool success = _host.RemoveDevice(device.Id); // это 222 строчка кода
        if (success)
        {
            _outputView.AppendLine($"Устройство {device.Id} удалено.");
            RefreshDeviceList();
        }
        else
        {
            _outputView.AppendLine("Не удалось удалить устройство.", LogLevel.Error);
        }
    }

    private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
