using System.Windows;

namespace ASM_gen.ViewModels
{
    public partial class CreateDeviceDialog : Window
    {
        public CreateDeviceViewModel ViewModel { get; }

        public CreateDeviceDialog()
        {
            InitializeComponent();
            ViewModel = new CreateDeviceViewModel();
            DataContext = ViewModel;

            ViewModel.DeviceCreated += (s, e) => { DialogResult = true; Close(); };
            ViewModel.Cancelled += (s, e) => { DialogResult = false; Close(); };
        }
    }
}
