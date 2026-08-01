using System.Windows;

namespace ASM_gen.StartWindow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IDEConsoleManager.InitConsole(false);
        }
    }
}
