using ASM_gen.ViewModels;
using Compiller.Emulation;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen.Utils.Device;

public static class CreatorDeviceDialoge
{
    public static void CreateDevice(this Window page, Emulator emulator)
    {
        var dialog = new CreateDeviceDialog
        {
            Owner = page
        };
        if (dialog.ShowDialog() == true)
        {
            var result = dialog.ViewModel.Result;
            if (result != null)
            {
                int deviceId = emulator.CreateDevice(
                    result.Bios,
                    result.RamSize,
                    result.Sector,
                    result.DeviceName,
                    result.ProcName,
                    result.RamName,
                    result.PortBusName
                );
                if (deviceId != -1)
                {
                    MessageBox.Show($"Устройство создано с ID: {deviceId}", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось создать устройство", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
