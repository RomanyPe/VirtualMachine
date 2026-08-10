using System.Windows;
using VMApplication.Emulator;

namespace ASM_gen.Information_Window;

public static class WindowCreatorIW
{
    extension(IDEPage page)
    {
        public void CreateStateWindow(DeviceData deviceData)
        {
            var currentDevice = deviceData.ReadMemory();
            var parentWindow = Window.GetWindow(page);

            InformationWindow infoWindow = new(currentDevice)
            {
                Owner = parentWindow
            };
            infoWindow.Show();
        }
    }
}
