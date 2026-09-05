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
    private readonly VMHostLogger _hostLogger;
    private readonly VMHostProject _hostProject;
    private readonly VMEmulator _hostEmulator;
    private readonly AnalizatorOnErrors _analizator;
    private readonly WpfOutputView _outputView;
    private readonly ProjectService _projectManager;
    private readonly string _projectPath;
    private readonly IProjectFilesConfig _projectPaths;
    private readonly WpfEditorService _editorService;
    private DeviceContext? _device;
    private LaunchModeDevice? _launchModeDevice;
    private DeviceStepMode? _deviceStepMode;
    private int _delayDeviceThred = 0;
    private int _countStepsBreakDown = 5;

    public bool BiosMode => UseBiosMode.IsChecked ?? false;
    public bool IsDebugMode => DebugMode.IsChecked ?? false;
    public bool UseConsole => ConsoleMode.IsChecked ?? false;
    public bool SnowAssemler => DisassembleMode.IsChecked ?? false;
    public bool SnowTimer => SnowTimerMode.IsChecked ?? false;
    public bool OptimizationCode => OptimizationMode.IsChecked ?? false;

    public IDEPage(string path, SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
    {
        Console.Title = "OutPut Console";
        InitializeComponent();

        _projectPath = path;
        _projectPaths = AppPaths.ProjectSystemPaths(path);
        _outputView = new WpfOutputView(outputBox);

        var fileService = new WpfFileService(_projectPath, _outputView);
        // Создаём редактор
        _editorService = new WpfEditorService(tabEditor, fileService);
        _projectManager = new ProjectService(fileService, _editorService);

        var host = VMHostFactory.CreateDefault(_outputView, _projectPaths, fileService, totalPorts, portsPerDevice);
        
        _hostLogger = host.Logger;
        _hostProject = host.Project;
        _hostEmulator = host.Emulator;

        _analizator = new(_editorService, _outputView);

        _projectManager.OpenProject();
    }

    //private void UpdateDeviceInfo()
    //{
    //    var dev = _hostEmulator.MainDevice();
    //    TxtCurrentDevice.Text = dev != null ? $"Устр-во: {dev.Value.Id}" : "Устр-во не выбрано";
    //}

    private void BtnBreakPointerModeOne(object sender, RoutedEventArgs e)
    {
        if (_launchModeDevice == null)
        {
            _outputView.AppendLine("Устройство не готово к запуску, загрузите в него программу", LogLevel.Error);
            return;
        }

        IDEConsoleManager.InitConsole(UseConsole);

        _deviceStepMode = _launchModeDevice.StepMode(ProjectBuilder.BaseAdressProgram);
    }

    private void BtnBreakPointerOne(object sender, RoutedEventArgs e)
    {
        if (_deviceStepMode == null)
        {
            _outputView.AppendLine("Устройство не подготовлено к последовательному режиму", LogLevel.Error);
            return;
        }
        _deviceStepMode.Step(IsDebugMode);
    }

    private void BtnBreakPointerMulti(object sender, RoutedEventArgs e)
    {
        if (_deviceStepMode == null)
        {
            _outputView.AppendLine("Устройство не подготовлено к последовательному режиму", LogLevel.Error);
            return;
        }
        _deviceStepMode.MultyStep(IsDebugMode, _countStepsBreakDown);
    }

    private void ConsoleMode_Checked(object sender, RoutedEventArgs e)
    {
        if (_device == null || _device.IsRunning == true) return;
    }

    private void BtnClearOutput(object sender, RoutedEventArgs e) => _outputView.Clear();

    private void SetDeviceData(DeviceContext deviceData) => _device = deviceData;
    private void SetLaunchModel(LaunchModeDevice launchModeDevice) => _launchModeDevice = launchModeDevice;

    private void BtnDeviceManager_Click(object sender, RoutedEventArgs e)
    {
        var window = new DeviceManagerWindow(_outputView, _hostEmulator, SetDeviceData, SetLaunchModel)
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

        CompileAndRun();
    }

    private void LogSystemData(CompilationResult res, bool optim)
    {
        if (SnowAssemler)
        {
            var text = VMHostHelper.DisassemblCode(res.Program.AsSpan());
            _outputView.AppendLine(text.TextAsm);
        }
        var resultOpt = res.OptimizationResultLog;
        if (optim && resultOpt != null)
        {
            _outputView.AppendLine("--- Result Optimization ---\n");

            LogsOptimization(resultOpt, TypeOptimization.ASTNodeInlinedFunc,"[Function inlining]");
            LogsOptimization(resultOpt, TypeOptimization.ASTNodeRemovedBeforeInline,"[Function removed]");
            LogsOptimization(resultOpt, TypeOptimization.ASTNodeConstPropagate, "[Constant Propagation]");
            LogsOptimization(resultOpt, TypeOptimization.ASTNodeConstFold, "[Constant Fold]");
            LogsOptimization(resultOpt, TypeOptimization.Peephole,"[Peephole]");
        }
    }
    private void LogsOptimization(OptimizationResultLog log, TypeOptimization type, string logName)
    {
        if (log.Logs.TryGetValue(type, out var v) && v != null)
        {
            _outputView.AppendLine(logName);
            _outputView.AppendLine(v.GetLogs());
        }
    }
    public void CompileAndSafeProgramFile(ulong baseAddress = ProjectBuilder.BaseAdressProgram)
    {
        bool optimize = OptimizationCode;
        _projectManager.SaveAllFiles();
        CompilationResult result = _hostProject.Compile(baseAddress, optimize);


        if (result.Success)
        {
            LogSystemData(result, optimize);
            _projectManager.FileService.SaveBinaryFile("bin", result.Program!);
        }
        else
        {

            foreach (var err in result.Errors!)
                _outputView.AppendLine(err, LogLevel.Error);
        }
    }

    public void CompileToILAndSafeProgramFile(ulong baseAddress = ProjectBuilder.BaseAdressProgram, int c = 5)
    {
        bool optimize = OptimizationCode;
        _projectManager.SaveAllFiles();
        var ilCode = _hostProject.CompileToIL(baseAddress, optimize);
        var result = optimize ? _hostProject.Compile(ilCode, c) : _hostProject.Compile(ilCode);

        if (result.Success)
        {
            LogSystemData(result, optimize);
            _projectManager.FileService.SaveBinaryFile("bin" ,result.Program!);
        }
        else
        {

            foreach (var err in result.Errors!)
                _outputView.AppendLine(err, LogLevel.Error);
        }
    }

    private void CompileAndRun(ulong baseAddress = ProjectBuilder.BaseAdressProgram)
    {
        _outputView.Clear();
        _projectManager.SaveAllFiles();
        _device = _hostEmulator.CreateDeviceContext();

        if (_device == null)
        {
            _outputView.AppendLine("Устройство не подготовлено к запуску", LogLevel.Error);
            return;
        }

        try
        {
            if (BiosMode)
            {
                // Проверяем наличие BIOS
                if (!_device.HaveBios)
                {
                    _outputView.AppendLine("Устройство не имеет BIOS. Создайте устройство с BIOS через Device Manager.", LogLevel.Error);
                    return;
                }

                bool optimize = OptimizationCode;
                CompilationResult result = _hostProject.Compile(baseAddress, optimize);

                if (!result.Success)
                {
                    foreach (var err in result.Errors!)
                        _outputView.AppendLine(err, LogLevel.Error);
                    return;
                }

                string? diskImagePath = PrepareBootDisk(result.Program!, out int diskSector);
                if (diskImagePath == null || diskSector == -1)
                    return;

                // Запускаем с BIOS (стартовый адрес = конец RAM, где расположен BIOS)
                CallBackOnLaunch callBackOnLaunch = new(onEnd: OnEndLaunch);
                _device.ResetMemoryRam();
                _launchModeDevice = _device.GetLaunchMode();
                _launchModeDevice.SetHeapAddress((ulong)result.Program!.Length);
                _launchModeDevice.LaunchDeviceOnDedicatedThread(_device.MaxRamSize, IsDebugMode, _delayDeviceThred, SnowTimer, callBackOnLaunch);
            }
            else
            {
                bool optimize = OptimizationCode;
                CompilationResult result = _hostProject.Compile(baseAddress, optimize);

                if (!result.Success)
                {
                    foreach (var err in result.Errors!)
                        _outputView.AppendLine(err, LogLevel.Error);
                    return;
                }

                CallBackOnLaunch callBackOnLaunch = new(onEnd: OnEndLaunch);
                LogSystemData(result, optimize);
                ulong startAdress = (ulong)result.Program!.Length + result.StartAdress;
                _device.ResetRegistors();
                _device.ResetMemoryRam();
                _launchModeDevice = _device.LoadProgram(result.Program!);
                _launchModeDevice.SetHeapAddress(startAdress);
                _launchModeDevice.LaunchDeviceOnDedicatedThread(ProjectBuilder.BaseAdressProgram, IsDebugMode, _delayDeviceThred, SnowTimer, callBackOnLaunch);
            }
        }
        catch(Exception ex) 
        {
            _outputView.AppendLine(ex.Message, LogLevel.Error);
            _launchModeDevice?.StopAndReset();
        }
    }
    private string? PrepareBootDisk(byte[] program, out int diskSector)
    {
        diskSector = -1;
        string imagePath = Path.Combine(_projectPath, $"boot_{Guid.NewGuid():N}.vmg");
        try
        {
            _hostEmulator.WriteBootableProgramAndCreateImage(imagePath, program); // пишет заголовок + программу
            int sectorCount = Math.Max(1, (program.Length + 8 + DiskContext.SectorSize - 1) / DiskContext.SectorSize);
            diskSector = _hostEmulator.CreateDisk(imagePath, sectorCount);
            return imagePath;
        }
        catch (Exception ex)
        {
            _outputView.AppendLine($"Ошибка подготовки загрузочного диска: {ex.Message}", LogLevel.Error);
            return null;
        }
    }

    private void OnEndLaunch(IDeviceLoggerContext logger)
    {
        logger.Log(_hostEmulator.GetDumpRegisters(), LogLevel.Log);
    }
    private void NewFile_Click(object sender, RoutedEventArgs e) => this.NewFile(_projectPath, _editorService);

    private void SaveAll_Click(object sender, RoutedEventArgs e) => _projectManager.SaveAllFiles();

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _device?.Stop();
        _device?.Dispose();
        _analizator.Dispose();
        _hostEmulator.Dispose();
        _projectManager.SaveAllFiles();
        NavigationService.Navigate(new MainMenu());
    }

    private void BtnSaveBinary_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            IDEConsoleManager.InitConsole(UseConsole);

            CompileToILAndSafeProgramFile();
        }
        catch (Exception ex)
        {
            _outputView.AppendLine($"SaveBinary {ex.Message}", LogLevel.Error);
        }
    }

    private void BtnStopDevices(object sender, RoutedEventArgs e)
    {
        if (_device == null || !_device.IsRunning)
        {
            _outputView.AppendLine("Попытка остановить не запущенного устройства", LogLevel.Error);
            return;
        }
        _device.Stop();
    }

    private void BtnDiskManager_Click(object sender, RoutedEventArgs e)
    {
        var window = new DiskManager(_hostEmulator, _outputView, _projectPath)
        {
            Owner = Window.GetWindow(this)
        };
        window.Show();
    }
}