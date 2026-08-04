using Compiller.Emulation;
using System.Windows;

namespace ASM_gen.Information_Window;

public static class WindowCreatorIW
{
    public static void CreateStateWindow(this IDEPage page, Emulator emulator)
    {
        var device = emulator.CurrentDevice;
        if (device == null) return;

        byte[] currentDevice = device.RamArray;
        Window parentWindow = Window.GetWindow(page);

        InformationWindow infoWindow = new(currentDevice)
        {
            Owner = parentWindow
        };
        infoWindow.Show();
    }
}
