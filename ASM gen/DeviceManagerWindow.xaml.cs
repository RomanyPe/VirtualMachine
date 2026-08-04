using ASM_gen.Information_Window;
using ASM_gen.Utils.Device;
using Compiller.Emulation;
using Kernel.Utilites;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen
{
    /// <summary>
    /// Логика взаимодействия для DeviceManagerWindow.xaml
    /// </summary>
    public partial class DeviceManagerWindow : Window
    {
        private readonly Emulator _emulator;
        //private readonly IDEPage _page;
        private ManagerDevices.DeviceInfo? _selectedDevice;

        public DeviceManagerWindow(/*IDEPage page,*/Emulator emulator)
        {
            InitializeComponent();
            _emulator = emulator;
            Activated += UpdateTable!;
            //_page = page;
        }
        private void UpdateTable(object sender, EventArgs e) => RefreshDeviceList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDeviceList();
        }

        public void RefreshDeviceList()
        {
            DeviceGrid.ItemsSource = _emulator.AllDevices.ToList();
        }

        private void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDevice = DeviceGrid.SelectedItem as ManagerDevices.DeviceInfo?;
        }

        private void ShowMemory_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDevice?.Device == null) return;

            byte[] memory = _selectedDevice.Value.Device.RamArray;
            var infoWindow = new InformationWindow(memory)
            {
                Owner = this
            };
            infoWindow.Show();
        }

        private void LoadBin_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDevice?.Device == null)
            {
                MessageBox.Show("Выберите устройство в списке.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
                Title = "Выберите .bin файл программы"
            };

            if (dialog.ShowDialog() == true)
            {
                byte[] program = System.IO.File.ReadAllBytes(dialog.FileName);
                _selectedDevice.Value.Device.LoadProgram(program, 0x0000);
                MessageBox.Show($"Программа загружена в устройство {_selectedDevice.Value.Id}.", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeSector_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDevice?.Device == null)
            {
                MessageBox.Show("Выберите устройство в списке.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!uint.TryParse(NewSectorBox.Text, out uint newSector))
            {
                MessageBox.Show("Введите корректный номер сектора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _emulator.ChangeDeviceSector(_selectedDevice.Value.Id, newSector);
            if (success)
            {
                MessageBox.Show($"Сектор изменён на {newSector}.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshDeviceList();
            }
            else
            {
                MessageBox.Show("Не удалось изменить сектор (возможно, занят).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetMainDevice_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDevice == null)
            {
                MessageBox.Show("Сначала выберите устройство.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _emulator.SetMainDevice(_selectedDevice.Value.Id);
            MessageBox.Show($"Устройство {_selectedDevice.Value.Id} теперь основное.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCreateDevice_Click(object sender, RoutedEventArgs e)
        {
            RefreshDeviceList();
            this.CreateDevice(_emulator);
        }
    }
}
