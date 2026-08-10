using ASM_gen.Analizator;
using ASM_gen.Output;
using ASM_gen.ProjectManage.Managers;
using ASM_gen.ProjectManage.Managers.Static;
using ASM_gen.Services;
using ASM_gen.StartWindow;
using Kernel.Common;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VMApplication;
using VMApplication.CallBacks;
using VMApplication.Emulator;
using VMApplication.Logger;
using VMApplication.Project;

namespace ASM_gen;

public partial class IDEPage : Page
{
    private readonly VMHost _vmHost;
    private readonly AnalizatorOnErrors _analizator;
    private readonly WpfOutputView _outputView;
    private readonly ProjectService _projectManager;
    private readonly string _projectPath;
    private readonly IProjectFilesConfig _projectPaths;
    private readonly string _binDir;
    private DeviceData? _device;
    private LaunchModeDevice? _launchModeDevice;
    private DeviceStepMode? _deviceStepMode;
    private int _delayDeviceThred = 0;
    private int _countStepsBreakDown = 5;

    public bool IsDebugMode => DebugMode.IsChecked ?? false;
    public bool UseConsole => ConsoleMode.IsChecked ?? false;
    public bool SnowAssemler => DisassembleMode.IsChecked ?? false;
    public bool SnowTimer => SnowTimerMode.IsChecked ?? false;

    public IDEPage(string path, SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
    {
        Console.Title = "OutPut Console";
        InitializeComponent();

        _projectPath = path;
        _projectPaths = AppPaths.ProjectSystemPaths(path);
        _outputView = new WpfOutputView(outputBox);

        var fileService = new WpfFileService(_projectPath, _outputView);
        // Создаём редактор
        var editorService = new WpfEditorService(tabEditor, fileService);
        _projectManager = new ProjectService(fileService, editorService);

        var hostLogger = new LoggerBuilder()
                    .WithOutPut(_outputView)
                    .Build();

        var hostProj = new VMHostProjectBuilder()
                    .WithPaths(_projectPaths)
                    .WithProjectSevice(_projectManager)
                    .WithLogger(hostLogger)
                    .Build();
        var hostEmulator = new VMEmulatorBuilder()
                    .WithLogger(hostLogger)
                    .WithPortBusSize(totalPorts)
                    .WithPortsPerDevice(portsPerDevice)
                    .Build();

        _vmHost = new VMHostBuilder()
                    .WithLogger(hostLogger)
                    .WithProject(hostProj)
                    .WithEmulator(hostEmulator)
                    .Build();
            

        _analizator = new(editorService, _outputView);

        _projectManager.OpenProject();
        _binDir = Path.Combine(_projectPath, "bin");
    }

    private void UpdateDeviceInfo()
    {
        var dev = _vmHost.Emulator.MainDevice();
        TxtCurrentDevice.Text = dev != null ? $"Устр-во: {dev.Value.Id}" : "Устр-во не выбрано";
    }

    private void BtnBreakPointerModeOne(object sender, RoutedEventArgs e)
    {
        if (_launchModeDevice == null)
        {
            _outputView.Append("Устройство не готово к запуску, загрузите в него программу", LogLevel.Error);
            return;
        }

        IDEConsoleManager.InitConsole(UseConsole);
        _deviceStepMode = _launchModeDevice.StepMode(ProjectBuilder.BaseAdressProgramm);
    }

    private void BtnBreakPointerOne(object sender, RoutedEventArgs e)
    {
        if (_deviceStepMode == null)
        {
            _outputView.Append("Устройство не подготовлено к последовательному режиму", LogLevel.Error);
            return;
        }
        _deviceStepMode.Step(IsDebugMode);
    }

    private void BtnBreakPointerMulti(object sender, RoutedEventArgs e)
    {
        if (_deviceStepMode == null)
        {
            _outputView.Append("Устройство не подготовлено к последовательному режиму", LogLevel.Error);
            return;
        }
        _deviceStepMode.MultyStep(IsDebugMode, _countStepsBreakDown);
    }

    private void ConsoleMode_Checked(object sender, RoutedEventArgs e)
    {
        if (_device == null || _device.IsRunning == true) return;
        //_outputView.UseConsole = UseConsole;
    }

    private void BtnClearOutput(object sender, RoutedEventArgs e) => _outputView.Clear();

    private void SetDeviceData(DeviceData deviceData) => _device = deviceData;
    private void SetLaunchModel(LaunchModeDevice launchModeDevice) => _launchModeDevice = launchModeDevice;

    private void BtnDeviceManager_Click(object sender, RoutedEventArgs e)
    {
        var window = new DeviceManagerWindow(_outputView, _vmHost, SetDeviceData, SetLaunchModel)
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

        _projectManager.SaveAllFiles();
        CompileAndRun();
    }
    public void CompileAndSafeProgramFile(ulong baseAddress = ProjectBuilder.BaseAdressProgramm)
    {
        var res = _vmHost.Project.Compile(baseAddress);
        

        if (res.Success)
        {
            if (SnowAssemler)
            {
                var text = VMHostHelper.DisassemblCode(res.Program!);
                _outputView.Append(text.TextAsm);
            }

            _projectManager.FileService.SaveProgramFile(res.Program!);
        }
        else
        {
            
            foreach (var err in res.Errors!)
                _outputView.Append(err, LogLevel.Error);
        }
    }

    private void CompileAndRun()
    {
        _device = _vmHost.Emulator.CreateDeviceContext();
        if (_device == null)
        {
            _outputView.Append("Устройство не подготовлено к запуску", LogLevel.Error);
            return;
        }

        CompilationResult result = _vmHost.Project.Compile();

        if (result.Success)
        {
            CallBackOnLaunch callBackOnLaunch = new(end: OnEndLaunch);
            if (SnowAssemler)
            {
                var text = VMHostHelper.DisassemblCode(result.Program!);
                _outputView.Append(text.TextAsm);
            }
            ulong startAdress = (ulong)result.Program!.Length + result.StartAdress;
            _launchModeDevice = _device.LoadProgram(result.Program!);
            _launchModeDevice.SetHeapAddress(startAdress);
            _launchModeDevice.LaunchDevice(ProjectBuilder.BaseAdressProgramm, IsDebugMode, _delayDeviceThred, SnowTimer, callBackOnLaunch);
        }
        else
        {
            foreach (var err in result.Errors!)
                _outputView.Append(err, LogLevel.Error);
        }
    }
    
    private void OnEndLaunch()
    {
        _outputView.Append(_vmHost.Emulator.GetDumpRegisters());
    }
    private void NewFile_Click(object sender, RoutedEventArgs e) => this.NewFile(_projectPath, _projectManager);

    private void SaveAll_Click(object sender, RoutedEventArgs e) => _projectManager.SaveAllFiles();

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _device?.Stop();
        _device?.Dispose();
        _projectManager.SaveAllFiles();
        NavigationService.Navigate(new MainMenu());
    }

    private void BtnSaveBinary_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            IDEConsoleManager.InitConsole(UseConsole);
            CompileAndSafeProgramFile();
        }
        catch (Exception ex)
        {
            _outputView.Append($"SaveBinary {ex.Message}", LogLevel.Error);
        }
    }

    private void BtnStopDevices(object sender, RoutedEventArgs e)
    {
        if (_device == null || !_device.IsRunning)
        {
            _outputView.Append("Попытка остановить не запущенного устройства", LogLevel.Error);
            return;
        }
        _device.Stop();        
    }
}