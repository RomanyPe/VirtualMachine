using System.Windows;
using VMApplication;

namespace ASM_gen.Information_Window;

public static class WindowCreatorIW
{
    public static void CreateStateWindow(this IDEPage page, DeviceData deviceData)
    {
        var currentDevice = deviceData.ReadMemory();
        Window parentWindow = Window.GetWindow(page);

        InformationWindow infoWindow = new(currentDevice)
        {
            Owner = parentWindow
        };
        infoWindow.Show();
    }
}
