using ASM_gen.Analizator;
using ASM_gen.Highlight;
using ASM_gen.Information_Window;
using ASM_gen.Output;
using ASM_gen.StartWindow;
using ASM_gen.ViewModels;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen;

public partial class IDEPage : Page
{
    private readonly AnalizatorOnErrors _analizator;
    private readonly CastomHighlightingManager _highlightingManager;
    private readonly Emulator _emulator;
    private readonly WpfLogger _logger;
    private readonly ProjectManager _projectManager;
    private int _delayDeviceThred = 0;
    private int _countStepsBreakDown = 5;

    public bool IsDebugMode => DebugMode.IsChecked ?? false;
    public bool UseConsole => ConsoleMode.IsChecked ?? false;
    public bool SnowAssemler => DisassembleMode.IsChecked ?? false;
    public bool SnowTimer => SnowTimerMode.IsChecked ?? false;

    public IDEPage(SizePort totalPorts = SizePort.Size16KB, SizePortOnDev portsPerDevice = SizePortOnDev.Size16B)
    {
        Console.Title = "OutPut Console";

        InitializeComponent();
        _logger = new WpfLogger(outputBox);
        LoggerProvider.SetLogger(_logger);

        _projectManager = new(tabEditor);
        _emulator = new(totalPorts, portsPerDevice);
        _analizator = new(_projectManager);
        _highlightingManager = new(textEditor);

        _highlightingManager.ChoseLang(CastomHighlightingManager.LanguageType.C);

        _logger.UseConsole = UseConsole;
    }

    private void DebugCompile()
    {
        if (_analizator.TryCompile(out string text))
        {
            _emulator.Reset();
            _emulator.CreateDevice([], RamSize.Size1MB, 0); // сектор 0
            _emulator.CompileLoadAndRun(text, disassemble: SnowAssemler);
        }
    }

    private void BtnCompileAndLaunch(object sender, RoutedEventArgs e)
    {
        IDEConsoleManager.InitConsole(UseConsole);

        if (_analizator.TryCompile(out string text))
        {
            _emulator.Reset();
            _emulator.CreateDevice([], RamSize.Size1MB, 0);
            _emulator.CompileLoadAndRun(text,
                disassemble: SnowAssemler,
                isDebug: IsDebugMode,
                delayThread: _delayDeviceThred,
                snowTimer: SnowTimer);
        }
    }

    private void BtnBreakPointerModeOne(object sender, RoutedEventArgs e)
    {
        if (_analizator.TryCompile(out string text))
        {
            _emulator.Reset();
            _emulator.CreateDevice([], RamSize.Size1MB, 0);
            _emulator.CompileLoadAndDebug(text);
        }
    }

    private void BtnBreakPointerOne(object sender, RoutedEventArgs e)
    {
        _emulator.Step(IsDebugMode);
    }

    private void BtnBreakPointerMulti(object sender, RoutedEventArgs e)
    {
        _emulator.Step((ulong)_countStepsBreakDown, IsDebugMode);
    }

    private void ConsoleMode_Checked(object sender, RoutedEventArgs e)
    {
        if (_emulator.CurrentDevice?.IsRunning == true) return;
        _logger.UseConsole = UseConsole;
    }

    private void BtnClearOutput(object sender, RoutedEventArgs e)
    {
        _logger.Clear();
    }

    private void BtnCreateStateWindow(object sender, RoutedEventArgs e)
    {
        var device = _emulator.CurrentDevice;
        if (device == null) return;

        byte[] currentDevice = device.RamArray;
        Window parentWindow = Window.GetWindow(this);

        InformationWindow infoWindow = new(currentDevice)
        {
            Owner = parentWindow
        };
        infoWindow.Show();
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

    private void BtnCreateDevice_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new CreateDeviceDialog
        {
            Owner = Window.GetWindow(this)
        };
        if (dialog.ShowDialog() == true)
        {
            var result = dialog.ViewModel.Result;
            if (result != null)
            {
                int deviceId = _emulator.CreateDevice(
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