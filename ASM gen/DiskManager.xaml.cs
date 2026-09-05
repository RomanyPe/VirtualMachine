using Kernel.BiosSystem;
using Kernel.Common;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VMApplication.Emulator;
using VMApplication.Logger;

namespace ASM_gen;

public partial class DiskManager : Window
{
    private const string NameSystem = "Disk Manager";

    private readonly struct DiskDataForTable(DiskInfo d, uint portSize)
    {
        public string Name { get; } = Path.GetFileNameWithoutExtension(d.PathToFile);
        public long SizeDisk { get; } = d.SizeDisk;
        public uint Port { get; } = d.Port;
        public string PortRange { get; } = CreatePortRange(d.Port, portSize);
        public DateTime CreatedAt { get; } = d.CreatedAt;
        public string PathToFile { get; } = d.PathToFile;

        private static string CreatePortRange(uint sector, uint portSize)
        {
            uint start = sector * portSize;
            uint end = start + portSize - 1;
            return $"{start}–{end}";
        }
    }
    private readonly string _diskDir;
    private readonly VMEmulator _host;
    private readonly ObservableCollection<DiskDataForTable> _disks = [];
    private DiskDataForTable? _selectedDisk;
    private readonly IOutputView _outputView;
    private readonly string _projectPath;

    public DiskManager(VMEmulator host, IOutputView outputView, string projectPath)
    {
        InitializeComponent();
        DeviceGrid.ItemsSource = _disks;
        Activated += UpdateTable!;
        _host = host;
        _outputView = outputView;
        _projectPath = projectPath;
        _diskDir = Path.Combine(_projectPath, "Disk");
    }

    private void UpdateTable(object sender, EventArgs e) => RefreshDiskList();
    private void Window_Loaded(object sender, RoutedEventArgs e) => RefreshDiskList();

    public void RefreshDiskList()
    {
        _disks.Clear();
        foreach (DiskInfo d in _host.GetAllDiskInfo())
        {
            _disks.Add(new DiskDataForTable(d, _host.PortsPerDevice));
        }
    }

    private void BtnCloseWindow_Click(object sender, RoutedEventArgs e) => Close();
    private void BtnUpdateGrid_Click(object sender, RoutedEventArgs e) => RefreshDiskList();

    private void BtnCreateNewDisk_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NewDiskDialog(NewDiskDialog.DiskCreationMode.Empty) 
        { 
            Owner = this 
        };

        if (dialog.ShowDialog() == true)
        {
            Directory.CreateDirectory(_diskDir);
            string imagePath = Path.Combine(_diskDir, Guid.NewGuid().ToString("N") + ".vmg");
            int code = _host.CreateDisk(imagePath, dialog.SectorCount, dialog.Port);
            if (code < 0)
                _outputView.AppendLine($"Code Error: {code}", LogLevel.Error);
            else
                RefreshDiskList();
        }
    }

    private void BtnCreateDiscView_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NewDiskDialog(NewDiskDialog.DiskCreationMode.FromImage) 
        { 
            Owner = this 
        };

        if (dialog.ShowDialog() == true)
        {
            int code = _host.CreateDisk(dialog.SelectedFilePath!, dialog.Port);
            if (code < 0)
                _outputView.AppendLine($"Code Error: {code}", LogLevel.Error);
            else
                RefreshDiskList();
        }
    }

    private void LoadBin_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NewDiskDialog(NewDiskDialog.DiskCreationMode.FromBin) 
        { 
            Owner = this 
        };

        if (dialog.ShowDialog() == true)
        {
            byte[] program = File.ReadAllBytes(dialog.SelectedFilePath!);
            Directory.CreateDirectory(_diskDir);

            string? name = dialog.NameDisk;
            if (string.IsNullOrEmpty(name))
            {
                name = Guid.NewGuid().ToString("N");
            }

            string imagePath = Path.Combine(_diskDir, name + ".vmg");
            if (!File.Exists(imagePath)) 
            {
                int code = _host.CreateDisk(imagePath, dialog.Port);
                if (code < 0)
                {
                    _outputView.AppendLine($"Code Error: {code}", LogLevel.Error);
                    File.Delete(imagePath);
                }
                else
                    RefreshDiskList();
            }
            _host.WriteBootableProgram(imagePath, program);
        }
    }

    private void RemoveDevice_Click(object sender, RoutedEventArgs e)
    {
        var selected = _selectedDisk;
        if (selected == null)
        {
            _outputView.AppendLine("Выберите диск в списке.", LogLevel.Error);
            return;
        }
        var confirm = MessageBox.Show($"Удалить диск {selected.Value.Name}?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (confirm != MessageBoxResult.Yes) return;

        uint port = selected.Value.Port;
        if (_host.RemoveDisk(port))
        {
            // Удаляем файл образа
            string imagePath = selected.Value.PathToFile;
            if (File.Exists(imagePath))
                File.Delete(imagePath);
            RefreshDiskList();
            _outputView.AppendLine($"Диск {port} удалён.");
        }
        else
        {
            _outputView.AppendLine("Не удалось удалить диск.", LogLevel.Error);
        }
    }

    private void ChangeSector_Click(object sender, RoutedEventArgs e)
    {
        var selected = _selectedDisk;
        if (selected == null)
        {
            _outputView.AppendLine("Выберите диск в списке.", LogLevel.Error);
            return;
        }
        if (!uint.TryParse(NewPortBox.Text, out uint newPort))
        {
            _outputView.AppendLine("Введите корректный номер порта.", LogLevel.Error);
            return;
        }
        uint oldPort = selected.Value.Port;
        if (_host.ChangeDiskSector(oldPort, newPort))
        {
            _outputView.AppendLine($"Порт диска изменён с {oldPort} на {newPort}.");
            RefreshDiskList();
        }
        else
        {
            _outputView.AppendLine("Не удалось сменить порт (возможно, занят).", LogLevel.Error);
        }
    }

    private void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedDisk = DeviceGrid.SelectedItem as DiskDataForTable?;
    }
}
