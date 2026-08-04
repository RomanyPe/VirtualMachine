using ASM_gen.Analizator;
using ASM_gen.Output;
using ASM_gen.ProjectManage;
using ASM_gen.ProjectManage.Managers.Static;
using ASM_gen.StartWindow;
using Compiller.C;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.ControllersData;
using Kernel.ProcessorSystem;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen;

public partial class IDEPage : Page
{
    private readonly AnalizatorOnErrors _analizator;
    private readonly Emulator _emulator;
    private readonly WpfLogger _logger;
    private readonly ProjectService _projectManager;
    private readonly string _projectPath;
    private readonly string _binDir;
    private int _delayDeviceThred = 0;
    private int _countStepsBreakDown = 5;

    public bool IsDebugMode => DebugMode.IsChecked ?? false;
    public bool UseConsole => ConsoleMode.IsChecked ?? false;
    public bool SnowAssemler => DisassembleMode.IsChecked ?? false;
    public bool SnowTimer => SnowTimerMode.IsChecked ?? false;

    public IDEPage(string path, SizePort totalPorts = SizePort.Size16KB, SizePortOnDev portsPerDevice = SizePortOnDev.Size16B)
    {
        Console.Title = "OutPut Console";
        _projectPath = path;
        InitializeComponent();
        _logger = new WpfLogger(outputBox);
        LoggerProvider.SetLogger(_logger);

        _projectManager = new(tabEditor);
        _emulator = new(totalPorts, portsPerDevice);
        _analizator = new(_projectManager);

        _logger.UseConsole = UseConsole;
        _projectManager.LoadProjectFiles(_projectPath);
        _binDir = Path.Combine(_projectPath, "bin");
        //Window.GetWindow(this).Activated += UpdateDeviceInfo!;
    }


    private void CompileAndRun()
    {
        if (_emulator.MainDevice == null)
        {
            MessageBox.Show("Не выбрано основное устройство. Откройте Device Manager и создайте/выберите устройство.",
                            "Нет устройства", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try
        {
            _projectManager.SaveAllFiles();
            var device = ProjectBuilder.Compile(UseConsole, SnowAssemler, _emulator, _projectManager, _projectPath);
            device?.LaunchDevice(0x0000, isDebug: IsDebugMode, delay: _delayDeviceThred, snowTimer: SnowTimer);
        }
        catch (Exception ex)
        {
            DeviceHelpers.LogFromSystem("Build", ex.Message, NotificationType.Error);
            // Опционально: MessageBox.Show(ex.Message, "Ошибка компиляции");
        }
    }
    private void UpdateDeviceInfo()
    {
        var dev = _emulator.MainDevice;
        TxtCurrentDevice.Text = dev != null ? $"Устр-во: {_emulator.MainDeviceId}" : "Устр-во не выбрано";
    }

    private void DebugCompile()
    {
        IDEConsoleManager.InitConsole(UseConsole);
        if (_emulator.MainDevice == null)
        {
            MessageBox.Show("Не выбрано основное устройство. Откройте Device Manager и создайте/выберите устройство.",
                            "Нет устройства", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try
        {
            _projectManager.SaveAllFiles();
            var device = ProjectBuilder.Compile(UseConsole, SnowAssemler, _emulator, _projectManager, _projectPath);
            device?.BreakPointerLaunchDevice(0x0000);
        }
        catch (Exception ex)
        {
            DeviceHelpers.LogFromSystem("Build", ex.Message, NotificationType.Error);
        }
    }
    private void BtnBreakPointerModeOne(object sender, RoutedEventArgs e) => DebugCompile();

    private void BtnBreakPointerOne(object sender, RoutedEventArgs e) => _emulator.Step(IsDebugMode);

    private void BtnBreakPointerMulti(object sender, RoutedEventArgs e) => _emulator.Step(_countStepsBreakDown, IsDebugMode);

    private void ConsoleMode_Checked(object sender, RoutedEventArgs e)
    {
        if (_emulator.CurrentDevice?.IsRunning == true) return;
        _logger.UseConsole = UseConsole;
    }

    private void BtnClearOutput(object sender, RoutedEventArgs e) => _logger.Clear();


    private void BtnDeviceManager_Click(object sender, RoutedEventArgs e)
    {
        var window = new DeviceManagerWindow(_emulator)
        {
            Owner = Window.GetWindow(this)
        };
        window.Show();
    }
    private void DelayInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DelayInput == null || string.IsNullOrWhiteSpace(DelayInput.Text)) return;

        if (int.TryParse(DelayInput.Text, out int value))
        {
            if (value < 0)
            {
                DelayInput.Text = "0";
                _delayDeviceThred = 0;
            }
            else if (value > 500)
            {
                DelayInput.Text = "500";
                _delayDeviceThred = 500;
            }
            else
            {
                _delayDeviceThred = value;
            }

            DelayInput.SelectionStart = DelayInput.Text.Length;
        }
        else
        {
            DelayInput.Text = "50";
        }
    }

    private void CountBreakPoint_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (countBreakPoint == null || string.IsNullOrWhiteSpace(countBreakPoint.Text)) return;

        if (int.TryParse(countBreakPoint.Text, out int value))
        {
            if (value < 0)
            {
                countBreakPoint.Text = "0";
                _countStepsBreakDown = 0;
            }
            else if (value > 1000)
            {
                countBreakPoint.Text = "1000";
                _countStepsBreakDown = 1000;
            }
            else
            {
                _countStepsBreakDown = value;
            }

            countBreakPoint.SelectionStart = countBreakPoint.Text.Length;
        }
        else
        {
            countBreakPoint.Text = "50";
        }
    }

    private void BtnCompileAndLaunch(object sender, RoutedEventArgs e)
    {
        IDEConsoleManager.InitConsole(UseConsole);
        
        _emulator.CurrentDevice?.LaunchDevice(0x0000,IsDebugMode,_delayDeviceThred, SnowTimer);
    }

    private void NewFile_Click(object sender, RoutedEventArgs e) => this.NewFile(_projectPath, _projectManager);

    private void SaveAll_Click(object sender, RoutedEventArgs e) => _projectManager.SaveAllFiles();

    private void Exit_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainMenu());

    private void BtnSaveBinary_Click(object sender, RoutedEventArgs e)
    {
        IDEConsoleManager.InitConsole(UseConsole);
        try
        {            
            byte[] program = _projectManager.BuildProject(_projectPath); // используем текущий метод сборки
            if (SnowAssemler) MiniCCompiler.DisassembleCode(program);
            if (!Directory.Exists(_binDir)) Directory.CreateDirectory(_binDir);
            string filePath = Path.Combine(_binDir, "program.bin");
            File.WriteAllBytes(filePath, program);
            MessageBox.Show($"Программа сохранена в {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            DeviceHelpers.LogFromSystem("SaveBinary", ex.Message, NotificationType.Error);
        }
    }

    private void BtnStopDevices(object sender, RoutedEventArgs e)
    {
        foreach(var dev in _emulator.AllDevices)
        {
            dev.Device.StopDevice();
        }
    }
}