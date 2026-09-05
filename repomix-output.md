Контекст
Решение состоит из 8 проектов (один из них — тесты).

# Kernel.Common
Библиотека классов: статические классы с общедоступными методами (декодирование, форматирование, выравнивание),
перечисления OpCode и RegType, описывающие возможности VM; легковесные безопасные нативные структуры для примитивов, например NameDeviceToken.

# ThreadingSystem
Библиотека классов: планировщик потоков, рассчитанный на короткие и средние задачи, но поддерживающий и долгие. Содержит:
- планировщик;
- класс-обёртку над Thread с мониторингом потока;
- структуры-хендлеры для управления потоком;
- внутренние структуры планировщика.

# Kernel
Библиотека классов: ядро системы. Содержит основные классы устройства, шины данных, процессора, ОЗУ (шина памяти), диска и т.д.

# Compiler
Библиотека классов с двумя компиляторами:

# ASM-подобный язык — переводит код напрямую в байт-код.

# MiniC (C-подобный язык) с возможностями:
- простая арифметика (полнота по Тьюрингу);
- объявление переменных;
- создание методов;
- передача параметров в методы;
- указатели;
- параметры-указатели;
- возврат данных через return;
- аллокация через new (аллокатор разработчик реализует сам, скрытого функционала нет);
- массивы;
- аллокация массивов;
- указатели на массив (возврат new);
- структуры;
- аллокация структур;
- указатели на структуры;
- массивы структур;
- внутри структур могут храниться любые объявленные данные, включая рекурсивные структуры (можно реализовать графы);
- inline-ASM: asm { ... };
- подключение библиотек ASM;
- таблицы прерываний.

# VMApplication
Библиотека классов: API-слой, предоставляет высокоуровневые абстракции для UI, позволяющие реализовать полноценную IDE. Почти полностью изолирует низкоуровневый код от UI.

# ASM_gen
WPF-приложение: текущая версия IDE.

# Текущий запрос
Нужна помощь в планировании кода и в его написании. Если конкретная задача не указана, сначала помоги её декомпозировать и составить план.

# Правила взаимодействия
- Если для решения не хватает информации, сначала задай уточняющие вопросы: что должно получиться, как это реализовать, зачем это нужно сейчас.
- Не додумывай требования и не принимай архитектурные решения без моего подтверждения.
- Не выходи за рамки текущей задачи и не предлагай несвязанные изменения.
- Работай инкрементально: сначала план/дизайн, после подтверждения — код.
- Если нужен доступ к коду — укажи, какие файлы/классы/проекты нужны.
- Доводи задачу до конца; если возникли блокеры — сообщи, но не оставляй решение незавершённым.

# Ожидаемый ответ
- При недостатке данных — список уточняющих вопросов.
- При достаточном контексте — план изменений с указанием затрагиваемых модулей, затем реализация после согласования.
- Пояснения, как изменения влияют на другие части системы.


This file is a merged representation of the entire codebase, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of the entire repository's contents.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
````
ASM gen/
  Analizator/
	AnalizatorOnErrors.cs
	ErrorLineColorizer.cs
  Highlight/
	CastomHighlightingManager.cs
	SyntaxHighlighter.cs
  Information Window/
	InformationWindow.xaml
	InformationWindow.xaml.cs
	MemoryDataProvider.cs
	MemoryRow.cs
  NewProjectManage/
	NewProjectDialog.xaml
	NewProjectDialog.xaml.cs
  Output/
	WpfLogger.cs
  ProjectManage/
	Managers/
	  Static/
		DirManager.cs
	  WpfEditorService.cs
	  WpfFileService.cs
  Services/
	ProjectService.cs
  StartWindow/
	MainMenu.xaml
	MainMenu.xaml.cs
	MainWindow.xaml
	MainWindow.xaml.cs
  Utils/
	EnumExtensions.cs
	ErrorFileHelper.cs
  ViewModels/
	CreateDeviceDialog.xaml
	CreateDeviceDialog.xaml.cs
	CreateDeviceViewModel.cs
  App.xaml
  App.xaml.cs
  ASM gen.csproj
  ASM gen.slnx
  DeviceManagerWindow.xaml
  DeviceManagerWindow.xaml.cs
  DiskManager.xaml
  DiskManager.xaml.cs
  IDEConsoleManager.cs
  IDEPage.xaml
  IDEPage.xaml.cs
  NewFileDialog.xaml
  NewFileDialog.xaml.cs
  Program.cs
Compiller/
  ASM/
	Assembler.cs
	AssemblerParser.cs
	Disassembler.cs
	InstructionEncoder.cs
  C/
	CodeGenerator/
	  CodeGenUtils.cs
	  ExpressionGenerator.cs
	  FunctionContext.cs
	  FunctionGenerator.cs
	  GlobalInfo.cs
	  GlobalMemoryManager.cs
	  StatementGenerator.cs
	  VarLocation.cs
	Optimizators/
	  AstOptimizer.cs
	ASTNode.cs
	Lexer.cs
	Parser.cs
	StructLayout.cs
  Emulation/
	Emulator.cs
  BiosBuilder.cs
  Compiller.csproj
Controllers/
  BiosSystem/
	Device.cs
  ControllersData/
	PortBus.cs
  LocalMemorySystem/
	DiskDevice.cs
  ProcessorSystem/
	Processor.cs
	ProcessorHelpers.cs
	ProcessorPoolEmulator.cs
  RamSystem/
	MemoryBus.cs
	MemoryBusHelpers.cs
	NativeMemoryBuffer.cs
	NativeMemoryPool.cs
	RAMResultInt64.cs
  Utilites/
	DiskManager.cs
	ManagerDevices.cs
  Kernel.csproj
include/
  lib.vma
  math.vma
  ops.vma
  std.vma
  sys.vma
Kernel.Common/
  BiosStatus.cs
  DecodeInstructionResult.cs
  FormaterTextEmulator.cs
  ILogger.cs
  InstructionDecoder.cs
  Kernel.Common.csproj
  LoggerKernel.cs
  LoggerProvider.cs
  LogLevel.cs
  NameDeviceToken.cs
  OpCode.cs
  OpCodeSize.cs
  RamSize.cs
  RegType.cs
  SizePort.cs
  SizePortOnDevice.cs
Tests/
  DiskBootTests.cs
  TestIOInAndOutSystem.cs
  TestIOInAndOutWake.cs
  TestIOSleepAndWake.cs
  Tests.csproj
  ThreadTests.cs
ThreadingSystem/
  Abstraction/
	ITask.cs
	TaskData.cs
  ThreadControl/
	ContextScheduler.cs
	MonitoringThreader.cs
	ThreadHandle.cs
  ThreadMetrics/
	MetricsCollector.cs
  ThreadPool/
	ManualResetEventSlimPool.cs
	ManualResetEventSlimPooledObjectPolicy.cs
  TaskPriority.cs
  ThreadingSystem.csproj
  ThreadScheduler.cs
VMApplication/
  CallBacks/
	CallBackOnLaunch.cs
  Emulator/
	DeviceContext.cs
	DeviceStepMode.cs
	DeviceView.cs
	Disk.cs
	LaunchModeDevice.cs
	VMEmulator.cs
	VMEmulatorBuilder.cs
  Logger/
	IOutputView.cs
	LoggerBuilder.cs
	VMHostLogger.cs
  Project/
	CompilationResult.cs
	ErrorFile.cs
	FileReadResult.cs
	IEditorService.cs
	IFileService.cs
	IProjectFilesConfig.cs
	IProjectService.cs
	ProjectBuilder.cs
	ResultDeCompilation.cs
	SourceFile.cs
	SourceLanguage.cs
	TabDataEditor.cs
	VMHostProject.cs
	VMHostProjectBuilder.cs
  VMApplication.csproj
  VMHostHelper.cs
.gitignore
.repomixignore
README.md
repomix.config.json
Tests.md
````

# Files

## File: ASM gen/DiskManager.xaml
````
<Window x:Class="ASM_gen.DiskManager"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:local="clr-namespace:ASM_gen"
		mc:Ignorable="d"
		Title="DiskManager" Height="400" Width="930">
	<Grid Margin="10">
		<Grid.RowDefinitions>
			<RowDefinition Height="*"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
		</Grid.RowDefinitions>

		<DataGrid x:Name="DeviceGrid" AutoGenerateColumns="False" IsReadOnly="True"
				  SelectionMode="Single" SelectionChanged="DeviceGrid_SelectionChanged">
			<DataGrid.Columns>
				<DataGridTextColumn Header="Название" Binding="{Binding Name}" Width="150"/>
				<DataGridTextColumn Header="Размер диска" Binding="{Binding SizeDisk}" Width="100"/>
				<DataGridTextColumn Header="Порт" Binding="{Binding Port}" Width="80"/>
				<DataGridTextColumn Header="Диапазон адрессов" Binding="{Binding PortRange}" Width="160"/>
				<DataGridTextColumn Header="Создан" Binding="{Binding CreatedAt, StringFormat='{}{0:HH:mm:ss}'}" Width="140"/>
				<DataGridTextColumn Header="Путь до образа" Binding="{Binding PathToFile}" Width="140"/>
			</DataGrid.Columns>
		</DataGrid>

		<StackPanel Grid.Row="1" Orientation="Horizontal" Margin="10,10, 10, 10">
			<Button Content="Загрузить .bin" Click="LoadBin_Click" Padding="10,5" Margin="5,5,10,5"/>
			<TextBlock Text="Новый порт:" VerticalAlignment="Center"/>
			<TextBox x:Name="NewSectorBox" Width="30" Margin="5,10"/>
			<Button Content="Сменить порт" Click="ChangeSector_Click" Padding="10,5" Margin="5,5"/>
			<Button x:Name="BtnCreateNewDisk" Click="BtnCreateNewDisk_Click" 
					Content="Создать диск из образа" Padding="10,5" Margin="10,5,5,5" />
			<Button x:Name="BtnCreateDiscView" Click="BtnCreateDiscView_Click" 
					Content="Создать новый диск" Padding="10,5" Margin="10,5,5,5" />
			<Button Content="Удалить диск" Click="RemoveDevice_Click" Padding="10,5" Margin="5,5"/>
		</StackPanel>
		<StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10" HorizontalAlignment="Right">
			<Button  Grid.Row="2" x:Name="BtnUpdateGrid" Click="BtnUpdateGrid_Click" 
					Content="Обновить" Padding="10,5" Margin="5,10,5,5" />
			<Button  Grid.Row="2" x:Name="BtnCloseWindow" 
					Content="Подтвердить" Padding="10,5" Margin="5,10,5,5" Click="BtnCloseWindow_Click" />
		</StackPanel>

	</Grid>
</Window>
````

## File: ASM gen/DiskManager.xaml.cs
````csharp
using Kernel.BiosSystem;
using Kernel.Common;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VMApplication.Emulator;
using VMApplication.Logger;

namespace ASM_gen;

public partial class DiskManager : Window
{
	private const string NameSystem = "Disk Manager";

	private readonly struct DiskDataForTable(DiskInfo d, uint portSize)
	{
		public readonly string Name = Path.GetFileNameWithoutExtension(d.PathToFile);
		public readonly long SizeDisk = d.SizeDisk;
		public readonly uint Port = d.Port;
		public readonly string PortRange = CreatePortRange(d.Port, portSize);
		public readonly DateTime CreatedAt = d.CreatedAt;
		public readonly string PathToFile = d.PathToFile;

		private static string CreatePortRange(uint sector, uint portSize)
		{
			uint start = sector * portSize;
			uint end = start + portSize - 1;
			return $"{start}–{end}";
		}
	}

	private readonly VMEmulator _host;
	private readonly ObservableCollection<DiskDataForTable> _disks = [];
	private DiskDataForTable? _selectedDisk;
	private readonly IOutputView _outputView;

	public DiskManager(VMEmulator host, IOutputView outputView)
	{
		InitializeComponent();
		DeviceGrid.ItemsSource = _disks;

		Activated += UpdateTable!;
		_host = host;
		_outputView = outputView;
	}

	private void UpdateTable(object sender, EventArgs e) => RefreshDeviceList();
	private void Window_Loaded(object sender, RoutedEventArgs e) => RefreshDeviceList();
	public void RefreshDeviceList()
	{
		_disks.Clear();
		foreach (DiskInfo d in _host.GetAllDiskInfo())
		{
			_disks.Add(new DiskDataForTable(d, _host.PortsPerDevice));
		}
	}

	private void BtnCloseWindow_Click(object sender, RoutedEventArgs e) => Close();

	private void BtnUpdateGrid_Click(object sender, RoutedEventArgs e) => RefreshDeviceList();

	private void BtnCreateDiscView_Click(object sender, RoutedEventArgs e)
	{

	}

	private void RemoveDevice_Click(object sender, RoutedEventArgs e)
	{

	}

	private void BtnCreateNewDisk_Click(object sender, RoutedEventArgs e)
	{

	}

	private void ChangeSector_Click(object sender, RoutedEventArgs e)
	{

	}

	private void LoadBin_Click(object sender, RoutedEventArgs e)
	{
		var selected = _selectedDisk;
		if (selected == null)
		{
			_outputView.Append($" {NameSystem} Выберите устройство в списке.", LogLevel.Error);
			return;
		}
		uint diskPort = selected.Value.Port;
		DiskData? disk = _host.GetDiskData(diskPort);
		if (disk == null)
		{
			_outputView?.Append($" {NameSystem} Выбранное устройство отсутствует в системе, порт устройства: {diskPort}.");
			return;
		}
		var dialog = new OpenFileDialog
		{
			Filter = "Binary files (*.vmg)|*.vmg|All files (*.*)|*.*",
			Title = "Выберите .vmg файл образа диска"
		};

		if (dialog.ShowDialog() == true)
		{

			byte[]? prog = DiskImageHelper.ReadSector(dialog.FileName, 0);

			if (prog == null)
			{
				_outputView?.Append(
					$" {NameSystem} Выбранный образ не существует или поврежден, эмулятор не смог прочитать его корректно:\n{dialog.FileName}.");
				return;
			}
			disk.LoadDataOnDisk(diskPort, prog);
			_outputView?.Append($" {NameSystem} Программа загружена в устройство {diskPort}.");
		}
	}

	private void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_selectedDisk = DeviceGrid.SelectedItem as DiskDataForTable?;
	}
}
````

## File: Kernel.Common/FormaterTextEmulator.cs
````csharp
namespace Kernel.Common;

public static class FormaterTextEmulator
{
	// Массив ровно под размер byte (от 0 до 255)
	private static readonly string[] ByteToTextCache = new string[256];
	private static readonly string[] suffixes = ["B", "KB", "MB", "GB", "TB"];

	static FormaterTextEmulator()
	{
		for (int i = 0; i < 256; i++)
		{
			char character = (char)i;
			ByteToTextCache[i] = character.AsText;
		}
	}

	public static string FormatBytes(ulong bytes)
	{
		int counter = 0;

		if (bytes < 0) bytes = 0;

		while (bytes >= 1024 && counter < suffixes.Length - 1)
		{
			bytes >>= 10;
			counter++;
		}

		return $"{bytes:F1} {suffixes[counter]}";
	}


	extension(char memoryValue)
	{
		// Супербыстрый метод вывода
		public string CharToString() => ByteToTextCache[memoryValue];

		public string AsText => ByteToTextCache[memoryValue];
	}
}
````

## File: Tests/ThreadTests.cs
````csharp
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Diagnostics;
using System.Text;
using ThreadingSystem;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadControl;
using Xunit.Abstractions;

namespace Tests;


public class ThreadTests(ITestOutputHelper output)
{

	private class CpuLoadTask(long iterations) : ITask
	{
		private readonly long _iterations = iterations;
		public long Result; // чтобы компилятор не удалил цикл

		public TaskPriority Priority => TaskPriority.High;
		public CancellationToken Token => new();

		public void Execute()
		{
			long sum = 0;
			for (long i = 0; i < _iterations; i++)
			{
				// Несложные арифметические операции
				sum += i * i ^ (i >> 3);
			}
			Result = sum;
		}
	}

	private class HeavyMathTask(int iterations) : ITask
	{
		private readonly int _iterations = iterations;
		public double Result;

		public TaskPriority Priority => TaskPriority.High;
		public CancellationToken Token => new();

		public void Execute()
		{
			double accumulator = 0.0;
			for (int i = 1; i <= _iterations; i++)
			{
				accumulator += Math.Sqrt(i) * Math.Log(i + 1) / (i % 7 + 1);
			}
			Result = accumulator;
		}
	}

	private const int count = 1000;
	private readonly ITestOutputHelper _outPut = output;

	[Fact]
	public void Test()
	{
		ThreadScheduler? scheduler = null;

		try
		{
			scheduler = new(2, logger: Log);

			HeavyMathTask? task = new(100000000);
			task.Execute();
			var res = task.Result;
			CpuLoadTask? task1 = new(10000000);
			task1.Execute();
			var res1 = task1.Result;
			{
				using ThreadHandle hundle1 = scheduler.Schedule(task);
				using ThreadHandle hundle2 = scheduler.Schedule(task1);

				Assert.True(hundle1.Wait(TimeSpan.FromSeconds(100)), $"Первая задача не выполнилась за 100 секунд");
				Assert.True(res == task.Result);

				Assert.True(hundle2.Wait(TimeSpan.FromSeconds(100)), $"Вторая задача не выполнилась за 100 секунд");
				Assert.True(res1 == task1.Result);
			}
			var times1 = scheduler.GetThreadMetric(0);
			var times2 = scheduler.GetThreadMetric(1);

			StringBuilder str = new();
			var length1 = times1.Index;
			for (int i = 0; i < length1; i++)
			{
				str.AppendLine($"id 0 = {times1[i].Milliseconds} мс");
			}
			var length2 = times2.Index;
			for (int i = 0; i < length2; i++)
			{
				str.AppendLine($"id 1 = {times2[i].Milliseconds} мс");
			}

			_outPut.WriteLine(str.ToString());
		}
		finally
		{
			scheduler?.Stop();

		}
	}

	private void Log(string? log)
	{
		_outPut.WriteLine(log);
	}


}


public class Test(ITestOutputHelper outPut)
{
	private readonly ITestOutputHelper _outPut = outPut;

	private class HeavyMathTask(int iterations) : ITask
	{
		private readonly int _iterations = iterations;
		private double _result;
		public double Result => _result;
		public TaskPriority Priority => TaskPriority.High;
		public CancellationToken Token => new();

		public void Execute()
		{
			double accumulator = 0.0;
			for (int i = 1; i <= _iterations; i++)
			{
				accumulator += Math.Sqrt(i) * Math.Log(i + 1) / (i % 7 + 1);
			}
			_result = accumulator;
		}
	}

	// Задача с целочисленными битовыми операциями
	private class IntegerBitTask(long iterations) : ITask
	{
		private readonly long _iterations = iterations;
		private long _result;
		public long Result => _result;
		public TaskPriority Priority => TaskPriority.High;
		public CancellationToken Token => new();

		public void Execute()
		{
			long sum = 0;
			for (long i = 0; i < _iterations; i++)
			{
				sum += i * i ^ (i >> 3);
			}
			_result = sum;
		}
	}

	// Задача с обработкой массива (нагрузка на память и кэш)
	private class ArrayProcessingTask(int size, int passes = 5) : ITask
	{
		private readonly int _size = size;
		private readonly int _passes = passes;
		private int[] _array = null!;
		private long _checksum;
		public long Checksum => _checksum;
		public TaskPriority Priority => TaskPriority.High;
		public CancellationToken Token => new();

		public void Execute()
		{
			_array = new int[_size];
			var rng = new Random(12345); // фиксированное зерно для детерминизма
			for (int i = 0; i < _array.Length; i++)
				_array[i] = rng.Next();

			long sum = 0;
			for (int pass = 0; pass < _passes; pass++)
			{
				for (int i = 0; i < _array.Length; i++)
				{
					_array[i] = (_array[i] * 31) ^ (_array[i] >> 7);
					sum += _array[i];
				}
			}
			_checksum = sum;
		}
	}

	// Задача с вычислением чисел Фибоначчи (рекурсивно, но с мемоизацией для увеличения нагрузки)
	private class FibonacciTask(int n) : ITask
	{
		private readonly int _n = n;
		private long _result;
		public long Result => _result;
		public TaskPriority Priority => TaskPriority.High;
		public CancellationToken Token => new();

		public void Execute()
		{
			_result = Fib(_n);
		}

		private static long Fib(int n)
		{
			if (n <= 1) return n;
			long a = 0, b = 1;
			for (int i = 2; i <= n; i++)
			{
				long temp = a + b;
				a = b;
				b = temp;
			}
			return b;
		}
	}
	[Fact]
	public void TestMultipleCpuBoundTasks()
	{
		ThreadScheduler? scheduler = null;

		try
		{
			// Создаём задачи с разной нагрузкой
			var tasks = new ITask[]
			{
				new HeavyMathTask(50_000_000),          // ~200-300 мс
				new IntegerBitTask(20_000_000),         // ~150-250 мс
				new ArrayProcessingTask(1_000_000, 3),  // ~300-500 мс
				new FibonacciTask(10_000_000)           // ~100-200 мс
			};

			// Эталонные результаты (последовательное выполнение)
			var expectedResults = new object?[tasks.Length];
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i].Execute();
				expectedResults[i] = tasks[i] switch
				{
					HeavyMathTask t => t.Result,
					IntegerBitTask t => t.Result,
					ArrayProcessingTask t => t.Checksum,
					FibonacciTask t => t.Result,
					_ => null
				};
			}

			scheduler = new ThreadScheduler( logger: Log);

			var handles = new ThreadHandle[tasks.Length];
			for (int i = 0; i < tasks.Length; i++)
				handles[i] = scheduler.Schedule(tasks[i]);

			for (int i = 0; i < handles.Length; i++)
			{
				Assert.True(handles[i].Wait(TimeSpan.FromSeconds(120)),
					$"Задача {i} не выполнилась за 120 секунд");
			}

			// Сравниваем результаты
			for (int i = 0; i < tasks.Length; i++)
			{
				object? actual = tasks[i] switch
				{
					HeavyMathTask t => t.Result,
					IntegerBitTask t => t.Result,
					ArrayProcessingTask t => t.Checksum,
					FibonacciTask t => t.Result,
					_ => null
				};

				// Для double можно указать точность (количество знаков после запятой)
				if (actual is double d1 && expectedResults[i] is double d2)
					Assert.Equal(d2, d1, 5); // 5 знаков точности
				else
					Assert.Equal(expectedResults[i], actual);
			}

			foreach (var h in handles)
				h.Dispose();

			var output = new StringBuilder();
			int threadCount = scheduler.ThreadCount;
			for (int threadId = 0; threadId < threadCount; threadId++)
			{
				var times = scheduler.GetThreadMetric(threadId);
				for (int i = 0; i < times.Index; i++)
					output.AppendLine($"Поток {threadId}, задача {i}: {times[i].Milliseconds} мс");
			}

			_outPut.WriteLine(output.ToString());
		}
		finally
		{
			scheduler?.Stop();
		}
	}
	private void Log(string? log)
	{
		_outPut.WriteLine(log);
	}
}
````

## File: ThreadingSystem/Abstraction/ITask.cs
````csharp
namespace ThreadingSystem.Abstraction;

public interface ITask
{
	TaskPriority Priority { get; }
	CancellationToken Token { get; }
	void Execute();
}
````

## File: ThreadingSystem/Abstraction/TaskData.cs
````csharp
namespace ThreadingSystem.Abstraction;

public readonly struct TaskData(ITask taskAction, ManualResetEventSlim doneEvent)
{
	public readonly ITask TaskAction = taskAction;
	public readonly ManualResetEventSlim DoneEvent = doneEvent;
}
````

## File: ThreadingSystem/ThreadControl/ContextScheduler.cs
````csharp
using System.Collections.Concurrent;
using System.Text;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadPool;

namespace ThreadingSystem.ThreadControl;

public class ContextScheduler(ManualResetEventSlimPool slimPool, SynchronizationContext uiContext, Action<string>? logger = null)
{
	public readonly ConcurrentQueue<TaskData> HighQueue = new();
	public readonly ConcurrentQueue<TaskData> NormalQueue = new();
	public readonly ConcurrentQueue<TaskData> LowQueue = new();

	public readonly ManualResetEventSlimPool SlimPool = slimPool;

	public readonly ManualResetEventSlim WorkSignal = new(false);
	public readonly ManualResetEventSlim ResumeSignal = new(true);

	public volatile bool IsStopping = false;
	public volatile bool IsPaused = false;

	public readonly SynchronizationContext UiContext = uiContext;
	public readonly Action<string>? Logger = logger; // внешний логгер (опционально)

	public bool TryDequeueAny(out TaskData item)
	{
		return HighQueue.TryDequeue(out item) || NormalQueue.TryDequeue(out item) || LowQueue.TryDequeue(out item);
	}

	public void ExecuteItem(TaskData item, StringBuilder log)
	{

		log.AppendLine($"Start {item.TaskAction?.Priority} task");
		try
		{
			item.TaskAction?.Token.ThrowIfCancellationRequested();
			item.TaskAction?.Execute();
			log.AppendLine($"Task with priority: {item.TaskAction?.Priority} completed");
		}
		catch (OperationCanceledException)
		{
			log.AppendLine("Task cancelled");
		}
		catch (Exception ex)
		{
			log.AppendLine($"Exception: {ex.Message}");
			Logger?.Invoke($"Error in {item.TaskAction?.Priority} task: {ex}");
		}
		finally
		{
			if (item.TaskAction?.Priority == TaskPriority.High)
			{
				UiContext?.Post(_ => FlushLog(log) , null);
			}
		}
	}

	public void FlushLog(StringBuilder log)
	{
		if (log.Length == 0) return;
		string text = log.ToString();
		log.Clear();
		Logger?.Invoke(text);
		UiContext?.Post(_ => { /* обновление UI с текстом */ }, null);
	}
}
````

## File: ThreadingSystem/ThreadControl/MonitoringThreader.cs
````csharp
using System.Diagnostics;
using System.Text;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadMetrics;

namespace ThreadingSystem.ThreadControl;

public class MonitoringThreader
{
	private readonly MetricsCollector _metricsTime = new();
	private readonly StringBuilder _log;
	private readonly ContextScheduler _contextScheduler;
	private readonly Stopwatch _sw = new();
	private readonly int _id;
	private readonly Thread _thread;
	private TaskData? _nextTask = null;
	private int _countHighTaskDone = 0;
	private int _countNormalTaskDone = 0;
	private int _countLowTaskDone = 0;
	private bool _isBusy = false;

	public bool IsBusy => Volatile.Read(ref _isBusy);
	public MetricsCollector Metrics => _metricsTime;
	public int Id => _id;
	public int СountHighTaskDone => _countHighTaskDone;
	public int CountNormalTaskDone => _countNormalTaskDone;
	public int CountLowTaskDone => _countLowTaskDone;
	public MonitoringThreader(int id, StringBuilder log, ContextScheduler contextScheduler)
	{
		_log = log;
		_contextScheduler = contextScheduler;
		_id = id;
		_thread = new(WorkerLoop)
		{
			IsBackground = true
		};
	}
	public void StartThread() => _thread.Start();
	public bool TryJoin(int millisecundesOnJoin = 1000) => _thread.Join(millisecundesOnJoin);
	public void Join() => _thread.Join();

	public bool TrySetNextTask(TaskData task)
	{
		if (_nextTask == null)
		{
			_nextTask = task;
			return true;
		}
		return false;
	}

	private void WorkerLoop()
	{
		while (true)
		{
			_contextScheduler.WorkSignal.Wait();

			if (_contextScheduler.IsStopping)
				break;

			if (_contextScheduler.IsPaused)
			{
				_contextScheduler.ResumeSignal.Wait();
				if (_contextScheduler.IsStopping) break;
			}

			_contextScheduler.WorkSignal.Reset();
			if (_nextTask != null)
			{
				ExecuteActionItem(_nextTask.Value);
				_nextTask = null;
			}
			TaskData item;
			while (_contextScheduler.TryDequeueAny(out item))
			{
				ExecuteActionItem(item);
			}

			_contextScheduler.WorkSignal.Reset();
			if (_contextScheduler.TryDequeueAny(out item))
			{
				ExecuteActionItem(item);
			}
		}

		_contextScheduler.FlushLog(_log);
	}

	private void ExecuteActionItem(TaskData item)
	{
		Volatile.Write(ref _isBusy, true);
		_sw.Restart();
		_contextScheduler.ExecuteItem(item, _log);
		_sw.Stop();
		_metricsTime.Add(_sw.Elapsed);
		SetCountOverTheEntirePeriod(item.TaskAction.Priority);
		item.DoneEvent.Set();
		Volatile.Write(ref _isBusy, false);
	}
	private void SetCountOverTheEntirePeriod(TaskPriority priority)
	{
		switch (priority)
		{
			case TaskPriority.High: _countHighTaskDone++; break;
			case TaskPriority.Normal: _countNormalTaskDone++; break;
			case TaskPriority.Low: _countLowTaskDone++; break;
		}
	}
}
````

## File: ThreadingSystem/ThreadControl/ThreadHandle.cs
````csharp
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadPool;

namespace ThreadingSystem.ThreadControl;

public readonly struct ThreadHandle(TaskData slot, ManualResetEventSlimPool pool) : IDisposable
{
	private readonly TaskData _slot = slot;
	private readonly ManualResetEventSlimPool _pool = pool;

	public bool Wait(TimeSpan? timeout = null)
	{
		timeout ??= TimeSpan.FromSeconds(1);
		return _slot.DoneEvent.Wait(timeout.Value);
	}
	public void Wait() => _slot.DoneEvent.Wait();

	public void Dispose() => _pool.Return(_slot.DoneEvent);
}
````

## File: ThreadingSystem/ThreadMetrics/MetricsCollector.cs
````csharp
using System.Runtime.CompilerServices;

namespace ThreadingSystem.ThreadMetrics;

public class MetricsCollector
{
	[InlineArray(BufferSize)]
	private struct TimeBuffer
	{
		public const int BufferSize = 1024;
		private TimeSpan _element0; // Меняем тип базового элемента
	}

	private const int BufferMask = TimeBuffer.BufferSize - 1;

	private TimeBuffer _buffer;
	private int _index = 0;

	public TimeSpan[] GetMetricsTime()
	{
		var result = new TimeSpan[TimeBuffer.BufferSize];
		for (int i = 0; i < TimeBuffer.BufferSize; i++)
		{
			result[i] = _buffer[i];
		}
		return result;
	}

	public TimeSpan this[int i] => _buffer[i];
	public TimeSpan Last => _buffer[_index];
	public int Index => _index;
	public void Clear() => _index = 0;

	public void Add(TimeSpan elapsed)
	{
		ref TimeSpan baseRef = ref Unsafe.As<TimeBuffer, TimeSpan>(ref _buffer);
		Unsafe.Add(ref baseRef, _index & BufferMask) = elapsed;

		_index++;
	}
}
````

## File: ThreadingSystem/ThreadPool/ManualResetEventSlimPool.cs
````csharp
using Microsoft.Extensions.ObjectPool;

namespace ThreadingSystem.ThreadPool;

public class ManualResetEventSlimPool
{
	private readonly ObjectPool<ManualResetEventSlim> _pool;

	public ManualResetEventSlimPool(int maximumRetained = 100)
	{
		var policy = new ManualResetEventSlimPooledObjectPolicy();

		var provider = new DefaultObjectPoolProvider
		{
			MaximumRetained = maximumRetained
		};

		_pool = provider.Create(policy);
	}

	public ManualResetEventSlim Get() => _pool.Get();

	public void Return(ManualResetEventSlim obj) => _pool.Return(obj);
}
````

## File: ThreadingSystem/ThreadPool/ManualResetEventSlimPooledObjectPolicy.cs
````csharp
using Microsoft.Extensions.ObjectPool;

namespace ThreadingSystem.ThreadPool;

public class ManualResetEventSlimPooledObjectPolicy : IPooledObjectPolicy<ManualResetEventSlim>
{
	public ManualResetEventSlim Create()
	{
		return new ManualResetEventSlim(initialState: false);
	}

	public bool Return(ManualResetEventSlim obj)
	{
		if (obj == null) return false;

		obj.Set();
		obj.Reset();

		return true;
	}
}
````

## File: ThreadingSystem/TaskPriority.cs
````csharp
namespace ThreadingSystem;

public enum TaskPriority { High, Normal, Low }
````

## File: ThreadingSystem/ThreadingSystem.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>net10.0</TargetFramework>
	<ImplicitUsings>enable</ImplicitUsings>
	<Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
	<PackageReference Include="Microsoft.Extensions.ObjectPool" Version="10.0.11" />
  </ItemGroup>

</Project>
````

## File: ThreadingSystem/ThreadScheduler.cs
````csharp
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using ThreadingSystem.Abstraction;
using ThreadingSystem.ThreadControl;
using ThreadingSystem.ThreadMetrics;

namespace ThreadingSystem;

public class ThreadScheduler
{
	private class ActionTask(Action action, TaskPriority priority, CancellationToken token) : ITask
	{
		private readonly Action _action = action;
		public TaskPriority Priority { get; } = priority;
		public CancellationToken Token { get; } = token;

		public void Execute() => _action();
	}

	private readonly MonitoringThreader[] _workers;
	private readonly ContextScheduler _contextScheduler;
	private readonly int _workerCount;
	public bool IsStopping => _contextScheduler.IsStopping;
	public bool IsPaused => _contextScheduler.IsPaused;
	public int ThreadCount => _workerCount;

	public ThreadScheduler(int? workerCount = null, Action<string>? logger = null)
	{
		workerCount ??= Environment.ProcessorCount & ~1;
		var uiContext = SynchronizationContext.Current
						?? GenerateNullExp();
		_workerCount = workerCount.Value;
		_workers = new MonitoringThreader[workerCount.Value];
		_contextScheduler = new(new(workerCount.Value), uiContext, logger);

		for (int i = 0; i < workerCount; i++)
		{
			var log = new StringBuilder();
			var t = new MonitoringThreader(i, log, _contextScheduler);
			_workers[i] = t;
			t.StartThread();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static SynchronizationContext GenerateNullExp() => throw new ArgumentNullException();

	public ThreadHandle Schedule(ITask task)
	{
		TaskData item = new(task, _contextScheduler.SlimPool.Get());
		GetQueue(task.Priority).Enqueue(item);
		_contextScheduler.WorkSignal.Set();
		return new ThreadHandle(item, _contextScheduler.SlimPool);
	}

	public ThreadHandle Schedule(Action action, TaskPriority priority = TaskPriority.Normal, CancellationToken token = default)
		=> Schedule(new ActionTask(action, priority, token));

	public void Stop()
	{
		_contextScheduler.IsStopping = true;
		_contextScheduler.WorkSignal.Set(); // разбудить все потоки, чтобы они вышли
		_contextScheduler.ResumeSignal.Set(); // если были в паузе – выйти
		foreach (var t in _workers)
		{
			t.Join(); // дождаться завершения
		}
	}

	public void Pause()
	{
		_contextScheduler.IsPaused = true;
		_contextScheduler.ResumeSignal.Reset(); // дальнейшие ожидания будут блокироваться
	}

	public void Resume()
	{
		_contextScheduler.IsPaused = false;
		_contextScheduler.ResumeSignal.Set(); // разбудить потоки в паузе
	}

	public bool TryAddNewTaskToWorker(int id, ITask task, out ThreadHandle handle)
	{
		handle = default;
		if (id < 0 || id >= _workerCount) return false;

		ManualResetEventSlim poolItem = _contextScheduler.SlimPool.Get();
		TaskData taskData = new(task, poolItem);
		if (_workers[id].TrySetNextTask(taskData))
		{
			handle = new ThreadHandle(taskData, _contextScheduler.SlimPool);
			return true;
		}
		else
		{
			_contextScheduler.SlimPool.Return(poolItem);
			return false;
		}
	}

	public static ITask CreateAsTask(Action action,
									 TaskPriority priority = TaskPriority.Low,
									 CancellationToken token = default) => new ActionTask(action, priority, token);

	public int? FoundFreeWorkerId()
	{
		for (int i = 0; i < _workers.Length; i++)
		{
			if (!_workers[i].IsBusy) return i;
		}
		return null;
	}
	public MetricsCollector GetThreadMetric(int id) => _workers[id].Metrics;

	private ConcurrentQueue<TaskData> GetQueue(TaskPriority priority) => priority switch
	{
		TaskPriority.High => _contextScheduler.HighQueue,
		TaskPriority.Low => _contextScheduler.LowQueue,
		_ => _contextScheduler.NormalQueue
	};
}
````

## File: VMApplication/Emulator/DeviceContext.cs
````csharp
using Kernel.BiosSystem;
using ThreadingSystem.ThreadControl;
using VMApplication.CallBacks;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class DeviceContext : IDisposable
{
	private readonly Device _device;
	private bool? _isBiosMode = null;
	
	internal DeviceContext(Device device) => _device = device;

	public bool IsRunning => _device.IsRunning;
	public long? StepCount => _device.StepCount;
	public ulong MaxRamSize => _device.MaxRamSize;
	public bool HaveBios => _device.HaveBios;
	public DateTime CreatedAt => _device.CreatedAt;

	[Obsolete("""
	Используйте перегрузку 'Stop(ThreadHandle, ...)' для работы через ThreadScheduler.
	
	Внимание: Этот метод предназначен только для потоков, запущенных через 'StartOnDedicatedThread'.
	Смешивание вызовов (например, запуск через Scheduler, а остановка этим методом) 
	приведет к зависанию задачи или утечке ресурсов в пуле воркеров.
	""", error: false)]
	public void Stop(int stopTime = 3000, CallBackOnStopDecidedThread callBack = default)
	{
		_device.StopDevice(stopTime,
						   callBack.OnThreadIsLiveTrue,
						   callBack.OnThreadIsDead,
						   callBack.OnThreadStopedTrue,
						   callBack.OnThreadIsDead);
	}

	public void Stop(ThreadHandle handle, TimeSpan stopTime, CallBackOnStopDecidedThread callBack = default)
	{
		_device.Stop(handle,
					 stopTime,
					 callBack.OnThreadStopedTrue,
					 callBack.OnThreadIsDead);
	}
	private void SetLoadMode(bool isBios)
	{
		if (_isBiosMode.HasValue)
			throw new InvalidOperationException(
				$"Режим загрузки уже установлен как {(_isBiosMode.Value ? "BIOS" : "прямая загрузка")}. Изменить его нельзя.");
		_isBiosMode = isBios;
	}

	public LaunchModeDevice LoadProgram(byte[] program, ulong loadAddress = ProjectBuilder.BaseAdressProgramm)
	{
		SetLoadMode(false);   // фиксируем прямую загрузку
		_device.LoadProgram(program, loadAddress);
		return new LaunchModeDevice(_device);
	}

	public LaunchModeDevice? TryFastLoadProgram(ReadOnlySpan<byte> program, ulong loadAddress, out string? error)
	{
		if (_isBiosMode.HasValue)
		{
			error = $"Режим загрузки уже установлен как {(_isBiosMode.Value ? "BIOS" : "прямая загрузка")}.";
			return null;
		}
		_isBiosMode = false;   // фиксируем прямую загрузку перед попыткой

		if (_device.TryLoadProgramFast(program, loadAddress))
		{
			error = null;
			return new LaunchModeDevice(_device);
		}

		error = $"Не удалось загрузить программу: выход за границы памяти (loadAddress + {program.Length} > RAM) или другая не инициализированная причина";
		return null;
	}

	public LaunchModeDevice LoadBios(byte[] program)
	{
		SetLoadMode(true);   // фиксируем режим BIOS
		_device.UpdateBios(program);
		return new LaunchModeDevice(_device);
	}

	public LaunchModeDevice GetLaunchMode()
	{
		return new LaunchModeDevice(_device);
	}

	public ReadOnlyMemory<byte> ReadMemory(ulong address, int length)
	{
		if (_device.IsRunning)
			throw new InvalidOperationException("Нельзя читать память во время симуляции.");
		return _device.AsRamMemory((int)address, length);
	}

	public ReadOnlyMemory<byte> ReadMemory()
	{
		if (_device.IsRunning)
			throw new InvalidOperationException("Нельзя читать память во время симуляции.");
		return _device.RamArray;
	}

	public void Dispose()
	{
		_isBiosMode = null;
		_device.Dispose();
		GC.SuppressFinalize(this);
	}

	public void ResetMemoryRam() => _device.ResetMemoryRam();
	public byte ReadPort(ulong offset) => _device.ReadPort(offset);

}
````

## File: ASM gen/Information Window/InformationWindow.xaml
````
<Window x:Class="ASM_gen.Information_Window.InformationWindow"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:local="clr-namespace:ASM_gen.Information_Window"
		mc:Ignorable="d"
		Title="InformationWindow" Height="450" Width="800">
	<Grid>
		<!-- Снижаем разрядность отрисовки для текста, чтобы убрать микрофризы при скролле -->
		<ListView x:Name="MemoryListView" 
				  VirtualizingStackPanel.IsVirtualizing="True"
				  VirtualizingStackPanel.VirtualizationMode="Recycling"
				  VirtualizingStackPanel.CacheLengthUnit="Page"
				  VirtualizingStackPanel.CacheLength="1,1"
				  ScrollViewer.IsDeferredScrollingEnabled="True"
				  FontFamily="Consolas" FontSize="12">
			<ListView.View>
				<GridView>
					<GridViewColumn Header="Адрес" DisplayMemberBinding="{Binding AddressHex}" Width="90"/>
					<GridViewColumn Header="00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F" DisplayMemberBinding="{Binding BytesHex}" Width="360"/>
					<GridViewColumn Header="ASCII" DisplayMemberBinding="{Binding AsciiText}" Width="120"/>
				</GridView>
			</ListView.View>
		</ListView>
	</Grid>
</Window>
````

## File: ASM gen/NewProjectManage/NewProjectDialog.xaml
````
<Window x:Class="ASM_gen.NewProjectManage.NewProjectDialog"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:local="clr-namespace:ASM_gen.NewProjectManage"
		mc:Ignorable="d"
		Title="Новый проект" Height="200" Width="350"
		WindowStartupLocation="CenterOwner" ResizeMode="NoResize">
	<Grid Margin="15">
		<Grid.RowDefinitions>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="*"/>
		</Grid.RowDefinitions>

		<TextBlock Text="Имя проекта:" Margin="0,5"/>
		<TextBox x:Name="ProjectNameBox" Grid.Row="1" Margin="0,5"/>

		<StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10">
			<TextBlock Text="Язык:" VerticalAlignment="Center" Margin="0,0,10,0"/>
			<ComboBox x:Name="LanguageBox" Width="120">
				<ComboBoxItem Content="C" IsSelected="True"/>
				<ComboBoxItem Content="Ассемблер"/>
			</ComboBox>
		</StackPanel>

		<StackPanel Grid.Row="3" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,10,0,0">
			<Button Content="Создать" Click="Create_Click" Width="80" Margin="0,0,10,0" IsDefault="True"/>
			<Button Content="Отмена" Click="Cancel_Click" Width="80" IsCancel="True"/>
		</StackPanel>
	</Grid>
</Window>
````

## File: ASM gen/StartWindow/MainMenu.xaml
````
<Page x:Class="ASM_gen.StartWindow.MainMenu"
	  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
	  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
	  xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
	  xmlns:local="clr-namespace:ASM_gen.StartWindow" xmlns:information_window="clr-namespace:ASM_gen.Information_Window" d:DataContext="{d:DesignInstance Type=information_window:RowViewModel}"
	  mc:Ignorable="d" 
	  d:DesignHeight="450" d:DesignWidth="800"
	  Title="MainMenu">

	<Grid Margin="10">
		<Grid.RowDefinitions>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="*"/>
		</Grid.RowDefinitions>

		<!-- Верхняя панель -->
		<StackPanel Orientation="Horizontal" Grid.Row="0" Margin="0,0,0,10">
			<TextBox x:Name="TxtPath" Width="220" Text="C:\YourFolder" Margin="0,0,5,0"/>
			<Button x:Name="BtnNewProj" Content="Создать новый проект" Padding="10,0" Click="BtnNewProj_Click"/>
		</StackPanel>

		<!-- Список результатов -->
		<ListBox x:Name="LstFiles" Grid.Row="1" HorizontalContentAlignment="Stretch" d:ItemsSource="{d:SampleData ItemCount=5}">
			<ListBox.ItemTemplate>
				<DataTemplate>
					<!-- Кнопка растягивается на весь элемент списка -->
					<!-- Tag используется для скрытого хранения полного пути к файлу -->
					<Button Click="ProjectButton_Click" 
							Tag="{Binding Key}" 
							HorizontalAlignment="Stretch" 
							HorizontalContentAlignment="Left"
							Padding="10,5"
							Background="Transparent"
							BorderThickness="0">
				
						<StackPanel>
							<StackPanel>
								<!-- Отображаем имя файла проекта (из структуры FileReadResult) -->
								<TextBlock Text="{Binding Value.FileName}" FontWeight="Bold" FontSize="14"/>

								<!-- Отображаем путь к файлу -->
								<TextBlock Text="{Binding Key}" Foreground="Gray" FontSize="10" Margin="0,2,0,0"/>

								<!-- Дополнительно: можно вывести статус ошибки, если она есть -->
								<!-- При желании этот блок можно скрыть, если ошибок нет -->
								<TextBlock Text="{Binding Value.Error}" Foreground="Red" FontSize="10" Margin="0,2,0,0"/>
							</StackPanel>
						</StackPanel>
				
					</Button>
				</DataTemplate>
			</ListBox.ItemTemplate>
		</ListBox>    
	</Grid>
</Page>
````

## File: ASM gen/StartWindow/MainWindow.xaml
````
<Window x:Class="ASM_gen.StartWindow.MainWindow"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:local="clr-namespace:ASM_gen.StartWindow" 
		xmlns:information_window="clr-namespace:ASM_gen.Information_Window" 
		d:DataContext="{d:DesignInstance Type=information_window:RowViewModel}"
		mc:Ignorable="d"
		WindowState="Maximized"
		Title="MainWindow" Height="450" Width="800">
	<Grid>
		<Frame x:Name="MainFrame" Source="MainMenu.xaml" NavigationUIVisibility="Hidden"/>
	</Grid>
</Window>
````

## File: ASM gen/StartWindow/MainWindow.xaml.cs
````csharp
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
````

## File: ASM gen/ViewModels/CreateDeviceDialog.xaml
````
<Window x:Class="ASM_gen.ViewModels.CreateDeviceDialog"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		mc:Ignorable="d"
		Title="Создание устройства" Height="450" Width="400"
		WindowStartupLocation="CenterOwner">
	<Grid Margin="10">
		<Grid.RowDefinitions>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="*"/>
			<RowDefinition Height="Auto"/>
		</Grid.RowDefinitions>
		<Grid.ColumnDefinitions>
			<ColumnDefinition Width="120"/>
			<ColumnDefinition Width="*"/>
		</Grid.ColumnDefinitions>

		<!-- RAM Size -->
		<TextBlock Grid.Row="0" Grid.Column="0" Text="RAM Size:" VerticalAlignment="Center"/>
		<ComboBox Grid.Row="0" Grid.Column="1" 
				  ItemsSource="{Binding RamSizes}" 
				  SelectedValuePath="Value"
				  SelectedValue="{Binding SelectedRamSize}"/>

		<!-- Sector -->
		<TextBlock Grid.Row="1" Grid.Column="0" Text="Sector:" VerticalAlignment="Center" Margin="5,0,0,0"/>
		<TextBox Grid.Row="1" Grid.Column="1" Text="{Binding Sector, UpdateSourceTrigger=PropertyChanged}"/>

		<!-- Device Name -->
		<TextBlock Grid.Row="2" Grid.Column="0" Text="Device Name:" VerticalAlignment="Center"/>
		<TextBox Grid.Row="2" Grid.Column="1" Text="{Binding DeviceName}"/>

		<!-- Processor Name -->
		<TextBlock Grid.Row="3" Grid.Column="0" Text="Processor Name:" VerticalAlignment="Center"/>
		<TextBox Grid.Row="3" Grid.Column="1" Text="{Binding ProcName}"/>

		<!-- RAM Name -->
		<TextBlock Grid.Row="4" Grid.Column="0" Text="RAM Name:" VerticalAlignment="Center"/>
		<TextBox Grid.Row="4" Grid.Column="1" Text="{Binding RamName}"/>

		<!-- PortBus Name -->
		<TextBlock Grid.Row="5" Grid.Column="0" Text="PortBus Name:" VerticalAlignment="Center"/>
		<TextBox Grid.Row="5" Grid.Column="1" Text="{Binding PortBusName}"/>

		<!-- BIOS -->
		<TextBlock Grid.Row="6" Grid.Column="0" Text="BIOS file:" VerticalAlignment="Center"/>
		<StackPanel Grid.Row="6" Grid.Column="1" Orientation="Horizontal">
			<TextBox Text="{Binding BiosPath}" Width="200" Margin="0,0,5,0"/>
			<Button Content="Browse..." Command="{Binding BrowseBiosCommand}" Padding="5,0"/>
		</StackPanel>

		<!-- Кнопки -->
		<StackPanel Grid.Row="8" Grid.ColumnSpan="2" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,10,0,0">
			<Button Content="Создать" Command="{Binding CreateCommand}" Width="80" Margin="0,0,10,0" IsDefault="True"/>
			<Button Content="Отмена" Command="{Binding CancelCommand}" Width="80" IsCancel="True"/>
		</StackPanel>
	</Grid>
</Window>
````

## File: ASM gen/ViewModels/CreateDeviceDialog.xaml.cs
````csharp
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
````

## File: ASM gen/App.xaml
````
<Application x:Class="ASM_gen.App"
			 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
			 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
			 xmlns:local="clr-namespace:ASM_gen"
			 xmlns:start="clr-namespace:ASM_gen.StartWindow">
	<Application.Resources>
		 
	</Application.Resources>
</Application>
````

## File: ASM gen/NewFileDialog.xaml
````
<Window x:Class="ASM_gen.NewFileDialog"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:local="clr-namespace:ASM_gen"
		mc:Ignorable="d"
		Title="Новый файл" Height="150" Width="400"
		WindowStartupLocation="CenterOwner" ResizeMode="NoResize">
	<Grid Margin="10">
		<Grid.RowDefinitions>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="*"/>
		</Grid.RowDefinitions>
		<TextBlock Text="Имя файла (без расширения):" Margin="0,5"/>
		<TextBox x:Name="FileNameBox" Grid.Row="1" Margin="0,5"/>
		<StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10">
			<TextBlock Text="Язык:" VerticalAlignment="Center" Margin="0,0,10,0"/>
			<ComboBox x:Name="LanguageBox" Width="120">
				<ComboBoxItem Content="C" IsSelected="True"/>
				<ComboBoxItem Content="Ассемблер"/>
			</ComboBox>
		</StackPanel>
		<StackPanel Grid.Row="3" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,10">
			<Button Content="OK" Click="Ok_Click" Width="70" Margin="0,0,10,0"/>
			<Button Content="Отмена" Click="Cancel_Click" Width="70"/>
		</StackPanel>
	</Grid>
</Window>
````

## File: Compiller/C/CodeGenerator/GlobalInfo.cs
````csharp
namespace Compiller.C.CodeGenerator;

public struct GlobalInfo(ulong address,
						 string type,
						 bool isArray = false,
						 bool isPointer = false,
						 string? pointedType = null)
{
	public ulong Address = address;
	public string Type = type;
	public bool IsArray = isArray;
	public bool IsPointer = isPointer;
	public string? PointedType = pointedType;
}
````

## File: Compiller/C/Optimizators/AstOptimizer.cs
````csharp
using System.Text;

namespace Compiller.C.Optimizators;

public readonly struct OutPutOptimizeText(StringBuilder removedNodes, StringBuilder inlinedFunc)
{
	public readonly StringBuilder RemovedNodes = removedNodes;
	public readonly StringBuilder InlinedFunc = inlinedFunc;
}

public static class AstOptimizer
{
	private const int MaxWeightInlineSize = 10;
	public static OutPutOptimizeText Optimize(ProgramNode program)
	{
		var removedNodes = new StringBuilder(1024);
		var inlinedFunc = new StringBuilder(1024);

		RemoveUnreachableCode(program, removedNodes);
		InlineSmallVoidFunctions(program, MaxWeightInlineSize, inlinedFunc);
		RemoveUnreachableCode(program, removedNodes); // после инлайнинга

		return new OutPutOptimizeText(removedNodes, inlinedFunc);
	}

	// ------------------------------------------------------------
	// УДАЛЕНИЕ НЕДОСТИЖИМОГО КОДА
	// ------------------------------------------------------------
	private static void RemoveUnreachableCode(ProgramNode program, StringBuilder removedNodesLog)
	{
		foreach (var func in program.Functions)
		{
			if (func.Body != null)
				OptimizeBlock(func.Body, removedNodesLog);
		}

		// Также обрабатываем глобальные инициализаторы, если они были в блоках (у нас их нет, но на будущее)
		foreach (var global in program.Globals)
		{
			if (global.Initializer is BlockNode block)
				OptimizeBlock(block, removedNodesLog);
		}
	}

	private static void OptimizeBlock(BlockNode block, StringBuilder removedNodesLog)
	{
		var newStatements = new List<ASTNode>();
		bool unreachable = false;

		foreach (var stmt in block.Statements)
		{
			if (unreachable)
			{
				// Логируем все оставшиеся как недостижимые
				for (int i = block.Statements.IndexOf(stmt); i < block.Statements.Count; i++)
				{
					removedNodesLog.AppendLine($"Unreachable node removed: {block.Statements[i].GetType().Name}");
				}
				break;
			}

			// Рекурсивно оптимизируем вложенные блоки
			switch (stmt)
			{
				case BlockNode nested:
					OptimizeBlock(nested, removedNodesLog);
					newStatements.Add(nested);
					break;

				case IfNode ifNode:
					OptimizeBlock(ifNode.ThenBlock, removedNodesLog);
					if (ifNode.ElseBlock != null) OptimizeBlock(ifNode.ElseBlock, removedNodesLog);
					newStatements.Add(ifNode);
					break;

				case WhileNode whileNode:
					OptimizeBlock(whileNode.Body, removedNodesLog);
					newStatements.Add(whileNode);
					break;

				case ForNode forNode:
					if (forNode.Body != null) OptimizeBlock(forNode.Body, removedNodesLog);
					newStatements.Add(forNode);
					break;

				case ReturnNode:
					newStatements.Add(stmt);
					unreachable = true;
					break;

				default:
					newStatements.Add(stmt);
					break;
			}
		}

		block.Statements.Clear();
		block.Statements.AddRange(newStatements);

		// Упрощение if (false) / while (false) / for(;false;)
		SimplifyControlFlow(block, removedNodesLog);
	}

	private static void SimplifyControlFlow(BlockNode block, StringBuilder removedNodesLog)
	{
		var simplified = new List<ASTNode>();
		foreach (var stmt in block.Statements)
		{
			if (stmt is IfNode ifNode)
			{
				if (IsConstantFalse(ifNode.Condition))
				{
					if (ifNode.ElseBlock != null)
					{
						removedNodesLog.AppendLine($"IfNode removed (then-block discarded, else-block kept): {ifNode.GetType().Name}");
						simplified.AddRange(ifNode.ElseBlock.Statements);
					}
					else
					{
						removedNodesLog.AppendLine($"IfNode removed (no else): {ifNode.GetType().Name}");
					}
				}
				else if (IsConstantTrue(ifNode.Condition))
				{
					removedNodesLog.AppendLine($"IfNode removed (condition always true, else-block discarded): {ifNode.GetType().Name}");
					simplified.AddRange(ifNode.ThenBlock.Statements);
				}
				else
				{
					simplified.Add(ifNode);
				}
			}
			else if (stmt is WhileNode whileNode && IsConstantFalse(whileNode.Condition))
			{
				removedNodesLog.AppendLine($"WhileNode removed (condition false): {whileNode.GetType().Name}");
			}
			else if (stmt is ForNode forNode && forNode.Condition != null && IsConstantFalse(forNode.Condition))
			{
				removedNodesLog.AppendLine($"ForNode removed (condition false): {forNode.GetType().Name}");
			}
			else
			{
				simplified.Add(stmt);
			}
		}
		block.Statements.Clear();
		block.Statements.AddRange(simplified);
	}

	private static bool IsConstantFalse(ASTNode node) =>
		node is NumberNode num && num.Value == 0;

	private static bool IsConstantTrue(ASTNode node) =>
		node is NumberNode num && num.Value != 0;

	// ------------------------------------------------------------
	// ИНЛАЙНИНГ МАЛЕНЬКИХ VOID-ФУНКЦИЙ
	// ------------------------------------------------------------
	private static void InlineSmallVoidFunctions(ProgramNode program, int maxBodySize, StringBuilder inlinedFunc)
	{
		// Собираем кандидатов: void-функции, не extern, не main, не рекурсивные, с маленьким телом
		var candidates = program.Functions
			.Where(f => f.ReturnType == "void" &&
						!f.IsExternal &&
						f.Name != "main" &&
						f.Body != null &&
						CountStatements(f.Body) <= maxBodySize)
			.ToList();

		if (candidates.Count == 0) return;

		var candidateNames = new HashSet<string>(candidates.Select(f => f.Name));

		// Проверяем рекурсию
		candidates = [.. candidates.Where(f => !ContainsCallTo(f.Body, f.Name))];

		var inlined = new HashSet<string>();

		foreach (var func in program.Functions)
		{
			if (func.Body != null && func.Name != "main")
			{
				InlineCallsInBlock(func.Body, candidates.ToDictionary(f => f.Name), inlined, inlinedFunc);
			}
		}

		// Можно удалить инлайнированные функции, если они больше не вызываются,
		// но для простоты оставим их в программе – они будут генерироваться, но не использоваться.
	}

	private static void InlineCallsInBlock(BlockNode block,
											Dictionary<string, FunctionNode> candidateMap,
											HashSet<string> inlined,
											StringBuilder inlinedFunc)
	{
		var newStatements = new List<ASTNode>();

		foreach (var stmt in block.Statements)
		{
			switch (stmt)
			{
				case FunctionCallNode call when candidateMap.TryGetValue(call.Name, out var target) &&
												target.ReturnType == "void" &&
												target.Parameters.Count == call.Arguments.Count:
					// Заменяем вызов на inline-блок
					var inlineBlock = CreateInlineBlock(call, target);
					newStatements.AddRange(inlineBlock.Statements);
					inlined.Add(target.Name);
					// Логируем факт инлайнинга
					inlinedFunc.AppendLine($"Inlined call to '{call.Name}' (replaced with inline block)");
					break;

				case IfNode ifNode:
					InlineCallsInBlock(ifNode.ThenBlock, candidateMap, inlined, inlinedFunc);
					if (ifNode.ElseBlock != null) InlineCallsInBlock(ifNode.ElseBlock, candidateMap, inlined, inlinedFunc);
					newStatements.Add(ifNode);
					break;

				case WhileNode whileNode:
					InlineCallsInBlock(whileNode.Body, candidateMap, inlined, inlinedFunc);
					newStatements.Add(whileNode);
					break;

				case ForNode forNode:
					if (forNode.Init is BlockNode initBlock) InlineCallsInBlock(initBlock, candidateMap, inlined, inlinedFunc);
					if (forNode.Increment is BlockNode incBlock) InlineCallsInBlock(incBlock, candidateMap, inlined, inlinedFunc);
					InlineCallsInBlock(forNode.Body, candidateMap, inlined, inlinedFunc);
					newStatements.Add(forNode);
					break;

				default:
					newStatements.Add(stmt);
					break;
			}
		}

		block.Statements.Clear();
		block.Statements.AddRange(newStatements);
	}

	private static BlockNode CreateInlineBlock(FunctionCallNode call, FunctionNode targetFunc)
	{
		var inlineBlock = new BlockNode();
		var renameMap = new Dictionary<string, string>();
		int uniqueId = Guid.NewGuid().GetHashCode() & 0xFFFF;

		// Присваивание аргументов параметрам (параметры переименовываются)
		for (int i = 0; i < targetFunc.Parameters.Count; i++)
		{
			var param = targetFunc.Parameters[i];
			var arg = call.Arguments[i];
			string newParamName = $"{param.Name}_inl_{uniqueId}";
			renameMap[param.Name] = newParamName;

			// Создаём локальную переменную для параметра (она будет вставлена в FunctionContext)
			var paramVar = new VariableNode(param.Type, newParamName, null)
			{
				IsPointer = param.IsPointer,
				PointedType = param.PointedType
			};
			inlineBlock.Statements.Add(paramVar);

			// Присваиваем значение аргумента
			var assign = new AssignmentNode(newParamName, arg) { LValue = new IdentifierNode(newParamName) };
			inlineBlock.Statements.Add(assign);
		}

		// Копируем тело функции с заменой имён переменных
		foreach (var stmt in targetFunc.Body.Statements)
		{
			var cloned = CloneStatementWithRename(stmt, renameMap);
			inlineBlock.Statements.Add(cloned);
		}

		return inlineBlock;
	}

	private static ASTNode CloneStatementWithRename(ASTNode node, Dictionary<string, string> renameMap)
	{
		switch (node)
		{
			case BlockNode block:
				var newBlock = new BlockNode();
				foreach (var s in block.Statements)
					newBlock.Statements.Add(CloneStatementWithRename(s, renameMap));
				return newBlock;

			case VariableNode varNode:
				string newName = varNode.Name;
				if (renameMap.TryGetValue(varNode.Name, out var mapped)) newName = mapped;
				return new VariableNode(varNode.Type, newName, CloneExpression(varNode.Initializer, renameMap))
				{
					IsArray = varNode.IsArray,
					ArraySize = varNode.ArraySize,
					IsPointer = varNode.IsPointer,
					PointedType = varNode.PointedType
				};

			case AssignmentNode assign:
				string assignName = assign.Name;
				if (renameMap.TryGetValue(assign.Name, out var mappedAssign)) assignName = mappedAssign;
				return new AssignmentNode(assignName, CloneExpression(assign.Value, renameMap)!, CloneExpression(assign.IndexExpr, renameMap))
				{
					LValue = CloneExpression(assign.LValue, renameMap)
				};

			case ReturnNode ret:
				return new ReturnNode(CloneExpression(ret.Value, renameMap));

			case IfNode ifNode:
				var newIf = new IfNode(CloneExpression(ifNode.Condition, renameMap)!,
									   (BlockNode)CloneStatementWithRename(ifNode.ThenBlock, renameMap),
									   ifNode.ElseBlock != null ? (BlockNode)CloneStatementWithRename(ifNode.ElseBlock, renameMap) : null);
				return newIf;

			case WhileNode whileNode:
				return new WhileNode(CloneExpression(whileNode.Condition, renameMap)!,
									 (BlockNode)CloneStatementWithRename(whileNode.Body, renameMap));

			case ForNode forNode:
				var newFor = new ForNode(CloneExpression(forNode.Init, renameMap),
										 CloneExpression(forNode.Condition, renameMap),
										 CloneExpression(forNode.Increment, renameMap),
										 (BlockNode)CloneStatementWithRename(forNode.Body, renameMap));
				return newFor;

			case FunctionCallNode call:
				var newCall = new FunctionCallNode(call.Name);
				foreach (var arg in call.Arguments)
					newCall.Arguments.Add(CloneExpression(arg, renameMap)!);
				return newCall;

			case BinaryOpNode bin:
				return new BinaryOpNode(bin.Operator,
										CloneExpression(bin.Left, renameMap)!,
										CloneExpression(bin.Right, renameMap)!);

			case UnaryOpNode un:
				return new UnaryOpNode(un.Operator, CloneExpression(un.Operand, renameMap)!);

			case IdentifierNode id:
				if (renameMap.TryGetValue(id.Name, out var idMapped))
					return new IdentifierNode(idMapped);
				return new IdentifierNode(id.Name);

			case ArrayAccessNode arr:
				return new ArrayAccessNode(
					renameMap.TryGetValue(arr.ArrayName, out var arrMapped) ? arrMapped : arr.ArrayName,
					CloneExpression(arr.Index, renameMap)!);

			case MemberAccessNode member:
				return new MemberAccessNode(CloneExpression(member.Object, renameMap)!,
											member.FieldName,
											member.IsArrow);

			case AddressOfNode addrOf:
				return new AddressOfNode(CloneExpression(addrOf.Operand, renameMap)!);

			case DereferenceNode deref:
				return new DereferenceNode(CloneExpression(deref.Operand, renameMap)!);

			case NewArrayNode newArr:
				return new NewArrayNode(newArr.Type, CloneExpression(newArr.Size, renameMap)!);

			default:
				// Числа и прочие листовые узлы не содержат имён переменных
				return node;
		}
	}

	private static ASTNode? CloneExpression(ASTNode? node, Dictionary<string, string> renameMap)
	{
		if (node == null) return null;
		return CloneStatementWithRename(node, renameMap);
	}

	private static int CountStatements(BlockNode block)
	{
		int count = 0;
		foreach (var stmt in block.Statements)
		{
			count++;
			switch (stmt)
			{
				case IfNode ifNode:
					count += CountStatements(ifNode.ThenBlock) + (ifNode.ElseBlock != null ? CountStatements(ifNode.ElseBlock) : 0);
					break;
				case WhileNode whileNode:
					count += CountStatements(whileNode.Body);
					break;
				case ForNode forNode:
					count += CountStatements(forNode.Body);
					break;
			}
		}
		return count;
	}

	private static bool ContainsCallTo(BlockNode block, string funcName)
	{
		foreach (var stmt in block.Statements)
		{
			if (ContainsCallToStatement(stmt, funcName))
				return true;
		}
		return false;
	}

	private static bool ContainsCallToStatement(ASTNode node, string funcName) => node switch
	{
		FunctionCallNode call => FunkCallContains(funcName, call),
		BinaryOpNode bin => ContainsCallToStatement(bin.Left, funcName) || ContainsCallToStatement(bin.Right, funcName),
		UnaryOpNode un => ContainsCallToStatement(un.Operand, funcName),
		IfNode ifNode => ContainsCallTo(ifNode.ThenBlock, funcName) ||
							   (ifNode.ElseBlock != null && ContainsCallTo(ifNode.ElseBlock, funcName)),
		WhileNode whileNode => ContainsCallTo(whileNode.Body, funcName),
		ForNode forNode => ContainsCallTo(forNode.Body, funcName),
		BlockNode block => ContainsCallTo(block, funcName),
		_ => false,
	};

	private static bool FunkCallContains(string funcName, FunctionCallNode call)
	{
		if (call.Name == funcName) return true;
		foreach (var arg in call.Arguments)
			if (ContainsCallToStatement(arg, funcName)) return true;
		return false;
	}
}
````

## File: Compiller/C/StructLayout.cs
````csharp
using Compiller.C.CodeGenerator;

namespace Compiller.C;

public class StructLayout
{
	public string Name { get; }
	public int Size { get; }
	public IReadOnlyList<FieldInfo> Fields => _fields;
	private readonly List<FieldInfo> _fields = [];

	public StructLayout(string name, List<(string type, string fieldName)> fieldDecls,
						Dictionary<string, StructLayout> structTable)
	{
		Name = name;
		int offset = 0;
		foreach (var (type, fieldName) in fieldDecls)
		{
			int align = CodeGenUtils.GetAlignment(type, structTable);
			int size = CodeGenUtils.GetTypeSize(type, structTable);
			offset = (offset + align - 1) & ~(align - 1);
			_fields.Add(new FieldInfo(fieldName, type, offset, size));
			offset += size;
		}
		Size = (offset + 7) & ~7;
	}

	public static Dictionary<string, StructLayout> Resolve(List<StructDeclNode> decls)
	{
		var layouts = new Dictionary<string, StructLayout>();
		var remaining = new Queue<StructDeclNode>(decls);
		int lastResolved;
		do
		{
			lastResolved = 0;
			int count = remaining.Count;
			for (int i = 0; i < count; i++)
			{
				var decl = remaining.Dequeue();
				// Проверяем, все ли вложенные структурные типы уже разрешены
				bool canResolve = true;
				foreach (var (type, _) in decl.Fields)
				{
					if (!CodeGenUtils.IsPrimitiveType(type) && !layouts.ContainsKey(type))
					{
						canResolve = false;
						break;
					}
				}
				if (canResolve)
				{
					layouts[decl.Name] = new StructLayout(decl.Name, decl.Fields, layouts);
					lastResolved++;
				}
				else
				{
					remaining.Enqueue(decl);
				}
			}
		} while (remaining.Count > 0 && lastResolved > 0);

		if (remaining.Count > 0)
			throw new Exception($"Cyclic or missing struct dependencies: {string.Join(", ", remaining.Select(d => d.Name))}");
		return layouts;
	}

	public FieldInfo? GetField(string name) => _fields.Find(f => f.Name == name);
}

public record FieldInfo(string Name, string Type, int Offset, int Size);
````

## File: Controllers/LocalMemorySystem/DiskDevice.cs
````csharp
using Kernel.ControllersData;
using System.Buffers.Binary;

namespace Kernel.LocalMemorySystem;

public sealed class DiskDevice : IPortUse, IDisposable
{
	public const int SectorSize = 512;

	// Смещения портов внутри сектора устройства
	public const byte PortData = 0;      // байт данных (чтение/запись с автоинкрементом)
	public const byte PortCommand = 1;   // команда (запись)
	public const byte PortStatus = 2;    // статус (чтение)
	public const byte PortLba0 = 3;      // младший байт LBA
	public const byte PortLba1 = 4;
	public const byte PortLba2 = 5;
	public const byte PortLba3 = 6;      // старший байт LBA
	public const byte PortErrorCode = 7; // код ошибки (чтение)

	// Команды
	private const byte CmdReadSector = 0x01;
	private const byte CmdWriteSector = 0x02;
	private const byte CmdIdentify = 0x03;

	private readonly FileStream _file;
	private readonly int _countSectors;
	private readonly long _sizeFile;
	private readonly byte[] _sectorBuffer = new byte[SectorSize];
	private int _bufferPos;
	private uint _currentLba;
	private bool _busy;
	private bool _error;
	private byte _errorCode;
	private readonly Lock _lock = new();

	public DiskDevice(string imagePath)
	{
		_file = new(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
		ImagePath = imagePath;
		_sizeFile = _file.Length;
		_countSectors = (int)_sizeFile / SectorSize;
	}

	public DateTime CreatedAt { get; } = DateTime.Now;
	public long SizeDisk => _sizeFile;
	public int CountSectors => _countSectors;
	public string ImagePath { get; }


	public byte[] SectorBuffer => _sectorBuffer;
	public byte ReadPort(ulong offset)
	{
		lock (_lock)
		{
			return offset switch
			{
				PortData => ReadData(),
				PortStatus => GetStatus(),
				PortErrorCode => _errorCode,
				_ => 0
			};
		}
	}

	public void WritePort(ulong offset, byte value)
	{
		lock (_lock)
		{
			switch (offset)
			{
				case PortData: WriteData(value); break;
				case PortCommand: ExecuteCommand(value); break;
				case PortLba0: _currentLba = (_currentLba & 0xFFFFFF00) | value; break;
				case PortLba1: _currentLba = (_currentLba & 0xFFFF00FF) | ((uint)value << 8); break;
				case PortLba2: _currentLba = (_currentLba & 0xFF00FFFF) | ((uint)value << 16); break;
				case PortLba3: _currentLba = (_currentLba & 0x00FFFFFF) | ((uint)value << 24); break;
			}
		}
	}

	private void ExecuteCommand(byte command)
	{
		_busy = true;
		_error = false;
		_errorCode = 0;

		switch (command)
		{
			case CmdReadSector: ReadSector(); break;
			case CmdWriteSector: WriteSector(); break;
			case CmdIdentify: Identify(); break;
			default:
				_error = true;
				_errorCode = 0x01; // неизвестная команда
				break;
		}

		_busy = false;
	}

	private void ReadSector()
	{
		long offset = (long)_currentLba * SectorSize;
		if (offset + SectorSize > _file.Length)
		{
			_error = true;
			_errorCode = 0x02; // выход за границы диска
			return;
		}
		_file.Seek(offset, SeekOrigin.Begin);
		_file?.Read(_sectorBuffer, 0, SectorSize);
		_bufferPos = 0;
	}

	private void WriteSector()
	{
		long offset = (long)_currentLba * SectorSize;
		if (offset + SectorSize > _file.Length)
		{
			_error = true;
			_errorCode = 0x02;
			return;
		}
		_file.Seek(offset, SeekOrigin.Begin);
		_file.Write(_sectorBuffer, 0, SectorSize);
		_file.Flush(); // гарантируем запись на физический диск
		_bufferPos = 0;
	}

	private void Identify()
	{
		Array.Clear(_sectorBuffer, 0, SectorSize);
		uint totalSectors = (uint)(_file.Length / SectorSize);
		BinaryPrimitives.WriteUInt32LittleEndian(_sectorBuffer.AsSpan(0, 4), totalSectors);
		BinaryPrimitives.WriteUInt16LittleEndian(_sectorBuffer.AsSpan(4, 2), SectorSize);
		_bufferPos = 0;
	}

	private byte ReadData()
	{
		if (_bufferPos >= SectorSize) return 0;
		return _sectorBuffer[_bufferPos++];
	}

	private void WriteData(byte value)
	{
		if (_bufferPos < SectorSize)
			_sectorBuffer[_bufferPos++] = value;
	}

	private byte GetStatus()
	{
		byte status = 0;
		if (_busy) status |= 0x01;
		if (!_busy && !_error) status |= 0x02; // READY
		if (_error) status |= 0x04;
		return status;
	}

	public void WakeProcessor() { }

	public void Dispose()
	{
		_file?.Dispose();
		GC.SuppressFinalize(this);
	}

	public byte[]? ReadSectorDirect(uint lba)
	{
		var result = new byte[SectorSize];
		long offset = lba * SectorSize;
		if (offset + SectorSize > _file.Length)
		{
			_error = true;
			_errorCode = 0x02; // выход за границы диска
			return null;
		}
		_file.Seek(offset, SeekOrigin.Begin);
		_file?.Read(result, 0, SectorSize);
		return result;
	}

	public bool WriteSectorDirect(uint lba, ReadOnlySpan<byte> data)
	{
		if (data.Length != SectorSize)
			throw new ArgumentException($"Длина данных должна быть ровно {SectorSize} байт", nameof(data));

		long offset = (long)lba * SectorSize;
		if (offset + SectorSize > _file.Length)
		{
			_error = true;
			_errorCode = 0x02;
			return false;
		}

		lock (_lock)
		{
			_file.Seek(offset, SeekOrigin.Begin);
			_file.Write(data);
			_file.Flush();
		}
		return true;
	}

	public bool WriteSectorDirect(uint lba, byte[] data) => WriteSectorDirect(lba, data.AsSpan());
}
````

## File: Controllers/RamSystem/MemoryBusHelpers.cs
````csharp
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Kernel.RamSystem;

internal static class MemoryBusHelpers
{

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RAMResultInt16 GenerateInt16Le(ReadOnlySpan<byte> span)
	{
		return new RAMResultInt16(BinaryPrimitives.ReadUInt16LittleEndian(span));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RAMResultInt32 GenerateInt32Le(ReadOnlySpan<byte> span)
	{
		return new RAMResultInt32(BinaryPrimitives.ReadUInt32LittleEndian(span));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RAMResultInt64 GenerateInt64Le(ReadOnlySpan<byte> span)
	{
		return new RAMResultInt64(BinaryPrimitives.ReadUInt64LittleEndian(span));
	}
}
````

## File: Controllers/RamSystem/NativeMemoryPool.cs
````csharp
using Kernel.Common;
using System.Collections.Concurrent;

namespace Kernel.RamSystem;

public static class NativeMemoryPool
{
	private static readonly ConcurrentDictionary<RamSize, ConcurrentBag<NativeMemoryBuffer>> _pools = new();

	public static NativeMemoryBuffer Rent(RamSize size)
	{
		var bag = _pools.GetOrAdd(size, _ => []);
		if (bag.TryTake(out var buffer))
			return buffer;
		return new NativeMemoryBuffer(size);
	}

	public static void Return(NativeMemoryBuffer buffer)
	{
		if (buffer == null) return;
		// опционально: очистить память
		buffer.AsSpan().Clear();
		var bag = _pools.GetOrAdd((RamSize)buffer.Length, _ => []);
		bag.Add(buffer);
	}
}
````

## File: Kernel.Common/BiosStatus.cs
````csharp
namespace Kernel.Common;

public enum BiosStatus : byte
{
	/// <summary> Выполнено </summary>
	Success,
	/// <summary> Выход за границы ОЗУ </summary>
	SegmentationFault,

	/// <summary> Попытка прочесть int по невыровненному адресу (например, 0x03) </summary>
	AlignmentFault,

	/// <summary> Попытка чтения из защищенной области памяти </summary>
	ReadViolation,

	/// <summary> Не ошибка, остановка программы </summary>
	EndProgramm,

	/// <summary> Неизвестная команда OpCode </summary>
	NotImplementedOpCode,

	NullDeviceInput,

	NullDeviceOutput,

	InfinityLoopWarning,

	DivOnZero,
}
````

## File: Kernel.Common/DecodeInstructionResult.cs
````csharp
namespace Kernel.Common;

public readonly struct DecodeInstructionResult(OpCode opCode,
											   RegType reg1,
											   RegType reg2,
											   OpCodeSize dataSizeCode)
{
	public readonly OpCode OpCode = opCode;
	public readonly RegType Reg1 = reg1;
	public readonly RegType Reg2 = reg2;
	public readonly OpCodeSize DataSizeCode = dataSizeCode;
}
````

## File: Kernel.Common/ILogger.cs
````csharp
namespace Kernel.Common;

/// <summary>
/// Абстракция для вывода сообщений (без привязки к конкретному UI).
/// </summary>
public interface ILogger
{
	void Info(string message);
	void Warning(string message);
	void Error(string message);
	void Clear();
}
````

## File: Kernel.Common/InstructionDecoder.cs
````csharp
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Kernel.Common;

public static class InstructionDecoder
{

	private static readonly ImmutableArray<bool> _has = CreateNeedsUlongOperandTable();


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OpCode GetOpCode(uint rawInst) =>
		(OpCode)(rawInst & 0xFF);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RegType GetReg1(uint rawInst) =>
		(RegType)((rawInst >> 8) & 0x1F);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RegType GetReg2(uint rawInst) =>
		(RegType)((rawInst >> 13) & 0x1F);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OpCodeSize GetDataSizeCode(uint rawInst) =>
		(OpCodeSize)((rawInst >> 18) & 0x3);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DecodeInstructionResult DecodeRawInstToEnums(uint rawInst)
	{
		OpCode opCode = (OpCode)(rawInst & 0xFF);

		RegType reg1 = (RegType)((rawInst >> 8) & 0x1F);
		RegType reg2 = (RegType)((rawInst >> 13) & 0x1F);

		OpCodeSize dataSizeCode = (OpCodeSize)((rawInst >> 18) & 0x3);
		return new DecodeInstructionResult(opCode, reg1, reg2, dataSizeCode);
	}

	public static bool HasNeed64IntData(OpCode opCode)
	{
		return _has[(int)opCode];
	}

	public static ImmutableArray<bool> CreateNeedsUlongOperandTable()
	{
		bool[] table = new bool[256];

		table[(int)OpCode.LDI] = true;      // константа
		table[(int)OpCode.LOAD] = true;     // адрес памяти
		table[(int)OpCode.STORE] = true;    // адрес памяти
		table[(int)OpCode.CALL] = true;     // адрес подпрограммы
		table[(int)OpCode.JMP] = true;      // адрес перехода
		table[(int)OpCode.JZ] = true;       // адрес перехода
		table[(int)OpCode.JNZ] = true;      // адрес перехода
		table[(int)OpCode.JG] = true;       // адрес перехода
		table[(int)OpCode.JL] = true;       // адрес перехода

		return [.. table];
	}

	extension(RegType reg)
	{
		public int IndexReg() => (int)reg;
		public int Int => (int)reg;
	}
}

public static class AlignmentExtensions
{
	extension<T>(T value) where T : IBinaryInteger<T>
	{
		/// <summary>
		/// Выравнивает значение вверх до ближайшего числа, кратного alignment.
		/// alignment должно быть степенью двойки (1, 2, 4, 8, ...).
		/// </summary>
		public T AlignUp(T alignment)
		{
			IsOutOfRangeException(alignment);
			return (value + (alignment - T.One)) & ~(alignment - T.One);
		}

		/// <summary>
		/// Выравнивает значение вниз до ближайшего числа, кратного alignment.
		/// alignment должно быть степенью двойки (1, 2, 4, 8, ...).
		/// </summary>
		public T AlignDown(T alignment)
		{
			IsOutOfRangeException(alignment);
			return value & ~(alignment - T.One);
		}

		public T AlignUpArithmetic(T alignment)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(alignment, T.Zero);

			T remainder = value % alignment;
			if (remainder == T.Zero)
				return value;
			return value + (alignment - remainder);
		}
	}

	[Conditional("DEBUG")]
	private static void IsOutOfRangeException<T>(T alignment) where T : IBinaryInteger<T>
	{
		if (alignment <= T.Zero || !IsPowerOfTwo(alignment))
			GenerateOutOfRangeException(nameof(alignment));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void GenerateOutOfRangeException(string name)
	{
		throw new ArgumentOutOfRangeException(name, "Выравнивание должно быть степенью двойки и больше нуля.");
	}
	private static bool IsPowerOfTwo<T>(T value) where T : IBinaryInteger<T>
	{
		return (value & (value - T.One)) == T.Zero;
	}
}
````

## File: Kernel.Common/Kernel.Common.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>net10.0</TargetFramework>
	<ImplicitUsings>enable</ImplicitUsings>
	<Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
````

## File: Kernel.Common/LoggerProvider.cs
````csharp
namespace Kernel.Common;

/// <summary>
/// Статический провайдер, через который библиотеки отправляют сообщения.
/// </summary>
public static class LoggerProvider
{
	private static readonly Dictionary<string, ILogger> _loggers = [];
	private static readonly AsyncLocal<string?> _currentLoggerName = new();
	public static ILogger? Current => CurrentLogger;

	public static void RegistryLogger(string name, ILogger logger)
	{
		lock (_loggers)
			_loggers[name] = logger;
	}

	public static void DeleteLogger(string name)
	{
		lock (_loggers)
			_loggers.Remove(name);
	}

	/// <summary>Задать активный логгер для текущего потока/контекста.</summary>
	public static void ChoiceLogger(string name)
	{
		_currentLoggerName.Value = name;
	}

	private static ILogger? CurrentLogger
	{
		get
		{
			var name = _currentLoggerName.Value;
			if (name == null) return null;
			lock (_loggers)
				return _loggers.TryGetValue(name, out var logger) ? logger : null;
		}
	}

	public static bool Has(string name)
	{
		lock (_loggers)
			return _loggers.ContainsKey(name);
	}
	public static IReadOnlyCollection<string> GetAllRegisteredLoggers()
	{
		lock (_loggers)
			return [.. _loggers.Keys];
	}

	public static void Info(char c) => CurrentLogger?.Info(c.AsText);
	public static void Info(string message) => CurrentLogger?.Info(message);
	public static void Warning(string message) => CurrentLogger?.Warning(message);
	public static void Error(string message) => CurrentLogger?.Error(message);
	public static void Clear() => CurrentLogger?.Clear();
}
````

## File: Kernel.Common/LogLevel.cs
````csharp
namespace Kernel.Common;

public enum LogLevel
{
	Log,
	Warning,
	Error
}
````

## File: Kernel.Common/OpCodeSize.cs
````csharp
namespace Kernel.Common;

public enum OpCodeSize
{
	S8 = 0b0,
	S16 = 0b1,
	S32 = 0b10,
	S64 = 0b11,
}
````

## File: Kernel.Common/RamSize.cs
````csharp
using System.ComponentModel;

namespace Kernel.Common;

public enum RamSize : ulong
{
	[Description("128 байт")] Size128B = 1U << 7,
	[Description("256 байт")] Size256B = 1U << 8,
	[Description("512 байт")] Size512B = 1U << 9,

	[Description("1 КБ")] Size1KB = 1U << 10,
	[Description("4 КБ")] Size4KB = 1U << 12,
	[Description("8 КБ")] Size8KB = 1U << 13,
	[Description("16 КБ")] Size16KB = 1U << 14,
	[Description("64 КБ")] Size64KB = 1U << 16,
	[Description("128 КБ")] Size128KB = 1U << 17,
	[Description("256 КБ")] Size256KB = 1U << 18,
	[Description("512 КБ")] Size512KB = 1U << 19,

	[Description("1 МБ")] Size1MB = 1U << 20,
	[Description("4 МБ")] Size4MB = 1U << 22,
	[Description("8 МБ")] Size8MB = 1U << 23,
	[Description("16 МБ")] Size16MB = 1U << 24,
	[Description("32 МБ")] Size32MB = 1U << 25,
	[Description("64 МБ")] Size64MB = 1U << 26,
	[Description("128 МБ")] Size128MB = 1U << 27,
}
````

## File: Tests/DiskBootTests.cs
````csharp
using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ProcessorSystem;
using System.Text;
using Xunit.Abstractions;

namespace Tests;

public class DiskBootTests(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _outPut = output;

	private class TestLogger : ILogger
	{
		private readonly StringBuilder _sb = new();
		private static readonly Lock _lock = new();

		public void Clear() { lock (_lock) _sb.Clear(); }
		public void Error(string message) => Append("ERR", message);
		public void Info(string message) => Append("INF", message);
		public void Warning(string message) => Append("WRN", message);

		private void Append(string level, string message)
		{
			lock (_lock) _sb.AppendLine($"[{level}] {message}");
		}

		public override string ToString() => _sb.ToString();
		public bool Contains(string text) => _sb.ToString().Contains(text);
	}

	[Fact]
	public void BiosLoadsProgramFromDisk()
	{
		const string BiosAsm = 
			"""
			// BIOS bootloader
			// Диск расположен в секторе 0 портов (базовый адрес портов диска = 0)
			// Порты диска:
			//   0 – данные (автоинкремент)
			//   1 – команда
			//   2 – статус
			//   3..6 – LBA (little-endian)

			// --- Устанавливаем LBA = 0 ---
			LDI r0, 0
			LDI r1, 3
			OUT r0, r1          // PortLba0
			LDI r1, 4
			OUT r0, r1          // PortLba1
			LDI r1, 5
			OUT r0, r1          // PortLba2
			LDI r1, 6
			OUT r0, r1          // PortLba3

			// --- Команда чтения сектора (CmdReadSector = 0x01) ---
			LDI r0, 1
			LDI r1, 1
			OUT r0, r1          // PortCommand

			// --- Ожидание готовности (busy-бит сброшен) ---
			wait_ready:
				LDI r1, 2       // PortStatus
				IN  r0, r1
				LDI r2, 1       // маска busy
				AND r0, r2
				JNZ wait_ready  // если busy != 0, ждём

			// --- Читаем 8 байт размера программы во временный буфер 0x200..0x207 ---
			LDI r10, 0x200     // адрес буфера
			LDI r11, 0         // счётчик
			LDI r12, 8         // количество байт
			read_size_loop:
				LDI r1, 0      // PortData
				IN  r0, r1
				STORE_IND.S8 r0, r10
				INC r10
				INC r11
				SUB r12, r12, 1
				JNZ read_size_loop

			// --- Загружаем размер из буфера в r8 ---
			LDI r10, 0x200     // сбрасываем указатель на буфер
			LOAD_IND.S64 r8, r10

			// --- Копируем программу из порта данных в ОЗУ, начиная с адреса 0 ---
			LDI r10, 0         // адрес назначения в ОЗУ
			copy_loop:
				// проверка: если r8 == 0, завершить
				LDI r12, 0
				SUB r12, r8, r12   // r12 = r8, флаги обновляются
				JZ boot_done

				// читаем байт из порта данных
				LDI r1, 0
				IN  r0, r1
				// записываем байт в ОЗУ
				STORE_IND.S8 r0, r10

				INC r10
				DEC r8
				JMP copy_loop

			boot_done:
				JMP 0

			""";
		const string TestProgramAsm = 
			"""
			LDI r0, 65
			PRINT_INT r0
			END
			""";

		const string loggerName = "disk-test";
		var logger = new TestLogger();
		LoggerProvider.RegistryLogger(loggerName, logger);
		LoggerProvider.ChoiceLogger(loggerName);

		SizePortOnDevice sizePOD = SizePortOnDevice.Size16B;

		var emu = new Emulator(SizePort.Size16KB, sizePOD);
		Device? device = null;
		Lock loc = new();
		uint sectorDevice = 4;
		uint idSectorDisk = 0;
		int diskCodeError = 0;
		string? imagePath = null;
		try
		{
			// 1. Создаём устройство с BIOS (пока без программы)
			byte[] biosCode = new AssemblerParser().Assemble(BiosAsm, (ulong)SizePort.Size16KB);
			int deviceId = emu.CreateDevice(biosCode, RamSize.Size16KB, sectorDevice, "BootDevice");
			Assert.InRange(deviceId, 0, int.MaxValue);
			logger.Info($"Device Id = {deviceId}");
			device = emu.GetDevice(deviceId);
			Assert.NotNull(device);

			// 2. Готовим образ диска
			byte[] program = new AssemblerParser().Assemble(TestProgramAsm);
			Span<byte> diskImage = CreateBootableImage(program, sectorCount: 1);

			imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".vmg");
			File.WriteAllBytes(imagePath, diskImage);

			// 3. Создаём диск, передавая уже готовый файл
			diskCodeError = emu.CreateDisk(imagePath, idSectorDisk);

			if (diskCodeError != 0)
				_outPut.WriteLine($"Code Error: {diskCodeError}");

			Assert.InRange(diskCodeError,0, int.MaxValue);
			Assert.NotNull(emu.GetDisk(idSectorDisk));
			
			for (ulong i = 0; i < 8; i++)
			{
				var res1 = emu.PortBus.ReadPort(i);
				if (res1.IsSuccess)
				{
					logger.Info($"1 Порт[{i}] устройство, владелец:{res1.NameDeviceToken}");
				}
				else
				{
					Assert.True(res1.IsSuccess, $"Port [{i}], {ProcessorHelpers.TryContinueAfterStatus(new(res1.Status, res1.FaultAddress), i, loc)}");
				}
			}

			// 4. Запускаем устройство с BIOS (стартовый адрес = RamSize)
			using var done = new ManualResetEventSlim(false);
			device.LaunchDeviceOnDedicatedThread(device.MaxRamSize, false, 0, false,
				title => title("Запуск", LogLevel.Log),
				null,
				end => { end("Завершено", LogLevel.Log); done.Set(); });

			// 5. Ждём завершения
			bool finished = done.Wait(TimeSpan.FromSeconds(10));
			Assert.True(finished, "Устройство не завершило выполнение за 10 секунд.");

			device.StopDevice(200, 
				log => log.Invoke("Остановка устройства", LogLevel.Log),
				log => log.Invoke("Устройство уже остановлено или не запускалось", LogLevel.Log), 
				log => log.Invoke("Устройство успешно остановлено", LogLevel.Log), 
				log => log.Invoke("По неизвестной причине устрйоство продолжает работу", LogLevel.Log));


			logger.Info(imagePath);
			Assert.Contains("65", logger.ToString());
		}
		finally
		{
			if (diskCodeError == 0)
				emu.RemoveDisk(idSectorDisk);
			if (imagePath != null && File.Exists(imagePath))
				File.Delete(imagePath);
			device?.Dispose();
			emu.Reset();
			_outPut.WriteLine(logger.ToString());
			LoggerProvider.DeleteLogger(loggerName);
		}
	}

	private static byte[] CreateBootableImage(byte[] program, int sectorCount = 1)
	{
		int sectorSize = 512; // DiskDevice.SectorSize

		Span<byte> image = stackalloc byte[sectorSize * sectorCount];
		// Записываем размер программы в первые 8 байт
		BitConverter.TryWriteBytes(image[..8], (ulong)program.Length);
		// Копируем программу
		program.CopyTo(image[8..]);
		return image.ToArray();
	}


	[Fact]
	public void DiskReadSingleByte()
	{
		const string ReadDiskProgramAsm = @"
LDI r0, 0
LDI r1, 19
OUT r0, r1
LDI r1, 20
OUT r0, r1
LDI r1, 21
OUT r0, r1
LDI r1, 22
OUT r0, r1
LDI r0, 1
LDI r1, 17
OUT r0, r1
wait_ready:
	LDI r1, 18
	IN  r0, r1
	LDI r2, 1
	AND r0, r2
	JNZ wait_ready
	LDI r1, 16
	IN  r0, r1
	PRINT_INT r0
	IN  r0, r1
	PRINT_INT r0
	END
";

	const string loggerName = "disk-single-byte";
		var logger = new TestLogger();
		LoggerProvider.RegistryLogger(loggerName, logger);
		LoggerProvider.ChoiceLogger(loggerName);

		var emu = new Emulator(SizePort.Size16KB, SizePortOnDevice.Size16B);
		Device? device = null;
		uint diskSector = 1;
		int diskCodeError = 0;
		string? imagePath = null;

		try
		{
			// Устройство в секторе 0
			int deviceId = emu.CreateDevice([], RamSize.Size16KB, 0, "ReadDevice");
			Assert.InRange(deviceId, 0, int.MaxValue);
			device = emu.GetDevice(deviceId);
			Assert.NotNull(device);

			// Диск в секторе 1
			Span<byte> diskImage = stackalloc byte[512];
			diskImage[0] = 65;
			diskImage[1] = 66;
			imagePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".vmg");
			File.WriteAllBytes(imagePath, diskImage);

			diskCodeError = emu.CreateDisk(imagePath, diskSector);
			Assert.InRange(diskCodeError, 0, int.MaxValue);

			// Программа чтения байта
			byte[] program = new AssemblerParser().Assemble(ReadDiskProgramAsm);
			device.LoadProgram(program, 0);
			device.InitHeap((ulong)program.Length);

			using var done = new ManualResetEventSlim(false);
			device.LaunchDeviceOnDedicatedThread(0, false, 0, false,
				title => title("Запуск", LogLevel.Log),
				null,
				end => { end("Завершено", LogLevel.Log); done.Set(); });

			Assert.True(done.Wait(TimeSpan.FromSeconds(10)), "Устройство не завершилось.");

			Assert.Contains("65", logger.ToString());
			Assert.Contains("66", logger.ToString());
		}
		finally
		{
			device?.Dispose();
			if (diskCodeError == 0) emu.RemoveDisk(diskSector);
			if (imagePath != null && File.Exists(imagePath)) File.Delete(imagePath);
			emu.Reset();
			_outPut.WriteLine(logger.ToString());
			LoggerProvider.DeleteLogger(loggerName);
		}
	}

}
````

## File: Tests/TestIOInAndOutSystem.cs
````csharp
using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using Xunit.Abstractions;

namespace Tests;

public class TestIOInAndOutSystem(ITestOutputHelper outPut)
{
	private readonly ITestOutputHelper _outPut = outPut;


	[Fact]
	public void TestReadingLocalIOMemory()
	{
		const string NameLogger = "default";
		const string Code_First_Device_Test_1 = @"
LDI r0, 0x42    // значение для записи
LDI r1, 0       // адрес порта 0 (глобальный адрес для сектора 0)
OUT r0, r1      // записать в порт 0

LDI r1, 0       // тот же порт
IN  r0, r1      // прочитать из порта 0 в r0

PRINT_INT r0    // выведет 66 (0x42)

END";

		var log = new LoggerOutPut();
		LoggerProvider.RegistryLogger(NameLogger, log);
		LoggerProvider.ChoiceLogger(NameLogger);

		Emulator emu = new(SizePort.Size16KB, SizePortOnDevice.Size16B);

		Device? deviceFirst = null;

		try
		{
			int deviceIdFirst = emu.CreateDevice([], RamSize.Size16KB, 0, "Device");
			Assert.InRange(deviceIdFirst, 0, int.MaxValue);

			deviceFirst = emu.GetDevice(deviceIdFirst);

			Assert.NotNull(deviceFirst);

			deviceFirst.ActionOnWake = OnWake;

			byte[]? programFirstDevice = GetByteCode(Code_First_Device_Test_1);
			Assert.NotNull(programFirstDevice);

			deviceFirst.LoadProgram(programFirstDevice, 0);
			deviceFirst.InitHeap((ulong)programFirstDevice.LongLength);

			using var doneFirst = new ManualResetEventSlim(false);

			deviceFirst.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneFirst.Set(); });

			bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));

			Assert.True(firstFinished, "Первое устройство не завершилось за отведённое время.");
			Assert.InRange(deviceFirst.CurrentIP, 0UL, (ulong)programFirstDevice.LongLength);
			Assert.False(deviceFirst.IsRunning);

			_outPut.WriteLine(log.ToString());
		}
		finally
		{
			deviceFirst?.Dispose();
			emu.Reset(); // или emu.Dispose(), если есть
			_outPut.WriteLine(log.ToString());
			LoggerProvider.DeleteLogger(NameLogger);
		}

	}

	[Fact]
	public void TestIOMemory()
	{
		const string NameLogger = "default";
		const string Code_First_Device_Test_1 = 
	@"
	LDI r0, 0xAA    // значение
	LDI r1, 16      // глобальный адрес порта второго устройства
	OUT r0, r1      // записать

	END
	";
		const string Code_Second_Device_Test_1 =
	@"
	LDI r1, 16      // тот же глобальный адрес порта
	IN  r0, r1      // прочитать в r0

	PRINT_INT r0    // выведет 170 (0xAA)

	END
	";
		var log = new LoggerOutPut();
		LoggerProvider.RegistryLogger(NameLogger, log);
		LoggerProvider.ChoiceLogger(NameLogger);


		Emulator emu = new(SizePort.Size64KB, SizePortOnDevice.Size16B);

		Device? deviceFirst = null;
		Device? deviceSecond = null;

		try
		{
			int deviceIdFirst = emu.CreateDevice([], RamSize.Size16KB, 0, "Device First");
			Assert.InRange(deviceIdFirst, 0, int.MaxValue);

			int deviceIdSecond = emu.CreateDevice([], RamSize.Size16KB, 1, "Device Second");
			Assert.InRange(deviceIdSecond, 0, int.MaxValue);

			deviceFirst = emu.GetDevice(deviceIdFirst);
			deviceSecond = emu.GetDevice(deviceIdSecond);

			Assert.NotNull(deviceFirst);
			Assert.NotNull(deviceSecond);

			deviceFirst.ActionOnWake = OnWake;
			deviceSecond.ActionOnWake = OnWake;

			byte[]? programFirstDevice = GetByteCode(Code_First_Device_Test_1);
			Assert.NotNull(programFirstDevice);

			deviceFirst.LoadProgram(programFirstDevice, 0);
			deviceFirst.InitHeap((ulong)programFirstDevice.LongLength);

			byte[]? programSecondDevice = GetByteCode(Code_Second_Device_Test_1);
			Assert.NotNull(programSecondDevice);

			deviceSecond.LoadProgram(programSecondDevice, 0);
			deviceSecond.InitHeap((ulong)programSecondDevice.LongLength);

			// После загрузки программ, перед запуском:
			using var doneFirst = new ManualResetEventSlim(false);
			using var doneSecond = new ManualResetEventSlim(false);

			// Запускаем с колбэками, которые сигнализируют о завершении
			deviceFirst.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneFirst.Set(); });
			deviceSecond.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneSecond.Set(); });

			// Ждём завершения обоих устройств (максимум 10 секунд каждое)
			bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));
			bool secondFinished = doneSecond.Wait(TimeSpan.FromSeconds(10));

			Assert.True(firstFinished, "Первое устройство не завершилось за отведённое время.");
			Assert.True(secondFinished, "Второе устройство не завершилось за отведённое время.");

			Assert.InRange(deviceFirst.CurrentIP, 0UL, (ulong)programFirstDevice.LongLength);
			Assert.InRange(deviceSecond.CurrentIP, 0UL, (ulong)programSecondDevice.LongLength);

			Assert.False(deviceFirst.IsRunning);
			Assert.False(deviceSecond.IsRunning);

			_outPut.WriteLine(log.ToString());
		}
		finally
		{
			deviceFirst?.Dispose();
			deviceSecond?.Dispose();
			emu.Reset(); // или emu.Dispose(), если есть
			_outPut.WriteLine(log.ToString());
			LoggerProvider.DeleteLogger(NameLogger);
		}

	}

	private static void OnEnd(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство выключилось штатно", LogLevel.Log);
	}

	private static void OnTitle(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство запущено", LogLevel.Log);
	}
	private static void OnWake(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство проснулось", LogLevel.Log);
	}


	private static byte[]? GetByteCode(string text)
	{
		try
		{
			var code = new AssemblerParser();
			return code.Assemble(text);
		}
		catch (Exception ex)
		{
			LoggerKernel.LogFromSystem("Compiler", ex.Message, LogLevel.Error);
			return null;
		}
	}
}
````

## File: Tests/TestIOInAndOutWake.cs
````csharp
using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using Xunit.Abstractions;

namespace Tests;

[Collection("StrictlySingleExecutionCollection")]
public class TestIOInAndOutWake(ITestOutputHelper outPut)
{
	private readonly ITestOutputHelper _outPut = outPut;

	[Fact]
	public void TestIOSincWAKE_INT()
	{
		const string NameLogger = "default";
		const string Code_First_Device_Test_1 =
		"""

	LDI r0, 16
	LDI r1, 16
	OUT r0, r1
	NOP
	LDI r0, 16      // адрес порта, через который будим
	WAKE_INT r0     // разбудить устройство, чей порт 16
	PRINT_INT r0

	END
	""";
		const string Code_Second_Device_Test_1 =
		"""
	HALT            // уснуть

	LDI r1, 16
	IN  r0, r1
	PRINT_INT r0

	END
	""";
		var log = new LoggerOutPut();
		LoggerProvider.RegistryLogger(NameLogger, log);
		LoggerProvider.ChoiceLogger(NameLogger);


		Emulator emu = new(SizePort.Size64KB, SizePortOnDevice.Size16B);

		Device? deviceFirst = null;
		Device? deviceSecond = null;

		try
		{
			int deviceIdFirst = emu.CreateDevice([], RamSize.Size16KB, 0, "Device First");
			Assert.InRange(deviceIdFirst, 0, int.MaxValue);

			int deviceIdSecond = emu.CreateDevice([], RamSize.Size16KB, 1, "Device Second");
			Assert.InRange(deviceIdSecond, 0, int.MaxValue);

			deviceFirst = emu.GetDevice(deviceIdFirst);
			deviceSecond = emu.GetDevice(deviceIdSecond);

			Assert.NotNull(deviceFirst);
			Assert.NotNull(deviceSecond);

			deviceFirst.ActionOnWake = OnWake;
			deviceSecond.ActionOnWake = OnWake;

			byte[]? programFirstDevice = GetByteCode(Code_First_Device_Test_1);
			Assert.NotNull(programFirstDevice);

			deviceFirst.LoadProgram(programFirstDevice, 0);
			deviceFirst.InitHeap((ulong)programFirstDevice.LongLength);

			byte[]? programSecondDevice = GetByteCode(Code_Second_Device_Test_1);
			Assert.NotNull(programSecondDevice);

			deviceSecond.LoadProgram(programSecondDevice, 0);
			deviceSecond.InitHeap((ulong)programSecondDevice.LongLength);

			// После загрузки программ, перед запуском:
			using var doneFirst = new ManualResetEventSlim(false);
			using var doneSecond = new ManualResetEventSlim(false);

			// Запускаем с колбэками, которые сигнализируют о завершении
			deviceFirst.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneFirst.Set(); });
			deviceSecond.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneSecond.Set(); });

			// Ждём завершения обоих устройств (максимум 10 секунд каждое)
			bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));
			bool secondFinished = doneSecond.Wait(TimeSpan.FromSeconds(10));

			var firstIp = deviceFirst.CurrentIP;
			var secondIp = deviceSecond.CurrentIP;

			Assert.InRange(firstIp, 0UL, (ulong)programFirstDevice.LongLength);
			Assert.InRange(secondIp, 0UL, (ulong)programSecondDevice.LongLength);

			Assert.True(firstFinished, $"Первое устройство не завершилось за отведённое время. Ip [{firstIp}]");
			Assert.True(secondFinished, $"Второе устройство не завершилось за отведённое время. Ip [{secondIp}]");

			Assert.False(deviceFirst.IsRunning);
			Assert.False(deviceSecond.IsRunning);

			_outPut.WriteLine(log.ToString());
		}
		finally
		{
			deviceFirst?.Dispose();
			deviceSecond?.Dispose();
			emu.Reset(); // или emu.Dispose(), если есть
			_outPut.WriteLine(log.ToString());
			LoggerProvider.DeleteLogger(NameLogger);
		}

	}

	private static void OnEnd(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство выключилось штатно", LogLevel.Log);
	}

	private static void OnTitle(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство запущено", LogLevel.Log);
	}
	private static void OnWake(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство проснулось", LogLevel.Log);
	}


	private static byte[]? GetByteCode(string text)
	{
		try
		{
			var code = new AssemblerParser();
			return code.Assemble(text);
		}
		catch (Exception ex)
		{
			LoggerKernel.LogFromSystem("Compiler", ex.Message, LogLevel.Error);
			return null;
		}
	}
}
````

## File: Tests/TestIOSleepAndWake.cs
````csharp
using Compiller.ASM;
using Compiller.Emulation;
using Kernel.BiosSystem;
using Kernel.Common;
using System.Text;
using Xunit.Abstractions;

namespace Tests;

public class LoggerOutPut : ILogger
{
	private readonly StringBuilder _output = new();
	private static readonly Lock _lock = new();

	public void Clear()
	{
		lock (_lock)
			_output.Clear();
	}

	public void Error(string message)
	{
		lock (_lock)
			_output.AppendLine(message);
	}

	public void Info(string message)
	{
		lock (_lock)
			_output.AppendLine(message);
	}

	public void Warning(string message)
	{
		lock (_lock)
			_output.AppendLine(message);
	}

	public override string ToString() => _output.ToString();
}


public class TestIOSleepAndWake(ITestOutputHelper outPut)
{
	private readonly ITestOutputHelper _outPut = outPut;


	[Theory]
	[InlineData(SizePort.Size16KB, SizePortOnDevice.Size16B)]
	[InlineData(SizePort.Size128KB, SizePortOnDevice.Size16B)]
	[InlineData(SizePort.Size512KB, SizePortOnDevice.Size32B)]
	public void TestCpu(SizePort totalPorts, SizePortOnDevice portsPerDevice)
	{
		const string NameLogger = "default";
		const string Code_First_Device_Test_1 = "NOP\r\nNOP\r\nHALT\r\nEND\r\nEND\r\nEND";
		const string Code_Second_Device_Test_1 =
			"\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP\r\nNOP" +
			"\r\nNOP\r\nNOP\r\nLDI r0, 0\r\nWAKE_INT r0\r\nEND\r\nEND\r\nEND";
		var log = new LoggerOutPut();
		LoggerProvider.RegistryLogger(NameLogger, log);
		LoggerProvider.ChoiceLogger(NameLogger);


		Emulator emu = new(totalPorts, portsPerDevice);

		Device? deviceFirst = null;
		Device? deviceSecond = null;

		try
		{
			int deviceIdFirst = emu.CreateDevice([], RamSize.Size1MB, 0, "Device First");
			Assert.InRange(deviceIdFirst, 0, int.MaxValue);

			int deviceIdSecond = emu.CreateDevice([], RamSize.Size1MB, 1, "Device Second");
			Assert.InRange(deviceIdSecond, 0, int.MaxValue);

			deviceFirst = emu.GetDevice(deviceIdFirst);
			deviceSecond = emu.GetDevice(deviceIdSecond);

			Assert.NotNull(deviceFirst);
			Assert.NotNull(deviceSecond);

			deviceFirst.ActionOnWake = OnWake;
			deviceSecond.ActionOnWake = OnWake;

			byte[]? programFirstDevice = GetByteCode(Code_First_Device_Test_1);
			Assert.NotNull(programFirstDevice);

			deviceFirst.LoadProgram(programFirstDevice, 0);
			deviceFirst.InitHeap((ulong)programFirstDevice.LongLength);

			byte[]? programSecondDevice = GetByteCode(Code_Second_Device_Test_1);
			Assert.NotNull(programSecondDevice);

			deviceSecond.LoadProgram(programSecondDevice, 0);
			deviceSecond.InitHeap((ulong)programSecondDevice.LongLength);

			// После загрузки программ, перед запуском:
			using var doneFirst = new ManualResetEventSlim(false);
			using var doneSecond = new ManualResetEventSlim(false);

			// Запускаем с колбэками, которые сигнализируют о завершении
			deviceFirst.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneFirst.Set(); });
			deviceSecond.LaunchDeviceOnDedicatedThread(0, false, 0, false, OnTitle, null,
				log => { OnEnd(log); doneSecond.Set(); });

			// Ждём завершения обоих устройств (максимум 10 секунд каждое)
			bool firstFinished = doneFirst.Wait(TimeSpan.FromSeconds(10));
			bool secondFinished = doneSecond.Wait(TimeSpan.FromSeconds(10));

			Assert.True(firstFinished, "Первое устройство не завершилось за отведённое время.");
			Assert.True(secondFinished, "Второе устройство не завершилось за отведённое время.");

			Assert.InRange(deviceFirst.CurrentIP, 0UL, (ulong)programFirstDevice.LongLength);
			Assert.InRange(deviceSecond.CurrentIP, 0UL, (ulong)programSecondDevice.LongLength);

			Assert.False(deviceFirst.IsRunning);
			Assert.False(deviceSecond.IsRunning);

			_outPut.WriteLine(log.ToString());
		}
		finally
		{
			deviceFirst?.Dispose();
			deviceSecond?.Dispose();
			emu.Reset(); // или emu.Dispose(), если есть
			_outPut.WriteLine(log.ToString());
			LoggerProvider.DeleteLogger(NameLogger);
		}

	}

	private static void OnEnd(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство выключилось штатно", LogLevel.Log);
	}

	private static void OnTitle(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство запущено", LogLevel.Log);
	}
	private static void OnWake(Action<string, LogLevel> logger)
	{
		logger.Invoke("Устройство проснулось", LogLevel.Log);
	}


	private static byte[]? GetByteCode(string text)
	{
		try
		{
			var code = new AssemblerParser();
			return code.Assemble(text);
		}
		catch (Exception ex)
		{
			LoggerKernel.LogFromSystem("Compiler", ex.Message, LogLevel.Error);
			return null;
		}
	}
}
````

## File: Tests/Tests.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>net10.0</TargetFramework>
	<ImplicitUsings>enable</ImplicitUsings>
	<Nullable>enable</Nullable>
	<IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
	<PackageReference Include="coverlet.collector" Version="6.0.4" />
	<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
	<PackageReference Include="xunit" Version="2.9.3" />
	<PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
  </ItemGroup>

  <ItemGroup>
	<ProjectReference Include="..\Compiller\Compiller.csproj" />
	<ProjectReference Include="..\Controllers\Kernel.csproj" />
	<ProjectReference Include="..\Kernel.Common\Kernel.Common.csproj" />
	<ProjectReference Include="..\ThreadingSystem\ThreadingSystem.csproj" />
	<ProjectReference Include="..\VMApplication\VMApplication.csproj" />
  </ItemGroup>

  <ItemGroup>
	<Using Include="Xunit" />
  </ItemGroup>

</Project>
````

## File: VMApplication/Logger/VMHostLogger.cs
````csharp
using Kernel.Common;

namespace VMApplication.Logger;

public sealed class VMHostLogger
{
	private readonly IOutputView _outputView;
	private readonly string _defaultName;

	public string LoggerName { get; private set; }

	public VMHostLogger(IOutputView outputView, string defaultName)
	{
		_outputView = outputView ?? throw new ArgumentNullException(nameof(outputView));
		_defaultName = string.IsNullOrWhiteSpace(defaultName) ? "default" : defaultName;
		LoggerName = _defaultName;
	}

	public void TestCurrentLoggerSystem()
	{
		_outputView?.Append("Log [Debug log]", LogLevel.Log);
		_outputView?.Append("Warning [Debug log]", LogLevel.Warning);
		_outputView?.Append("Error [Debug log]", LogLevel.Error);
	}

	public void RegistryLogger(string name, IOutputView outPutView)
	{
		LoggerProvider.RegistryLogger(name, new ActionLogger(_outputView.Append, outPutView.Clear));
	}

	public void ChoiceLogger(string? name = null)
	{
		LoggerName = name!;
		LoggerProvider.ChoiceLogger(LoggerName);
	}

	public void DeleteLogger(string name)
	{
		if (name == _defaultName)
			_outputView.Append("Удален первоначальный логгер", LogLevel.Warning);

		LoggerProvider.DeleteLogger(name);
	}

	public void Append(string str, LogLevel log) => _outputView.Append(str, log);
	public void Clear() => _outputView.Clear();

	private class ActionLogger(Action<string, LogLevel> logAction, Action clearAction) : ILogger
	{
		private readonly Action<string, LogLevel> _logAction = logAction;
		private readonly Action _clearAction = clearAction;

		public void Info(string message) => _logAction(message, LogLevel.Log);
		public void Warning(string message) => _logAction(message, LogLevel.Warning);
		public void Error(string message) => _logAction(message, LogLevel.Error);
		public void Clear() => _clearAction();
	}
}
````

## File: VMApplication/Project/ErrorFile.cs
````csharp
namespace VMApplication.Project;

public enum ErrorFile
{
	None = 0,               // Ошибок нет, операция успешна
	UnknownError,           // Непредвиденная или неклассифицированная ошибка

	// --- Ошибки существования и путей ---
	FileNotFound,           // Файл проекта (.vmproj) не найден
	FileNotFoundInProject,  // Файл есть на диске, но не находится в проекте
	DirectoryNotFound,      // Папка с проектами или метаданными удалена или отсутствует
	AlreadyExists,          // Файл или папка с таким именем уже существуют (при создании нового)
	InvalidPathCharacters,  // Путь содержит запрещенные операционной системой символы

	// --- Ошибки доступа и прав ---
	AccessDenied,           // Нет прав администратора на запись/чтение (например, в C:\Program Files)
	FileLocked,             // Файл занят другим процессом (открыт в блокноте, другой IDE или антивирусом)

	// --- Ошибки структуры и парсинга (Специфика IDE) ---
	EmptyFile,              // Файл проекта пустой (нечего читать)
	CorruptedData,          // Нарушена структура метаданных (битый JSON/XML или неверный формат)
	InvalidVersion,         // Версия файла проекта (.vmproj) не поддерживается текущей версией IDE

	// --- Ошибки ограничений ОС ---
	PathTooLong,            // Путь к файлу превышает лимит Windows (обычно 260 символов)
	DiskFull                // На диске закончилось свободное место при попытке сохранения
}
````

## File: VMApplication/Project/FileReadResult.cs
````csharp
namespace VMApplication.Project;

public readonly struct FileReadResult(string fileName, string finalFilePath, ErrorFile errorFile = ErrorFile.None)
{
	public string FileName { get; } = fileName;
	public string FinalFilePath { get; } = finalFilePath;
	public ErrorFile Error { get; } = errorFile;
}
````

## File: VMApplication/Project/IEditorService.cs
````csharp
namespace VMApplication.Project;

public interface IEditorService
{
	void OpenTab(string fileName, string content);
	void CloseTab(string fileName);
	string GetCurrentText();
	string? GetCurrentFileName();
	string? GetText(string fileName);   // возвращает текст из открытой вкладки или null
	void SaveCurrentFile();
	void HighlightErrors(IEnumerable<int> errorLines);   // подсветить строки с ошибками
	void ClearHighlights();

	event EventHandler? TextChanged;

}
````

## File: VMApplication/Project/IFileService.cs
````csharp
namespace VMApplication.Project;

public interface IFileService
{
	string ProjectPath { get; }   // путь к папке проекта
	IEnumerable<string> GetSourceFiles();
	string ReadFile(string fileName);
	void SaveFile(string fileName, string content);
	void SaveProgramFile(byte[] prog);
	bool Exists(string fileName);
}
````

## File: VMApplication/Project/IProjectFilesConfig.cs
````csharp
namespace VMApplication.Project;

public interface IProjectFilesConfig
{
	string ProjectPath { get; }
	string IncludePath { get; }
	string[] ExtensionsAsm { get; }
	string[] ExtensionsMiniC { get; }
}
````

## File: VMApplication/Project/ResultDeCompilation.cs
````csharp
namespace VMApplication.Project;

public readonly record struct ResultDeCompilation(string TextAsm, int Lenght, int Size);
````

## File: VMApplication/Project/SourceFile.cs
````csharp
namespace VMApplication.Project;

public readonly record struct SourceFile(string Name, string Content, SourceLanguage Language);
````

## File: VMApplication/Project/SourceLanguage.cs
````csharp
namespace VMApplication.Project;

public enum SourceLanguage
{
	Asm,
	C,
	None,
}
````

## File: VMApplication/Project/TabDataEditor.cs
````csharp
namespace VMApplication.Project;

public class TabDataEditor(string path)
{
	public string Path { get; } = path;
	public bool IsOpened { get; set; } = false;
	public string Content { get; set; } = string.Empty;
	public SourceLanguage Language { get; init; } = !path.EndsWith(".asm", StringComparison.OrdinalIgnoreCase) ? SourceLanguage.C : SourceLanguage.Asm;
}
````

## File: VMApplication/Project/VMHostProjectBuilder.cs
````csharp
using VMApplication.Logger;

namespace VMApplication.Project;

public class VMHostProjectBuilder
{
	private IProjectFilesConfig? _projectPaths;
	private IProjectService? _projectService;
	private VMHostLogger? _logger;

	public VMHostProjectBuilder WithLogger(VMHostLogger logger)
	{
		_logger = logger;
		return this;
	}
	public VMHostProjectBuilder WithPaths(IProjectFilesConfig paths)
	{
		_projectPaths = paths;
		return this;
	}
	public VMHostProjectBuilder WithProjectSevice(IProjectService proj)
	{
		_projectService = proj;
		return this;
	}

	public VMHostProject Build()
	{
		return _projectPaths == null || _projectService == null || _logger == null
			? throw new InvalidOperationException("ProjectPaths, Projectservice, Logger обязательны")
			: new VMHostProject(_projectPaths, _projectService, _logger);
	}
}
````

## File: ASM gen/Analizator/ErrorLineColorizer.cs
````csharp
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace ASM_gen.Analizator;

public class ErrorLineColorizer : DocumentColorizingTransformer
{
	// Храним номера строк с ошибками (1-based индексация, как в AvalonEdit)
	public HashSet<int> ErrorLines = [];

	protected override void ColorizeLine(DocumentLine line)
	{
		// Проверяем, есть ли текущая строка в списке ошибок
		if (ErrorLines.Contains(line.LineNumber))
		{
			// Изменяем свойства отображения для всей строки целиком
			ChangeLinePart(
				line.Offset,
				line.EndOffset,
				visualLineElement =>
				{
					// Устанавливаем светло-красный фон для строки
					visualLineElement.TextRunProperties.SetBackgroundBrush(
						new SolidColorBrush(Color.FromArgb(50, 255, 0, 0))
					);

					// Опционально: можно изменить цвет самого текста на темно-красный
					// visualLineElement.TextRunProperties.SetForegroundBrush(Brushes.DarkRed);
				});
		}
	}
}
````

## File: ASM gen/Information Window/MemoryRow.cs
````csharp
namespace ASM_gen.Information_Window;

public readonly ref struct MemoryRow
{
	private readonly ulong _address;
	private readonly ReadOnlySpan<byte> _memory;

	public MemoryRow(ulong address, ReadOnlyMemory<byte> memory)
	{
		_address = address;

		int length = Math.Min(16, memory.Length - (int)address);
		_memory = memory.Span.Slice((int)address, length);
	}

	public string AddressHex => _address.ToString("X8");

	public readonly string GetBytesHex()
	{
		if (_memory.IsEmpty) return string.Empty;

		return string.Create(_memory.Length * 3 - 1, _memory, (dest, src) =>
		{
			for (int i = 0; i < src.Length; i++)
			{
				src[i].TryFormat(dest.Slice(i * 3, 2), out _, "X2");

				if (i < src.Length - 1)
				{
					dest[i * 3 + 2] = ' ';
				}
			}
		});
	}

	public readonly string GetAsciiText()
	{
		if (_memory.IsEmpty) return string.Empty;

		return string.Create(_memory.Length, _memory, (dest, src) =>
		{
			for (int i = 0; i < src.Length; i++)
			{
				byte b = src[i];
				dest[i] = (b is >= 32 and <= 126) ? (char)b : '.';
			}
		});
	}
}
````

## File: ASM gen/NewProjectManage/NewProjectDialog.xaml.cs
````csharp
using ASM_gen.ProjectManage.Managers.Static;
using System.Windows;
using System.Windows.Controls;

namespace ASM_gen.NewProjectManage
{
	/// <summary>
	/// Логика взаимодействия для NewProjectDialog.xaml
	/// </summary>
	public partial class NewProjectDialog : Window
	{
		public string? CreatedProjectPath { get; private set; }

		public NewProjectDialog()
		{
			InitializeComponent();
		}

		private void Create_Click(object sender, RoutedEventArgs e)
		{
			string name = ProjectNameBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				MessageBox.Show("Введите имя проекта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			bool isAsm = ((ComboBoxItem)LanguageBox.SelectedItem).Content.ToString() == "Ассемблер";
			try
			{
				CreatedProjectPath = DirManager.CreateNewProject(name, isAsm);
				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

	}
}
````

## File: ASM gen/Services/ProjectService.cs
````csharp
using VMApplication.Project;

namespace ASM_gen.Services;

public class ProjectService(IFileService fileService, IEditorService editorService, Action<string>? logCallback = null) : IProjectService
{
	private readonly IFileService _fileService = fileService;
	private readonly IEditorService _editorService = editorService;
	private readonly Action<string>? _logCallback = logCallback; // для логирования

	public IFileService FileService => _fileService;
	public IEditorService EditorService => _editorService;
	public string ProjectPath => _fileService.ProjectPath;

	public void OpenProject()
	{
		_logCallback?.Invoke($"Открытие проекта: {_fileService.ProjectPath}");
		// Проверяем наличие файлов .c и .asm, загружаем их в редактор
		var sourceFiles = _fileService.GetSourceFiles();
		foreach (var file in sourceFiles)
		{
			if (_fileService.Exists(file))
			{
				string content = _fileService.ReadFile(file);
				_editorService.OpenTab(file, content);
			}
		}
	}

	public void SaveAllFiles()
	{
		// Сохраняем текущий открытый файл, а также пробегаем по всем открытым вкладкам
		_editorService.SaveCurrentFile();
		// Дополнительно можно сохранить все изменённые файлы, зная список через IFileService
		foreach (var file in _fileService.GetSourceFiles())
		{
			string? content = _editorService.GetText(file);
			if (content != null)
				_fileService.SaveFile(file, content);
		}
		_logCallback?.Invoke("Все файлы сохранены.");
	}

	public IEnumerable<SourceFile> GetSourceFiles()
	{
		var files = new List<SourceFile>();
		foreach (var fileName in _fileService.GetSourceFiles())
		{
			string source = _editorService.GetText(fileName) ?? _fileService.ReadFile(fileName);
			SourceLanguage lang = fileName.EndsWith(".asm", StringComparison.OrdinalIgnoreCase)
				? SourceLanguage.Asm : SourceLanguage.C;
			files.Add(new SourceFile(fileName, source, lang));
		}
		return files;
	}
}
````

## File: ASM gen/Utils/EnumExtensions.cs
````csharp
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace ASM_gen.Utils;

public static class EnumExtensions
{
	extension(Enum value)
	{
		public string GetDescription()
		{
			FieldInfo? field = value.GetType().GetField(value.ToString());
			if (field == null) return value.ToString();

			DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
			return attribute?.Description ?? value.ToString();
		}
	}

	public static IEnumerable<EnumItem> GetEnumItems<T>() where T : Enum
	{
		return Enum.GetValues(typeof(T))
			.Cast<T>()
			.Select(e => new EnumItem(e, e.GetDescription()));
	}

	// Необобщённый метод для использования в конвертере
	public static IEnumerable<EnumItem> GetEnumItems(Type enumType)
	{
		if (!enumType.IsEnum) throw new ArgumentException("Type must be an enum");
		return Enum.GetValues(enumType)
			.Cast<Enum>()
			.Select(e => new EnumItem(e, e.GetDescription()));
	}
}

public class EnumItem(Enum value, string displayName)
{
	public Enum Value { get; } = value;
	public string DisplayName { get; } = displayName;
	public override string ToString() => DisplayName;
}

// Конвертер для ComboBox, который показывает список элементов перечисления
public class EnumToItemsSourceConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		if (value == null) return Array.Empty<EnumItem>();
		// Получаем тип перечисления из значения (значение — это выбранный элемент)
		Type enumType = value.GetType();
		return EnumExtensions.GetEnumItems(enumType);
	}

	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		if (value is EnumItem item) return item.Value;
		return Binding.DoNothing;
	}
}
````

## File: Compiller/Compiller.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>net10.0</TargetFramework>
	<ImplicitUsings>enable</ImplicitUsings>
	<Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
	<ProjectReference Include="..\Controllers\Kernel.csproj" />
	<ProjectReference Include="..\Kernel.Common\Kernel.Common.csproj" />
  </ItemGroup>

</Project>
````

## File: Controllers/ProcessorSystem/ProcessorPoolEmulator.cs
````csharp
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Collections.Concurrent;

namespace Kernel.ProcessorSystem;

public static class ProcessorPoolEmulator
{
	private const int MaxPoolSize = 32;
	private static readonly ConcurrentQueue<Processor> _poolProcessors = [];

	public static Processor Rent(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
	{
		if (_poolProcessors.TryDequeue(out Processor? processor))
		{
			if (processor != null && processor.TryInitInPool(ram, nameDeviceToken, portBus, regLock, name))
				return processor;
		}

		return new Processor(ram, nameDeviceToken, portBus, regLock, name);
	}

	public static void Return(Processor proc)
	{
		if (proc == null) return;

		proc.Reset();
		if (_poolProcessors.Count < MaxPoolSize)
			_poolProcessors.Enqueue(proc);
	}
}
````

## File: Controllers/RamSystem/NativeMemoryBuffer.cs
````csharp
using Kernel.Common;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Kernel.RamSystem;

public unsafe sealed class NativeMemoryBuffer : IDisposable
{
	private readonly NativeMemoryManager _manager;
	private byte* _ptr;
	private readonly nuint _length;
	private int _isDisposed;

	public NativeMemoryBuffer(RamSize size)
	{
		_length = (nuint)size;
		_ptr = (byte*)NativeMemory.Alloc(_length);
		NativeMemory.Clear(_ptr, _length);
		_manager = new(this);
	}

	public int Length => (int)_length; // для совместимости, но лучше использовать nuint
	public nuint LengthU => _length;
	public byte* Pointer => _ptr;

	public Span<byte> AsSpan() => new(_ptr, (int)_length);
	public ReadOnlySpan<byte> AsReadOnlySpan() => new(_ptr, (int)_length);

	public Span<byte> AsSpan(int start, int length) => new(_ptr + start, length);
	public ReadOnlySpan<byte> AsReadOnlySpan(int start, int length) => new(_ptr + start, length);

	public byte this[ulong index]
	{
		get => _ptr[index];
		set => _ptr[index] = value;
	}

	public void Clear() => AsSpan().Clear();

	public void Dispose()
	{
		if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
		{
			NativeMemory.Free(_ptr);
			_ptr = null;
		}
	}

	public Memory<byte> AsMemory()
	{
		return _manager.Memory;
	}

	public Memory<byte> AsMemory(int start, int length)
	{
		return _manager.Memory.Slice(start, length);
	}

	public ReadOnlyMemory<byte> AsReadOnlyMemory()
	{
		return _manager.Memory;
	}
}

public sealed unsafe class NativeMemoryManager(NativeMemoryBuffer buffer) : MemoryManager<byte>
{
	private readonly int _length = buffer.Length;
	private byte* _ptr = buffer.Pointer; // копия указателя для быстрого доступа

	protected override void Dispose(bool disposing)
	{
		_ptr = null;
	}

	public override Span<byte> GetSpan()
	{
		return _ptr == null ? throw new ObjectDisposedException(nameof(NativeMemoryManager)) : new Span<byte>(_ptr, _length);
	}

	public override MemoryHandle Pin(int elementIndex = 0)
	{
		if ((uint)elementIndex >= (uint)_length)
			throw new ArgumentOutOfRangeException(nameof(elementIndex));
		return new MemoryHandle(_ptr + elementIndex);
	}

	public override void Unpin() { } // нативная память не перемещается
}
````

## File: Controllers/Utilites/DiskManager.cs
````csharp
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.LocalMemorySystem;

namespace Kernel.Utilites;

public class DiskManager(PortBus portBus)
{
	private readonly PortBus _portBus = portBus;
	private readonly Dictionary<uint, DiskDevice> _disks = []; // ключ – номер сектора

	/// <summary>
	/// Создаёт диск и регистрирует его. Возвращает номер сектора или -1.
	/// </summary>
	public int CreateDisk(string imagePath)
	{
		int sector = _portBus.AllocateFreeSector();
		if (sector < 0) return sector;
		if (!_portBus.IsFreeSector((uint)sector)) return -5;

		var disk = new DiskDevice(imagePath);
		if (!_portBus.RegisterDevice(disk, (uint)sector))
		{
			disk.Dispose();
			return -1;
		}

		_disks[(uint)sector] = disk;
		return sector;
	}

	public int CreateDisk(string imagePath, uint sector)
	{
		if (!_portBus.IsFreeSector(sector)) return -1;

		var disk = new DiskDevice(imagePath);
		if (!_portBus.RegisterDevice(disk, sector))
		{
			disk.Dispose();
			return -2;
		}

		_disks[sector] = disk;
		return 0;
	}

	public bool RemoveDisk(uint sector)
	{
		if (!_disks.TryGetValue(sector, out var disk))
			return false;
		_portBus.UnregisterDevice(sector);
		disk.Dispose();
		_disks.Remove(sector);
		return true;
	}

	public IEnumerable<int> GetAllDisks() => from KeyValuePair<uint, DiskDevice> disk in _disks
											 select (int)disk.Key;

	public IEnumerable<DiskInfo> GetAllDiskInfo()
	{
		foreach (KeyValuePair<uint, DiskDevice> item in _disks)
		{
			yield return new DiskInfo(item.Value.SizeDisk,
									  item.Value.CountSectors,
									  item.Key,
									  item.Value.CreatedAt,
									  item.Value.ImagePath);
		}
	}

	public DiskDevice? GetDisk(uint sector) => _disks.GetValueOrDefault(sector);

	public void Clear()
	{
		foreach (var sector in _disks.Keys.ToArray())
			RemoveDisk(sector);
	}
}
````

## File: include/std.vma
````
// std.asm – Standard library for Mini-C VM
// Requires sys.vma for get_program_end and set_int_table_base

// -------------------------------------------------------------------
// Инициализация таблицы векторов прерываний
// Размещает таблицу сразу за кодом программы и сдвигает кучу
// Должна быть вызвана в main до использования импортированных прерываний
// -------------------------------------------------------------------

func_init_vectors:
	CALL get_program_end       // r0 = текущий rHP (конец программы)
	// Выровнять адрес вверх до 8 байт
	LDI r1, 7
	ADD r0, r1
	LDI r1, 0xFFFFFFFFFFFFFFF8
	AND r0, r1                      // r0 = (rHP + 7) & ~7
	MOV r2, r0                      // r2 = база таблицы (выровненный адрес)

	// Заполняем векторы 0..2
	LDI r0, 0
	LDI r1, int0_handler
	CALL func_set_int_vector_dyn
	LDI r0, 1
	LDI r1, int1_handler
	CALL func_set_int_vector_dyn
	LDI r0, 2
	LDI r1, int2_handler
	CALL func_set_int_vector_dyn
	LDI r0, 3
	LDI r1, int3_handler
	CALL func_set_int_vector_dyn

	// Сдвинуть кучу на 256 байт после таблицы
	MOV r0, r2
	LDI r1, 256
	ADD r0, r1
	MOV rHP, r0                     // новый rHP = база + 256

	// Установить rTB = адрес таблицы (r2)
	MOV r0, r2
	CALL set_int_table_base
	RET


// Локальный доступ к адресу кучи
get_program_end:
	MOV r0, rHP
	RET

set_int_table_base:
	MOV rTB, r0
	RET

// Установка одного вектора (для внутреннего использования)
// r0 = номер вектора, r1 = адрес обработчика, r2 = базовый адрес таблицы
func_set_int_vector_dyn:
	PUSH r3
	MOV r3, r0
	ADD r3, r3      // *2
	ADD r3, r3      // *4
	ADD r3, r3      // *8
	ADD r3, r2      // r3 = base + offset
	STORE_IND.S64 r1, r3
	POP r3
	RET

// -------------------------------------------------------------------
// Обработчики прерываний
// -------------------------------------------------------------------
int0_handler:
	PUSH r0
	PRINT_INT r0
	POP r0
	IRET

int1_handler:
	PUSH r0
	PUSH r1
	OUT r1, r0
	POP r1
	POP r0
	IRET

int2_handler:
	IN r0, r0
	IRET

int3_handler:
	IN r0, r0
	IRET
// -------------------------------------------------------------------
// Публичные обёртки для системных вызовов
// -------------------------------------------------------------------
func_print_int:
	LDI r2, 0
	INT r2
	RET

func_out_port:
	LDI r2, 1
	INT r2
	RET

func_in_port:
	LDI r2, 2
	INT r2
	RET

func_wake_processor:
	LDI r2, 3
	INT r2
	RET

func_exit:
	HALT
	RET
````

## File: Kernel.Common/LoggerKernel.cs
````csharp
using System.Runtime.CompilerServices;

namespace Kernel.Common;

public static class LoggerKernel
{

	private static string FormatLogString(ReadOnlySpan<char> prefix, string text)
	{
		int exactLength = 1 + prefix.Length + 2 + text.Length;

		return string.Create(exactLength, (prefix: prefix.ToString(), text), (span, state) =>
		{
			span[0] = '[';
			ReadOnlySpan<char> pSpan = state.prefix.AsSpan();
			pSpan.CopyTo(span[1..]);

			int index = 1 + pSpan.Length;
			span[index] = ']';
			span[index + 1] = ' ';

			state.text.AsSpan().CopyTo(span[(index + 2)..]);
		});
	}

	public static void LogFromDevice(in NameDeviceToken nameDevice, string text, LogLevel level = LogLevel.Log)
	{
		ReadOnlySpan<char> nameSpan = nameDevice.Name.AsSpan();
		ExecuteLog(nameSpan, text, level);
	}

	public static void LogFromSystem(string nameSystem, string text, LogLevel level = LogLevel.Log)
	{
		ReadOnlySpan<char> nameSpan = nameSystem.AsSpan();
		ExecuteLog(nameSpan, text, level);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ExecuteLog(ReadOnlySpan<char> prefix, string text, LogLevel level)
	{
		string fullMessage = FormatLogString(prefix, text);

		switch (level)
		{
			case LogLevel.Warning: LoggerProvider.Warning(fullMessage); break;
			case LogLevel.Error: LoggerProvider.Error(fullMessage); break;
			default: LoggerProvider.Info(fullMessage); break;
		}
	}

	public static void ClearLog() => LoggerProvider.Clear();
}
````

## File: Kernel.Common/NameDeviceToken.cs
````csharp
namespace Kernel.Common;

public readonly struct NameDeviceToken
{
	public const string UnknownName = "Unknown Name Device";

	private readonly string _cachedName;

	public NameDeviceToken(ReadOnlySpan<char> name)
	{
		// Если имя пустое — записываем ссылку на константу, иначе — очищенную строку
		_cachedName = name.IsEmpty ? UnknownName : name.ToString();
	}

	public NameDeviceToken()
	{
		_cachedName = UnknownName;
	}

	// Свойство вычисляется на лету, не занимая места в памяти структуры!
	// Благодаря интернированию, проверка (ReferenceEquals) работает мгновенно.
	public bool IsUnkown => ReferenceEquals(_cachedName, UnknownName) || _cachedName == null;

	// Если объект создали через default(NameDeviceToken), _name будет null. 
	// Защитим свойство Name от возврата null:
	public string Name => _cachedName ?? UnknownName;

	public override string ToString()
	{
		if (!IsUnkown) return Name;
		return "[Warning] " + Name;
	}

	public NameDeviceToken CreateChild(ReadOnlySpan<char> childName)
	{
		return new NameDeviceToken($"{Name}/{childName}");
	}
}
````

## File: Kernel.Common/OpCode.cs
````csharp
namespace Kernel.Common;

public enum OpCode : byte
{
	// === 1. Системные команды ===
	NOP, // Нет операции (пропуск такта)
	END, // Остановка процессора / завершение программы
	PRINT,

	// === 2. Работа с памятью (Указатели и регистры) ===
	MOV, // Копировать значение из регистра в регистр (MOV R1, R2)
	LOAD, // Загрузить в регистр число из памяти по адресу (LOAD R1, [R2])
	STORE, // Записать число из регистра в память по адресу (STORE [R1], R2)
	LDI, // Загрузить константу (Immediate) прямо в регистр (LDI R1, 42)
	LOAD_IND,
	STORE_IND,

	// === 3. Арифметика и Логика (Тьюринг-базис) ===
	ADD, // Сложение (ADD R1, R2 -> R1 = R1 + R2)
	SUB, // Вычитание (SUB R1, R2 -> R1 = R1 - R2)
	MULT_INT, //Умножение (MULT_INT R1 R2 -> R1 = R1 * R2)
	SHR, //Деление SHR r1, r2 (r1 = r1 >> r2)
	INC, // Инкремент значения в регистре (INC R1)
	DEC, // Декремент значения в регистре (DEC R1)
	DIV,

	// === 4. Логика (нужна для битовых масок и флагов) ===
	AND, // Побитовое И
	OR, // Побитовое ИЛИ
	XOR, // Побитовое исключающее ИЛИ (часто используется для обнуления: XOR R1, R1)
	NOT, // Побитовое НЕ

	// === 5. Управление потоком (Ветвление и Указатели команд) ===
	JMP, // Безусловный переход по адресу (JMP 0x05)
	JZ, // Переход, если результат последней операции равен нулю (Jump if Zero)
	JNZ, // Переход, если результат НЕ равен нулю (Jump if Not Zero)
	JG, // Переход, если первое число больше второго (Jump if Greater)
	JL, // Переход, если первое число меньше второго (Jump if Less)

	// === 6. Работа со Стеком (необходима для вызова функций) ===
	PUSH, // Положить значение регистра в стек
	POP, // Забрать значение из стека в регистр
	CALL, // Вызов подпрограммы (сохраняет адрес возврата в стек и делает JMP)
	RET,  // Возврат из подпрограммы (делает POP адреса возврата в Instruction Pointer)

	// === 7. Ввод-вывод ===
	IN,  // Чтение из порта: IN Rdest, Rport  (Rdest ← порт[Rport])
	OUT,  // Запись в порт:  OUT Rsrc, Rport  (порт[Rport] ← Rsrc)
	PRINT_INT,
	INT,   // программное прерывание
	IRET,   // возврат из прерывания

	ALLOC,
	HALT,
	WAKE,

	WAKE_INT
}
````

## File: Kernel.Common/RegType.cs
````csharp
namespace Kernel.Common;

public enum RegType : byte
{
	rZ, r0, r1, 
	r2, r3, r4, r5, 
	r6, r7, r8, r9,
	r10, r11, r12, 
	r13, r14, r15, 
	r16, r17, r18, 
	r19, r20, r21,
	rTB, rCD, rFL,
	rLP, rCL, rRT,
	rSP, rHP, rIP,

}
````

## File: Kernel.Common/SizePort.cs
````csharp
using System.ComponentModel;

namespace Kernel.Common;

public enum SizePort : ulong
{
	[Description("64 байта")] Size64B = 6,
	[Description("128 байт")] Size128B = 7,
	[Description("256 байт")] Size256B = 8,
	[Description("512 байт")] Size512B = 9,
	[Description("1 КБ")] Size1KB = 10,
	[Description("4 КБ")] Size4KB = 12,
	[Description("8 КБ")] Size8KB = 13,
	[Description("16 КБ")] Size16KB = 14,
	[Description("64 КБ")] Size64KB = 16,
	[Description("128 КБ")] Size128KB = 17,
	[Description("256 КБ")] Size256KB = 18,
	[Description("512 КБ")] Size512KB = 19,
}
````

## File: Kernel.Common/SizePortOnDevice.cs
````csharp
using System.ComponentModel;

namespace Kernel.Common;

public enum SizePortOnDevice : uint
{
	[Description("4 байта")] Size4B = 2,
	[Description("8 байт")] Size8B = 3,
	[Description("16 байт")] Size16B = 4,
	[Description("32 байта")] Size32B = 5,
}

public readonly struct DiskInfo(long sizeDisk, int countSectors, uint port, DateTime createdAt, string pathToFile)
{
	public readonly long SizeDisk = sizeDisk;
	public readonly int CountSectors = countSectors;
	public readonly uint Port = port;
	public readonly DateTime CreatedAt = createdAt;
	public readonly string PathToFile = pathToFile;
}
````

## File: VMApplication/Emulator/DeviceStepMode.cs
````csharp
using Kernel.BiosSystem;

namespace VMApplication.Emulator;

public class DeviceStepMode
{
	private readonly Device _device;

	internal DeviceStepMode(Device device) => _device = device;


	public void Step(bool debug = false) => _device.NextStepProcessor(debug);
	public void MultyStep(bool debug = false, int count = 5) => _device.NextStepProcessorCount(count, debug);

}
````

## File: VMApplication/Emulator/DeviceView.cs
````csharp
namespace VMApplication.Emulator;

public readonly struct DeviceView(int id,
								  uint ramSize,
								  uint portSize,
								  uint sector, ReadOnlyMemory<byte> ram,
								  DateTime createdAt,
								  string? name = null)
{
	public int Id { get; init; } = id;
	public uint RamSize { get; init; } = ramSize;
	public uint PortSize { get; init; } = portSize;
	public uint Sector { get; init; } = sector;
	public ReadOnlyMemory<byte> Ram { get; init; } = ram;
	public string? Name { get; init; } = name;
	public DateTime CreatedAt { get; init; } = createdAt;
	public string PortRange { get; init; } = CreatePortRange(sector, portSize);

	public static string CreatePortRange(uint sector, uint portSize)
	{
		uint start = sector * portSize;
		uint end = start + portSize - 1;
		return $"{start}–{end}";
	}
}
````

## File: VMApplication/Emulator/Disk.cs
````csharp
using Kernel.LocalMemorySystem;

namespace VMApplication.Emulator;

public class DiskData : IDisposable
{
	private readonly DiskDevice _disk;
	private readonly VMEmulator _emulator; // для удаления
	private readonly long _totalSectors;
	public uint Sector { get; }
	public string ImagePath { get; }
	public bool IsMounted { get; private set; } = true;

	internal DiskData(DiskDevice disk, uint sector, string imagePath, VMEmulator emulator)
	{
		_disk = disk;
		Sector = sector;
		ImagePath = imagePath;
		_emulator = emulator;
		_totalSectors = (new FileInfo(imagePath).Length / DiskDevice.SectorSize);
	}

	public long TotalSectors => _totalSectors;
	public static int SectorSize => DiskDevice.SectorSize;
	public uint BasePortAddress => Sector * _emulator.PortsPerDevice;
	public ReadOnlyMemory<byte> ReadSectorBuffer => new(_disk.SectorBuffer);

	public void LoadDataOnDisk(uint adress, ReadOnlySpan<byte> data) => _disk.WriteSectorDirect(adress, data);
	public void LoadDataOnDisk(uint adress, byte[] data) => _disk.WriteSectorDirect(adress, data);
	
	public byte[]? ReadSectorDirect(uint lba) => _disk.ReadSectorDirect(lba);

	/// <summary>
	/// Отмонтировать диск и удалить его из системы.
	/// </summary>
	public bool Remove()
	{
		if (!IsMounted) return false;
		bool success = _emulator.RemoveDisk((uint)Sector);
		if (success)
			IsMounted = false;
		return success;
	}

	public void Dispose()
	{
		if (IsMounted)
			Remove();
		GC.SuppressFinalize(this);
	}
}

public static class DiskImageHelper
{
	public static byte[]? ReadSector(string imagePath, uint lba)
	{
		if (!File.Exists(imagePath))
			return null;

		using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
		long offset = lba * DiskDevice.SectorSize;
		if (offset + DiskDevice.SectorSize > fs.Length)
			return null;

		var buffer = new byte[DiskDevice.SectorSize];
		fs.Seek(offset, SeekOrigin.Begin);
		fs.ReadExactly(buffer, 0, DiskDevice.SectorSize);
		return buffer;
	}

	public static void WriteSector(string imagePath, uint lba, ReadOnlySpan<byte> data)
	{
		if (data.Length > DiskDevice.SectorSize)
			throw new ArgumentException("Данные превышают размер сектора");

		using var fs = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.Write);
		long offset = lba * DiskDevice.SectorSize;
		if (offset + DiskDevice.SectorSize > fs.Length)
			fs.SetLength(offset + DiskDevice.SectorSize);

		fs.Seek(offset, SeekOrigin.Begin);
		fs.Write(data);
		// Заполнить остаток нулями при необходимости
	}
}
````

## File: VMApplication/Emulator/VMEmulatorBuilder.cs
````csharp
using Kernel.Common;
using VMApplication.Logger;

namespace VMApplication.Emulator;

public class VMEmulatorBuilder
{
	private SizePort _portBusSize = SizePort.Size16KB;
	private SizePortOnDevice _portsPerDevice = SizePortOnDevice.Size16B;

	private VMHostLogger? _logger;

	public VMEmulatorBuilder WithLogger(VMHostLogger logger)
	{
		_logger = logger;
		return this;
	}
	public VMEmulatorBuilder WithPortBusSize(SizePort size)
	{
		_portBusSize = size;
		return this;
	}

	public VMEmulatorBuilder WithPortsPerDevice(SizePortOnDevice ports)
	{
		_portsPerDevice = ports;
		return this;
	}

	public VMEmulator Build()
	{
		return _logger == null
			? throw new InvalidOperationException("Logger обязателен")
			: new VMEmulator(new(_portBusSize, _portsPerDevice), _logger, _portsPerDevice);
	}
}
````

## File: VMApplication/Logger/IOutputView.cs
````csharp
using Kernel.Common;

namespace VMApplication.Logger;

public interface IOutputView
{
	void Append(string message, LogLevel level = LogLevel.Log);    // LogLevel = Info, Warning, Error
	void Clear();
}
````

## File: VMApplication/Logger/LoggerBuilder.cs
````csharp
namespace VMApplication.Logger;

public class LoggerBuilder
{
	private IOutputView? _outputView;
	private string _nameLogger = "default";
	public LoggerBuilder WithOutPut(IOutputView outputView)
	{
		_outputView = outputView;
		return this;
	}

	public LoggerBuilder WithNameLogger(string name)
	{
		if (!string.IsNullOrWhiteSpace(name)) _nameLogger = name;
		return this;
	}

	public VMHostLogger Build()
	{
		if (_outputView == null)
		{
			throw new InvalidOperationException("OutputView обязателен");
		}

		var logger = new VMHostLogger(_outputView, _nameLogger!);
		logger.RegistryLogger(_nameLogger, _outputView);
		logger.ChoiceLogger(_nameLogger);
		return logger;
	}
}
````

## File: VMApplication/Project/IProjectService.cs
````csharp
namespace VMApplication.Project;

public interface IProjectService
{
	void OpenProject();
	void SaveAllFiles();
	IEnumerable<SourceFile> GetSourceFiles();
	string ProjectPath { get; }
	IFileService FileService { get; }
	IEditorService EditorService { get; }
}
````

## File: VMApplication/VMApplication.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>net10.0</TargetFramework>
	<ImplicitUsings>enable</ImplicitUsings>
	<Nullable>enable</Nullable>
	<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup>
	<ProjectReference Include="..\Compiller\Compiller.csproj" />
	<ProjectReference Include="..\Controllers\Kernel.csproj" />
	<ProjectReference Include="..\Kernel.Common\Kernel.Common.csproj" />
  </ItemGroup>

  <ItemGroup>
	<Service Include="{508349b6-6b84-4df5-91f0-309beebad82d}" />
  </ItemGroup>

</Project>
````

## File: VMApplication/VMHostHelper.cs
````csharp
using Compiller.ASM;
using Compiller.C;
using VMApplication.Emulator;
using VMApplication.Project;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication;

public static class VMHostHelper
{
	/// <summary>
	/// RU: Запускает парсер и лексер кода, вызов его не в блоке try - catch приведет к постоянным выбросам исключений
	/// ENG: Executes the code parser and lexer. This method throws exceptions on parsing failures and must be wrapped in a try-catch block.
	/// </summary>
	/// <param name="text"> исходный текст </param>
	public static void LaunchUnsafeParse(string text)
	{
		var lexer = new Lexer(text);
		var tokens = lexer.Tokenize();
		var parser = new Parser(tokens);
		parser.Parse();
	}

	extension(DeviceInfo d)
	{
		public DeviceView ConvertDeviceInfo() => new(d.Id,
													 (uint)d.RamSize,
													 (uint)d.PortSize,
													 d.Sector,
													 d.Device.RamArray,
													 d.CreatedAt,
													 d.Name);
	}

	public static ResultDeCompilation DisassemblCode(ReadOnlyMemory<byte> prog, ulong baseAddress = 0UL)
	{
		var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
		return new(text, lenght, size);
	}

	public static ResultDeCompilation DisassemblCode(ReadOnlySpan<byte> prog, ulong baseAddress = 0UL)
	{
		var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
		return new(text, lenght, size);
	}

	public static ResultDeCompilation DisassemblCode(byte[] prog, ulong baseAddress = 0UL)
	{
		var text = Disassembler.Disassemble(prog, out int lenght, out int size, baseAddress);
		return new(text, lenght, size);
	}


}
````

## File: ASM gen/Highlight/CastomHighlightingManager.cs
````csharp
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows;
using System.Windows.Media;

namespace ASM_gen.Highlight;

public enum LanguageType
{
	ASM,
	C,
}
public static partial class CastomHighlightingManager
{
	private class CustomHighlightingDefinition(HighlightingRuleSet mainRuleSet) : IHighlightingDefinition
	{
		private readonly HighlightingRuleSet _mainRuleSet = mainRuleSet;

		public string Name => "MyAsm";
		public HighlightingRuleSet MainRuleSet => _mainRuleSet;
		public HighlightingRuleSet GetNamedRuleSet(string name) => null!;
		public HighlightingColor GetNamedColor(string name) => null!;
		public static IEnumerable<HighlightingColor> NamedColors => null!;
		public IDictionary<string, string> Properties => null!;

		public IEnumerable<HighlightingColor> NamedHighlightingColors => null!;
	}

	extension(TextEditor textEditor)
	{
		public void ChoseLang(LanguageType lang)
		{
			if (textEditor == null) return;
			textEditor.SyntaxHighlighting = lang switch
			{
				LanguageType.ASM => ApplyASMHighlighting(),
				LanguageType.C => ApplyCHighlighting(),
				_ => ApplyCHighlighting()
			};
		}
	}

	private static CustomHighlightingDefinition ApplyASMHighlighting()
	{
		// 1. Создаем пустую разметку правил синтаксиса
		HighlightingRuleSet ruleSet = new();

		// 2. Правило для комментариев (; комментарий или // комментарий)
		HighlightingColor commentColor = new()
		{
			Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#064a1b"))
		};
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.RegexASMCommentColor(),
			Color = commentColor
		});

		// 3. Правило для команд (MOV, CLR, ADD, SUB, LW, SW, LOAD, STR, RET)
		HighlightingColor keywordColor = new()
		{
			Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#a5e6e3")),
		};
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.RegexASMKeywordColor(),
			Color = keywordColor
		});

		// 4. Правило для ВАШИХ регистров (r0..r22, sp, ra, zero)
		HighlightingColor registerColor = new()
		{
			Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#729687")),
		};
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.RegexASMRegisterColor(),
			Color = registerColor
		});

		// 5. Правило для чисел и смещений (10, 0x1A)
		HighlightingColor numberColor = new()
		{
			Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#7fe069")),
			FontWeight = FontWeights.Bold
		};
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.RegexASMNumberColor(),
			Color = numberColor,
		});

		// 6. Упаковываем это в определение синтаксиса
		CustomHighlightingDefinition asmDefinition = new(ruleSet);

		// 7. Применяем к редактору
		return asmDefinition;
	}

	private static CustomHighlightingDefinition ApplyCHighlighting()
	{
		var ruleSet = new HighlightingRuleSet();

		// 1. Многострочные комментарии /* ... */ (должны быть обработаны первыми,
		//    чтобы не перекрываться однострочными или ключевыми словами)
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.CommentMultiLine(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#6A9955"))
			}
		});

		// 2. Однострочные комментарии //
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.CommentSingleLine(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#6A9955"))
			}
		});

		// 3. Строковые литералы "..."
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.StringLiteral(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#CE9178"))
			}
		});

		// 4. Символьные литералы 'x'
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.CharLiteral(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#CE9178"))
			}
		});

		// 5. Ключевые слова (int, char, void, if, while, for, return и т.д.)
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.Keyword(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#569CD6"))
			}
		});

		// 6. Числа (десятичные и 0x...)
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.Number(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#B5CEA8")),
				FontWeight = FontWeights.Bold
			}
		});

		// 7. Операторы (+, -, *, /, ==, !=, <, >, &&, || и т.д.)
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.Operator(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"))
			}
		});

		// 8. Пунктуация (скобки, запятые, точка с запятой)
		//    (можно закомментировать, если не нужна отдельная подсветка)
		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.Punctuation(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"))
			}
		});

		ruleSet.Rules.Add(new HighlightingRule
		{
			Regex = SyntaxHighlighter.IncludeDirective(),
			Color = new HighlightingColor
			{
				Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#C586C0"))
			}
		});
		return new CustomHighlightingDefinition(ruleSet);
	}
}
````

## File: ASM gen/Information Window/MemoryDataProvider.cs
````csharp
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ASM_gen.Information_Window;

public class MemoryDataProvider(ReadOnlyMemory<byte> rawMemory) : IList
{
	private const int BytesPerRow = 16;

	private ReadOnlyMemory<byte> _rawMemory = rawMemory;
	private readonly Dictionary<int, RowViewModel> _activeRows = [];

	public int Count => (int)Math.Ceiling((double)_rawMemory.Length / BytesPerRow);

	public object? this[int index]
	{
		get
		{
			// Если объект для этой строки уже создан WPF, возвращаем его
			if (_activeRows.TryGetValue(index, out var existingRow))
			{
				return existingRow;
			}

			ulong address = (ulong)(index * BytesPerRow);
			var newRow = new RowViewModel(address, _rawMemory);

			if (_activeRows.Count > 500) _activeRows.Clear();

			_activeRows[index] = newRow;
			return newRow;
		}
		set => throw new NotSupportedException();
	}

	public RowViewModel? GetCachedRow(int index)
	{
		_activeRows.TryGetValue(index, out var row);
		return row;
	}

	public bool IsReadOnly => true;
	public bool IsFixedSize => true;
	public bool IsSynchronized => false;
	public object SyncRoot => this;
	public int Add(object? value) => -1;
	public void Clear() 
	{ 
		_rawMemory = default;
		_activeRows.Clear();
	}
	public bool Contains(object? value) => false;
	public int IndexOf(object? value) => -1;
	public void Insert(int index, object? value) { }
	public void Remove(object? value) { }
	public void RemoveAt(int index) { }
	public void CopyTo(Array array, int index) { }
	public IEnumerator GetEnumerator()
	{
		int totalRows = Count;
		for (int i = 0; i < totalRows; i++)
		{
			yield return this[i];
		}
	}
}


public class RowViewModel : INotifyPropertyChanged
{
	private readonly ulong _address;
	private readonly ReadOnlyMemory<byte> _memory;

	// Кэш для проверки реальных изменений данных
	private ulong _cachedHash1;
	private ulong _cachedHash2;

	public event PropertyChangedEventHandler? PropertyChanged;

	public RowViewModel(ulong address, ReadOnlyMemory<byte> memory)
	{
		_address = address;
		_memory = memory;
		UpdateCache();
	}

	public string AddressHex => _address.ToString("X8");
	public string BytesHex => new MemoryRow(_address, _memory).GetBytesHex();
	public string AsciiText => new MemoryRow(_address, _memory).GetAsciiText();

	// Быстрый хэш 16 байт без аллокаций для детекта изменений
	private (ulong, ulong) GetCurrentHash()
	{
		int length = Math.Min(16, _memory.Length - (int)_address);
		ReadOnlySpan<byte> span = _memory.Span.Slice((int)_address, length);
		ulong h1 = 0;
		ulong h2 = 0;

		if (span.Length >= 8) h1 = BitConverter.ToUInt64(span[..8]);
		if (span.Length == 16) h2 = BitConverter.ToUInt64(span[8..16]);
		// Если хвост меньше 8/16 байт, можно добить побайтово (для оптимизации опущено)

		return (h1, h2);
	}

	private void UpdateCache() => (_cachedHash1, _cachedHash2) = GetCurrentHash();

	// Метод вызывается по таймеру ТОЛЬКО для видимых строк
	public void RefreshIfChanged()
	{
		var (newH1, newH2) = GetCurrentHash();

		// Если хэш совпал — память не изменилась, выходим без аллокации строк!
		if (newH1 == _cachedHash1 && newH2 == _cachedHash2) return;

		// Память изменилась — обновляем кэш и дергаем UI
		_cachedHash1 = newH1;
		_cachedHash2 = newH2;

		OnPropertyChanged(nameof(BytesHex));
		OnPropertyChanged(nameof(AsciiText));
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
````

## File: ASM gen/ProjectManage/Managers/Static/DirManager.cs
````csharp
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using VMApplication.Project;

namespace ASM_gen.ProjectManage.Managers.Static;

public static class DirManager
{


	/// <summary>
	/// Создает всю необходимую структуру папок для работы IDE при старте.
	/// </summary>
	public static void InitializeDirectories()
	{
		CreateDir(AppPaths.CurrentTemplatesPath);
		CreateDir(AppPaths.CurrentProjectsDataPath);
		CreateDir(AppPaths.CurrentUserProjectsPath);
		CreateDir(AppPaths.SharedIncludePath);
	}

	public static void NewFile(this IDEPage page, string projectPath, IProjectService projectService)
	{
		var dialog = new NewFileDialog
		{
			Owner = Window.GetWindow(page)
		};
		if (dialog.ShowDialog() == true && dialog.Result != null)
		{
			string ext = dialog.Result.Value.language == SourceLanguage.C ? ".c" : ".asm";
			string fileName = dialog.Result.Value.fileName + ext;
			string filePath = Path.Combine(projectPath, fileName);

			// Создаём файл на диске с базовым шаблоном
			string template = dialog.Result.Value.language == SourceLanguage.C
				? "int main() {\n    return 0;\n}\n"
				: "// Программа на ассемблере\nLDI r0, 0\nHALT\n";
			File.WriteAllText(filePath, template);

			// Добавляем в FileService и открываем вкладку
			string? result = projectService.EditorService.GetText(fileName);
			string content = result ?? string.Empty;
			projectService.EditorService.OpenTab(fileName, content);
		}
	}


	public static async Task<Dictionary<string, FileReadResult>> SearchProjects()
	{
		return await Task.Run(GetProjectFilesHeader);
	}

	private static Dictionary<string, FileReadResult> GetProjectFilesHeader()
	{
		CreateDir(AppPaths.CurrentUserProjectsPath);
		var results = new Dictionary<string, FileReadResult>();
		var projFiles = Directory.GetFiles(
			AppPaths.CurrentUserProjectsPath, AppPaths.ExtensionProj, SearchOption.AllDirectories);
		foreach (string projFile in projFiles)
		{
			string projectName = Path.GetFileNameWithoutExtension(projFile);
			string firstLine = File.ReadLines(projFile).FirstOrDefault() ?? string.Empty;
			results[projFile] = new FileReadResult(projectName, firstLine);
		}
		// Также можно добавить старую папку, если нужно
		return results;
	}

	public static string CreateNewProject(string projectName, bool useAsm)
	{
		string projectDir = Path.Combine(AppPaths.CurrentUserProjectsPath, projectName);
		if (Directory.Exists(projectDir))
			throw new InvalidOperationException("Проект с таким именем уже существует.");

		Directory.CreateDirectory(projectDir);

		// Создаём файл проекта (.vmproj) – простой текстовый контейнер с метаинформацией
		string projFilePath = Path.Combine(projectDir, $"{projectName}.vmproj");
		File.WriteAllLines(projFilePath, [
			$"ProjectName:{projectName}",
		"Version:1.0",
		"Language:" + (useAsm ? "ASM" : "C"),
		"Files:"
		]);

		// Создаём начальный исходный файл
		string ext = useAsm ? ".asm" : ".c";
		string sourceFileName = "main" + ext;
		string sourceFilePath = Path.Combine(projectDir, sourceFileName);

		string template = useAsm
			? "; main.asm\nLDI r0, 0\nHALT\n"
			: "int main() {\n    return 0;\n}\n";

		File.WriteAllText(sourceFilePath, template);

		// Возвращаем путь к файлу .vmproj (или к папке проекта, решай сам)
		return projFilePath;
	}

	public static ErrorFile TryLoadProjectFirstLine(string path, out string firstLine)
	{
		firstLine = string.Empty;

		if (!File.Exists(path))
			return ErrorFile.FileNotFound;

		try
		{
			using var reader = new StreamReader(path);
			firstLine = reader.ReadLine() ?? string.Empty;
			return firstLine.Length == 0 ? ErrorFile.EmptyFile : ErrorFile.None;
		}
		catch (UnauthorizedAccessException)
		{
			return ErrorFile.AccessDenied;
		}
		catch (IOException ex) when (IsFileLocked(ex))
		{
			return ErrorFile.FileLocked;
		}
		catch (Exception)
		{
			return ErrorFile.UnknownError;
		}
	}

	// Вспомогательный метод для проверки блокировки файла
	public static bool IsFileLocked(IOException exception)
	{
		int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
		return errorCode == 32 || errorCode == 33; // Коды ошибок Windows: Sharing violation / Lock violation
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void CreateDir(string path) => Directory.CreateDirectory(path);

	private static void OpenFile(string path)
	{
		using var reader = new StreamReader(path);
		int lenghtPathsFiles = GetLenghtPathsFiles(reader.ReadLine());
	}

	private static int GetLenghtPathsFiles(string? line)
	{
		if (line != null && int.TryParse(line.AsSpan(), out int res))
		{
			return res;
		}
		return 0;
	}

	public static bool TryOpenFile(string path, out ErrorFile error, out string pathProj)
	{
		pathProj = null!;

		try
		{
			if (!File.Exists(path))
			{
				error = ErrorFile.FileNotFound;
				return false;
			}

			FileInfo fileInfo = new(path);
			if (fileInfo.Length == 0)
			{
				error = ErrorFile.EmptyFile;
				return false;
			}

			using var reader = new StreamReader(path);

			string? pathProjFile = reader.ReadLine();
			if (string.IsNullOrWhiteSpace(pathProjFile))
			{
				error = ErrorFile.CorruptedData;
				return false;
			}
			if (!TryIdentifyPathProject(pathProjFile, out error)) return false;
			pathProj = pathProjFile;
			string? versionFile = reader.ReadLine();
			if (string.IsNullOrWhiteSpace(versionFile))
			{
				error = ErrorFile.CorruptedData;
				return false;
			}

			// .AsSpan() преобразует строку в ReadOnlySpan<char> без выделения памяти
			if (!TryIdentifyVersion(versionFile.AsSpan(), out error)) return false;
		}
		catch (UnauthorizedAccessException)
		{
			error = ErrorFile.AccessDenied;
			return false;
		}
		catch (IOException ex) when (IsFileLocked(ex))
		{
			error = ErrorFile.FileLocked;
			return false;
		}
		catch
		{
			error = ErrorFile.UnknownError;
			return false;
		}

		error = ErrorFile.None;
		return true;
	}

	private static bool TryIdentifyVersion(ReadOnlySpan<char> version, out ErrorFile error)
	{
		error = ErrorFile.None;
		Span<Range> componentRanges = stackalloc Range[4];
		int componentsCount = version.Split(componentRanges, '.');

		if (componentsCount < 1 || componentsCount > 4)
		{
			error = ErrorFile.InvalidVersion;
			return false;
		}

		Span<int> versionNumbers = stackalloc int[4];
		versionNumbers.Clear();

		for (int i = 0; i < componentsCount; i++)
		{
			ReadOnlySpan<char> componentSpan = version[componentRanges[i]];

			if (!int.TryParse(componentSpan, out int number) || number < 0)
			{
				error = ErrorFile.CorruptedData;
				return false;
			}

			versionNumbers[i] = number;
		}

		// Структура успешно создается на стеке
		VersionData parsedVersion = new(versionNumbers[0], versionNumbers[1], versionNumbers[2], versionNumbers[3]);
		return true;
	}

	private static bool TryIdentifyPathProject(ReadOnlySpan<char> path, out ErrorFile error)
	{
		// Вместо обращения к жесткому диску (File.Exists) просто проверяем валидность строки пути
		if (path.IsWhiteSpace() || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
		{
			error = ErrorFile.CorruptedData;
			return false;
		}

		error = ErrorFile.None;
		return true;
	}

	public readonly ref struct VersionData(int major, int minor, int build = 0, int revision = 0)
	{
		public readonly int Major = major;
		public readonly int Minor = minor;
		public readonly int Build = build;
		public readonly int Revision = revision;
	}

	public static string ErrorFileMessage(this ErrorFile error) => error switch
	{
		ErrorFile.None => "[Code Error: 0] Operation is Complited",
		ErrorFile.UnknownError => "[Code Error: 1] Неизвестная ошибка",
		ErrorFile.FileNotFound => "[Code Error: 2] Файл отсутствует или путь неверный",
		ErrorFile.FileNotFoundInProject => "[Code Error: 3] Файл есть на диске, но не находится в проекте",
		ErrorFile.DirectoryNotFound => "[Code Error: 4] Папка с проектами или метаданными удалена или отсутствует",
		ErrorFile.AlreadyExists => "[Code Error: 5] Файл или папка с таким именем уже существуют",
		ErrorFile.InvalidPathCharacters => "[Code Error: 6] Путь содержит запрещенные операционной системой символы",
		ErrorFile.AccessDenied => "[Code Error: 7] Недостаточно прав на чтение/запись файла",
		ErrorFile.FileLocked => "[Code Error: 8] Файл занят другим процессом",
		ErrorFile.EmptyFile => "[Code Error: 9] Файл пуст",
		ErrorFile.CorruptedData => "[Code Error: 10] Файл поврежден",
		ErrorFile.InvalidVersion => "[Code Error: 11] Версия проекта не поддерживается текущей системой",
		ErrorFile.PathTooLong => "[Code Error: 12] Путь к файлу превышает лимит символов",
		ErrorFile.DiskFull => "[Code Error: 13] На диске закончилось свободное место при попытке сохранения",
		_ => "[Code Error: 14] Ошибка не идентифицирована, невозможно определить проблему",
	};
}
````

## File: ASM gen/ProjectManage/Managers/WpfEditorService.cs
````csharp
using ASM_gen.Analizator;
using ASM_gen.Highlight;
using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VMApplication.Project;

namespace ASM_gen.ProjectManage.Managers;

public class WpfEditorService : IEditorService
{
	private readonly TabControl _tabControl;
	private readonly Dictionary<string, TabItem> _openTabs = [];
	private readonly Dictionary<string, ErrorLineColorizer> _errorColorizers = [];
	private readonly IFileService _fileService;

	public event EventHandler? TextChanged;

	public WpfEditorService(TabControl tabControl, IFileService fileService)
	{
		_tabControl = tabControl;
		_fileService = fileService;
		_tabControl.SelectionChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);
	}

	public void OpenTab(string fileName, string content)
	{
		Application.Current.Dispatcher.Invoke(() =>
		{
			if (_openTabs.TryGetValue(fileName, out var existingTab))
			{
				_tabControl.SelectedItem = existingTab;
				return;
			}

			var editor = new TextEditor
			{
				Text = content,
				// Базовая настройка внешнего вида (как в TabService.Configurate)
				Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
				Foreground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
				FontFamily = new FontFamily("Consolas"),
				FontSize = 12,
				ShowLineNumbers = true
			};
			editor.ChoseLang(LanguageType.C);
			editor.TextChanged += (s, e) => TextChanged?.Invoke(this, e);

			var errorColorizer = new ErrorLineColorizer();
			editor.TextArea.TextView.LineTransformers.Add(errorColorizer);
			_errorColorizers[fileName] = errorColorizer;

			var tabItem = new TabItem { Header = fileName, Content = editor };
			_openTabs[fileName] = tabItem;
			_tabControl.Items.Add(tabItem);
			_tabControl.SelectedItem = tabItem;
		});
	}

	public void CloseTab(string fileName)
	{
		Application.Current.Dispatcher.Invoke(() =>
		{
			if (_openTabs.TryGetValue(fileName, out var tab))
			{
				_tabControl.Items.Remove(tab);
				_openTabs.Remove(fileName);
				_errorColorizers.Remove(fileName);
			}
		});
	}

	public string GetCurrentText()
	{
		// Этот метод должен вызываться из UI-потока, или мы можем маршалировать
		if (Application.Current.Dispatcher.CheckAccess())
		{
			return GetTextInternal();
		}
		else
		{
			return Application.Current.Dispatcher.Invoke(GetTextInternal);
		}
	}

	private string GetTextInternal()
	{
		if (_tabControl.SelectedItem is TabItem tab && tab.Content is TextEditor editor)
			return editor.Text;
		return string.Empty;
	}

	public void SaveCurrentFile()
	{
		string? text = null;
		string? fileName = null;
		Application.Current.Dispatcher.Invoke(() =>
		{
			if (_tabControl.SelectedItem is TabItem tab && tab.Header is string header)
			{
				fileName = header;
				if (tab.Content is TextEditor editor)
					text = editor.Text;
			}
		});

		if (fileName != null && text != null)
			_fileService.SaveFile(fileName, text);
	}

	public string? GetCurrentFileName()
	{
		return Application.Current.Dispatcher.Invoke(() =>
			(_tabControl.SelectedItem as TabItem)?.Header as string);
	}

	public string? GetText(string fileName)
	{
		if (Application.Current.Dispatcher.CheckAccess())
			return GetTextInternal(fileName);
		else
			return Application.Current.Dispatcher.Invoke(() => GetTextInternal(fileName));
	}

	private string? GetTextInternal(string fileName)
	{
		if (_openTabs.TryGetValue(fileName, out var tab) && tab.Content is TextEditor editor)
			return editor.Text;
		return null;
	}

	public void HighlightErrors(IEnumerable<int> errorLines)
	{
		var fileName = GetCurrentFileName();
		if (fileName == null || !_errorColorizers.TryGetValue(fileName, out var colorizer))
			return;

		colorizer.ErrorLines = [.. errorLines];
		// Принудительно перерисовываем текущий редактор
		if (_tabControl.SelectedItem is TabItem tab && tab.Content is TextEditor editor)
			editor.TextArea.TextView.Redraw();
	}

	public void ClearHighlights()
	{
		var fileName = GetCurrentFileName();
		if (fileName == null || !_errorColorizers.TryGetValue(fileName, out var colorizer))
			return;

		colorizer.ErrorLines.Clear();
		if (_tabControl.SelectedItem is TabItem tab && tab.Content is TextEditor editor)
			editor.TextArea.TextView.Redraw();
	}
}
````

## File: ASM gen/ProjectManage/Managers/WpfFileService.cs
````csharp
using System.IO;
using VMApplication.Logger;
using VMApplication.Project;

namespace ASM_gen.ProjectManage.Managers;

public class WpfFileService(string projectPath, IOutputView outputView) : IFileService
{
	private readonly string _binDir = Path.Combine(projectPath, "bin");
	private readonly IOutputView _outputView = outputView;

	public string ProjectPath { get; set; } = projectPath;

	public IEnumerable<string> GetSourceFiles()
	{
		return Directory.GetFiles(ProjectPath, "*.c")
			.Concat(Directory.GetFiles(ProjectPath, "*.asm"))
			.Select(Path.GetFileName)!;
	}

	public string ReadFile(string fileName)
	{
		string fullPath = Path.Combine(ProjectPath, fileName);
		return File.ReadAllText(fullPath);
	}

	public void SaveFile(string fileName, string content)
	{
		string fullPath = Path.Combine(ProjectPath, fileName);
		File.WriteAllText(fullPath, content);
	}

	public bool Exists(string fileName)
	{
		string fullPath = Path.Combine(ProjectPath, fileName);
		return File.Exists(fullPath);
	}

	public void SaveProgramFile(byte[] prog)
	{
		Directory.CreateDirectory(_binDir);
		string filePath = Path.Combine(_binDir, "program.bin");
		File.WriteAllBytes(filePath, prog);
		_outputView.Append($"[SaveBinary] Программа сохранена в {filePath}");
	}
}
````

## File: ASM gen/DeviceManagerWindow.xaml
````
<Window x:Class="ASM_gen.DeviceManagerWindow"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:local="clr-namespace:ASM_gen" xmlns:viewmodels="clr-namespace:ASM_gen.ViewModels" d:DataContext="{d:DesignInstance Type=viewmodels:CreateDeviceViewModel}"
		mc:Ignorable="d"
		Title="Device Manager" Height="400" Width="950"
		WindowStartupLocation="CenterOwner" Loaded="Window_Loaded">
	<Grid Margin="10">
		<Grid.RowDefinitions>
			<RowDefinition Height="*"/>
			<RowDefinition Height="Auto"/>
			<RowDefinition Height="Auto"/>
		</Grid.RowDefinitions>

		<DataGrid x:Name="DeviceGrid" AutoGenerateColumns="False" IsReadOnly="True"
				  SelectionMode="Single" SelectionChanged="DeviceGrid_SelectionChanged">
			<DataGrid.Columns>
				<DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="50"/>
				<DataGridTextColumn Header="Имя" Binding="{Binding Name}" Width="180"/>
				<DataGridTextColumn Header="Размер RAM" Binding="{Binding RamSize}" Width="100"/>
				<DataGridTextColumn Header="Порт" Binding="{Binding Sector}" Width="90"/>
				<DataGridTextColumn Header="Диапазон адресов порта" Binding="{Binding PortRange}" Width="170"/>
				<DataGridTextColumn Header="Создан" Binding="{Binding CreatedAt, StringFormat='{}{0:HH:mm:ss}'}" Width="140"/>
			</DataGrid.Columns>
		</DataGrid>

		<StackPanel Grid.Row="1" Orientation="Horizontal" Margin="10,10, 10, 10">
			<Button Content="Загрузить .bin" Click="LoadBin_Click" Padding="10,5" Margin="5,5,10,5"/>
			<TextBlock Text="Новый сектор:" VerticalAlignment="Center"/>
			<TextBox x:Name="NewSectorBox" Width="30" Margin="5,10"/>
			<Button Content="Сменить сектор" Click="ChangeSector_Click" Padding="10,5" Margin="5,5"/>
			<Button Content="Сделать основным" Click="SetMainDevice_Click" Padding="10,5" Margin="10,5,5,5"/>
			<Button x:Name="BtnCreateDevice" Click="BtnCreateDevice_Click" 
					Content="Создать устройство" Padding="10,5" Margin="10,5,5,5" />
			<Button Content="Обновить BIOS" Click="UpdateBios_Click" Padding="10,5" Margin="5,5"/>
			<Button Content="Удалить устройство" Click="RemoveDevice_Click" Padding="10,5" Margin="5,5"/>
		</StackPanel>

		<StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10" HorizontalAlignment="Right">
			<Button Grid.Row="2" Content="Показать память (Information Window)"
					HorizontalAlignment="Right" Margin="5,10,5,5"
					Click="ShowMemory_Click" Padding="10,5"
					ToolTipService.InitialShowDelay="1" 
					ToolTip="Создать IW - Information Window, позволяет видеть в реальном времени за изменением данных у устройства"/>
			
			<Button  Grid.Row="2" x:Name="BtnUpdateGrid" Click="BtnUpdateGrid_Click" 
					Content="Обновить" Padding="10,5" Margin="5,10,5,5" />
			<Button  Grid.Row="2" x:Name="BtnCloseWindow" 
					Content="Подтвердить" Padding="10,5" Margin="5,10,5,5" Click="BtnCloseWindow_Click" />
		</StackPanel>
	</Grid>
</Window>
````

## File: ASM gen/IDEConsoleManager.cs
````csharp
using System.Runtime.InteropServices;

namespace ASM_gen;

public static partial class IDEConsoleManager
{
	// Константы Win32 API
	private const uint MF_BYCOMMAND = 0x00000000;
	private const uint SC_CLOSE = 0xF060;
	// Константы для управления видимостью
	private const int SW_HIDE = 0;     // Скрыть окно
	private const int SW_SHOW = 5;     // Показать окно

	public static void DisableCloseButton()
	{

		IntPtr hwnd = GetConsoleWindow();
		if (hwnd != IntPtr.Zero)
		{
			IntPtr hMenu = GetSystemMenu(hwnd, false);
			if (hMenu != IntPtr.Zero)
			{

				// Удаляем пункт "Закрыть", кнопка 'X' станет серой и неактивной
				RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
			}
		}
	}

	public static void InitConsole(bool useConsole)
	{
		nint handle = GetConsoleWindow();
		DisableCloseButton();
		if (handle != nint.Zero)
		{
			ShowWindow(handle, useConsole ? SW_SHOW : SW_HIDE);
		}

		if (useConsole)
		{
			Console.Clear();
		}

	}

	[LibraryImport("kernel32.dll")]
	private static partial nint GetConsoleWindow();

	[LibraryImport("user32.dll")]
	private static partial IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
````

## File: ASM gen/IDEPage.xaml
````
<Page x:Class="ASM_gen.IDEPage"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
		xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
		xmlns:avalonEdit="clr-namespace:ICSharpCode.AvalonEdit;assembly=ICSharpCode.AvalonEdit" xmlns:information_window="clr-namespace:ASM_gen.Information_Window" d:DataContext="{d:DesignInstance Type=information_window:RowViewModel}"
		mc:Ignorable="d"
		Title="IDE" 
		d:DesignHeight="600" d:DesignWidth="800">
	<Grid>
		<Grid.RowDefinitions>
			<RowDefinition Height="Auto" />
			<!-- Строка 0: Новая верхняя панель -->
			<RowDefinition Height="*" />
			<!-- Строка 1: Основной контент (была 0) -->
			<RowDefinition Height="Auto" />
			<!-- Строка 2: Разделитель (была 1) -->
			<RowDefinition Height="170" />
			<!-- Строка 3: Нижнее окно вывода (была 2) -->
		</Grid.RowDefinitions>

		<Grid.ColumnDefinitions>
			<ColumnDefinition Width="183"/>
			<ColumnDefinition/>
		</Grid.ColumnDefinitions>

		<!-- ВЕРХНЯЯ ПАНЕЛЬ УПРАВЛЕНИЯ -->
		<Menu Grid.Row="0" Grid.ColumnSpan="2" Background="#F0F0F0">
			<MenuItem Header="Файл">
				<MenuItem Header="Новый файл..." Click="NewFile_Click"/>
				<Separator/>
				<MenuItem Header="Сохранить все" Click="SaveAll_Click"/>
				<MenuItem Header="Выход" Click="Exit_Click"/>
			</MenuItem>
			<MenuItem Header="Настройки"/>
			<MenuItem Header="Помощь"/>
		</Menu>

		<!-- ЛЕВАЯ ПАНЕЛЬ КНОПОК (Перенесена в Grid.Row="1") -->
		<StackPanel Grid.Row="1" Grid.Column="0" Margin="10,10,10,10">
			<Button x:Name="BtnSaveBinary" Content="Сохранить .bin" Margin="5,5,5,0" Height="20"
		Click="BtnSaveBinary_Click"/>
			<Button x:Name="CompileAndLaunch" Click="BtnCompileAndLaunch" Content="Start" Margin="5,5,5,0" Height="20"/>
			<Button x:Name="StopDevice" Click="BtnStopDevices" Content="Stop all device" Margin="5,5,5,0" Height="20"/>
			<Button x:Name="BreakPointerLaunch" Click="BtnBreakPointerModeOne" Content="BreakPointer Mode" Margin="5,5,5,0" Height="20"/>
			<Button x:Name="BreakPointerOne" Click="BtnBreakPointerOne" Content="BreakPointer 1" Margin="5,5,5,0" Height="20"/>
			<Button x:Name="BreakPointerMulti" Click="BtnBreakPointerMulti" Content="BreakPointer multy" Margin="5,5,5,0" Height="20"/>

			<TextBox Name="countBreakPoint" Margin="20,5,20,0" Text="5" TextChanged="CountBreakPoint_TextChanged"/>

			<CheckBox Name="DebugMode" Content="Включить DEBUG" Margin="5,5,5,0" 
					  ToolTipService.InitialShowDelay="1"
					  ToolTip="Включение Debug режима, позволяет видеть большую часть информации об состояние данных"/>
			<CheckBox Name="ConsoleMode" Content="Включить консоль" Margin="5,5,5,0" 
					  ToolTipService.InitialShowDelay="1"
					  ToolTip="Выведенные данные будут появлятся только в консоли которую вы открыли" Checked="ConsoleMode_Checked"/>
			<CheckBox Name="DisassembleMode" Content="Показать ASM" Margin="5,5,5,0" 
					  ToolTipService.InitialShowDelay="1"
					  ToolTip="Включение Disassemble покажет ASM код, текущей программы"/>
			<CheckBox Name="UseBiosMode" Content="Запустить с Bios" Margin="5,5,5,0" 
					  ToolTipService.InitialShowDelay="1"
					  ToolTip="Компьютер запуститься сначала с Bios"/>
			<CheckBox Name="SnowTimerMode" Content="Запустить с таймером" Margin="5,5,5,0" 
					  ToolTipService.InitialShowDelay="1"
					  ToolTip="Вся машина запуститься с таймером"/>
			<CheckBox Name="OptimizationMode" Content="Оптимизировать код" Margin="5,5,5,0" 
					  ToolTipService.InitialShowDelay="1"
					  ToolTip="Оптимизация кода с логирование удалленых данных"/>

			<TextBlock x:Name="DelayThread" Margin="5,5,5,0" HorizontalAlignment="Center" FontSize="14">
				Интервал команд
			</TextBlock>

			<TextBox Name="DelayInput" Margin="20,5,20,0" Text="5" TextChanged="DelayInput_TextChanged"/>

			<Button x:Name="ClearOutput" Click="BtnClearOutput" 
			Content="Очистить вывод" Margin="5,5,5,0" Height="20"/>

			<Button x:Name="BtnDeviceManager" Content="Device Manager" Margin="5,5,5,0" Height="20"
			Click="BtnDeviceManager_Click" ToolTipService.InitialShowDelay="1" 
					ToolTip= "Просмотр всех устройств"/>
			
			<TextBlock x:Name="TxtCurrentDevice" Margin="5,5,5,0" Foreground="Gray" FontSize="11" Text=""/>
		</StackPanel>

		<!-- РЕДАКТОР КОДА (Перенесен в Grid.Row="1") -->
		<TabControl x:Name="tabEditor" Grid.Row="1" Grid.Column="1" Margin="12,10,10,4">

		</TabControl>

		<!-- НИЖНЕЕ ОКНО ВЫВОДА (Перенесено в Grid.Row="3") -->
		<Border Grid.Row="3" Grid.ColumnSpan="2" Margin="10,0,10,10" BorderBrush="Gray" BorderThickness="1">
			<ScrollViewer>
				<RichTextBox x:Name="outputBox" Background="#1E1E1E" Foreground="#DCDCDC"
						 FontFamily="Consolas" FontSize="10" IsReadOnly="True"
						 VerticalScrollBarVisibility="Auto" HorizontalScrollBarVisibility="Auto"
						 Padding="5" />
			</ScrollViewer>
		</Border>
	</Grid>
</Page>
````

## File: Compiller/ASM/Assembler.cs
````csharp
namespace Compiller.ASM;

/// <summary>
/// Сборщик машинного кода с поддержкой меток и выравнивания по 8 байт для 64-битных данных.
/// </summary>
public class Assembler
{
	private readonly MemoryStream _stream;
	private readonly BinaryWriter _writer;
	private readonly Dictionary<string, long> _labels = [];
	private readonly List<(long pos, string label)> _patches = [];
	private readonly ulong _baseAddress;

	public Assembler(ulong baseAddress = 0)
	{
		_baseAddress = baseAddress;
		_stream = new MemoryStream();
		_writer = new BinaryWriter(_stream);
	}

	/// <summary>Пометить текущую позицию меткой.</summary>
	public void MarkLabel(string name)
	{
		if (_labels.ContainsKey(name))
			throw new InvalidOperationException($"Метка '{name}' уже определена.");
		_labels[name] = _stream.Position;
	}

	/// <summary>Записать 32-битную инструкцию (без дополнительных данных).</summary>
	public void EmitInstruction(uint instruction)
	{
		_writer.Write(instruction);
	}

	/// <summary>Записать инструкцию с последующим 64-битным операндом (выравнивание по 8).</summary>
	public void EmitInstruction64(uint instruction, ulong data)
	{
		_writer.Write(instruction);
		Align8();
		_writer.Write(data);
	}

	/// <summary>Записать инструкцию перехода с меткой (адрес будет подставлен при сборке).</summary>
	public void EmitJump(uint jmpOpcode, string label)
	{
		_writer.Write(jmpOpcode);
		Align8();
		long pos = _stream.Position;
		_patches.Add((pos, label));
		_writer.Write(0UL); // placeholder
	}

	/// <summary>Записать инструкцию перехода на абсолютный адрес (без патча).</summary>
	public void EmitJumpToAbsolute(uint jmpOpcode, ulong absoluteTarget)
	{
		_writer.Write(jmpOpcode);
		Align8();
		_writer.Write(absoluteTarget);
	}

	private void Align8()
	{
		long pos = _stream.Position;
		long pad = ((pos + 7) & ~7) - pos;
		if (pad > 0)
			_writer.Write(new byte[pad]);
	}

	/// <summary>Собрать байт-код, подставить адреса меток.</summary>
	public byte[] Build()
	{
		byte[] bytes = _stream.ToArray();
		foreach (var (pos, label) in _patches)
		{
			if (!_labels.TryGetValue(label, out long targetPos))
				throw new InvalidOperationException($"Неопределённая метка: {label}");
			ulong absoluteAddr = _baseAddress + (ulong)targetPos;
			BitConverter.GetBytes(absoluteAddr).CopyTo(bytes, (int)pos);
		}
		return bytes;
	}

	public bool HasLabel(string name) => _labels.ContainsKey(name);
	public long GetStreamPosition() => _stream.Position;

	public void AddPatch(long position, string label)
	{
		_patches.Add((position, label));
	}

}
````

## File: Compiller/C/CodeGenerator/ExpressionGenerator.cs
````csharp
using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// ExpressionGenerator – генерация выражений
// ============================================================
public class ExpressionGenerator(Assembler asm,
								 GlobalMemoryManager globalMem,
								 FunctionContext funcCtx,
								 Dictionary<string, FunctionNode> functionTable,
								 Func<string> getLabel,
								 Dictionary<string, StructLayout> structTable)
{
	private readonly Assembler _asm = asm;
	private readonly GlobalMemoryManager _globalMem = globalMem;
	private readonly FunctionContext _funcCtx = funcCtx;
	private readonly Dictionary<string, FunctionNode> _functionTable = functionTable;
	private readonly Dictionary<string, StructLayout> _structTable = structTable;
	private readonly Func<string> _getLabel = getLabel;

	public void GenerateExpression(ASTNode expr)
	{
		switch (expr)
		{
			case NumberNode num:
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)num.Value);
				break;
			case IdentifierNode id:
				LoadVariableToR0(id.Name);
				break;
			case BinaryOpNode binop:
				GenerateBinaryOp(binop);
				break;
			case UnaryOpNode unop:
				GenerateUnaryOp(unop);
				break;
			case FunctionCallNode call:
				GenerateFunctionCall(call);
				break;
			case ArrayAccessNode arrAcc:
				LoadArrayElementToR0(arrAcc.ArrayName, arrAcc.Index);
				break;
			case AddressOfNode addrOf:
				GenerateAddressOf(addrOf);
				break;
			case DereferenceNode deref:
				GenerateDereference(deref);
				break;
			case NewArrayNode newArr:
				GenerateNewOp(newArr);
				break;
			case MemberAccessNode member:
				GenerateMemberAccess(member);
				break;
			default:
				throw new Exception($"Unsupported expression: {expr.GetType()}");
		}
	}

	public void GenerateMemberAddress(MemberAccessNode node)
	{
		string? structType;
		if (node.Object is IdentifierNode id)
		{
			// Ищем в локальных или глобальных
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
			{
				if (loc.StructTypeName != null)
					structType = loc.StructTypeName;
				else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
					structType = loc.PointedType; // ptr->field
				else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
			}
			else if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (_structTable.ContainsKey(gInfo.Type))
					structType = gInfo.Type;
				else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
					structType = gInfo.PointedType;
				else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
			}
			else throw new Exception($"Unknown variable '{id.Name}'");
		}
		else if (node.Object is MemberAccessNode node1)
		{
			// Рекурсивно вычисляем адрес вложенного объекта (результат в r0)
			GenerateMemberAddress(node1);

			// Получаем ТИП поля node1, чтобы понять, в какой структуре искать текущее поле
			var (_, _, innerFieldType) = ResolveMemberAccessType(node1);

			if (!_structTable.TryGetValue(innerFieldType, out var innerLayout))
				throw new Exception($"Unknown struct type '{innerFieldType}'");

			var currentField = innerLayout.GetField(node.FieldName)
							   ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{innerFieldType}'");

			if (currentField.Offset > 0)
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)currentField.Offset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
			}
			return;
		}
		else if (node.Object is DereferenceNode)
		{
			// *ptr  ->  ptr – указатель на структуру
			// Тип указателя должен быть известен. Пока не поддерживается, но можно через контекст.
			throw new Exception("Dereference member access not implemented yet");
		}
		else if (node.Object is ArrayAccessNode arrAcc)
		{
			// arr[i].field — адрес поля
			string? arrStructType = ResolveArrayStructType(arrAcc.ArrayName);
			var layoutstr = _structTable[arrStructType!];
			var fieldstr = layoutstr.GetField(node.FieldName)
						?? throw new Exception($"Field '{node.FieldName}' not found in struct '{arrStructType}'");

			// Вычисляем адрес элемента arr[i]
			GenerateExpression(arrAcc.Index);                    // r0 = i
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));
			LoadVariableToR0(arrAcc.ArrayName);                 // r0 = arr (указатель)
			int elemSize = layoutstr.Size;
			if (elemSize > 1)
				CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r2, elemSize);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r2));
			// r0 = адрес arr[i]

			// Прибавляем смещение поля
			if (fieldstr.Offset > 0)
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)fieldstr.Offset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
			}
			// Готово: r0 = адрес поля
			return;
		}
		else
		{
			throw new Exception("Unsupported object for member access");
		}

		if (structType == null)
			throw new Exception("Cannot determine struct type for member access");

		var layout = _structTable[structType];
		var field = layout.GetField(node.FieldName) ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{structType}'");

		if (node.IsArrow)
		{
			GenerateExpression(node.Object);  // r0 = ptr
		}
		else
		{
			if (node.Object is IdentifierNode identifer)
				LoadAddressToR0(identifer.Name);
			else throw new Exception("Dot address only for simple variables");
		}
		if (field.Offset > 0)
		{
			_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)field.Offset);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
		}
	}

	private void GenerateMemberAccess(MemberAccessNode node)
	{
		// 1. Определить StructLayout
		// Нужно узнать тип объекта. Он может быть переменной (IdentifierNode) или другим выражением.
		string? structType;
		if (node.Object is IdentifierNode id)
		{
			// Ищем в локальных или глобальных
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
			{
				if (loc.StructTypeName != null)
					structType = loc.StructTypeName;
				else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
					structType = loc.PointedType; // ptr->field
				else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
			}
			else if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (_structTable.ContainsKey(gInfo.Type))
					structType = gInfo.Type;
				else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
					structType = gInfo.PointedType;
				else throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
			}
			else throw new Exception($"Unknown variable '{id.Name}'");
		}
		else if (node.Object is MemberAccessNode)
		{
			// Рекурсивно вычисляем адрес вложенного объекта (без загрузки значения) в r0
			GenerateMemberAddress(node); // этот метод вычислит адрес поля
										 // Теперь r0 содержит адрес поля, загружаем значение
			var (_, fieldMem, _) = ResolveMemberAccessType(node);
			OpCodeSize fieldSizeMem = CodeGenUtils.GetSizeForType(fieldMem.Type);
			_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSizeMem.Uint));
			return;
		}
		else if (node.Object is DereferenceNode)
		{
			// *ptr  ->  ptr – указатель на структуру
			// Тип указателя должен быть известен. Пока не поддерживается, но можно через контекст.
			throw new Exception("Dereference member access not implemented yet");
		}
		else if (node.Object is ArrayAccessNode arrAcc)
		{
			string? arrStructType = ResolveArrayStructType(arrAcc.ArrayName);
			var layoutstr = _structTable[arrStructType!];
			var fieldstr = layoutstr.GetField(node.FieldName)
						?? throw new Exception($"Field '{node.FieldName}' not found in struct '{arrStructType}'");

			GenerateExpression(arrAcc.Index);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));
			LoadVariableToR0(arrAcc.ArrayName);
			int elemSize = layoutstr.Size;
			if (elemSize > 1)
				CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r2, elemSize);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r2));

			if (fieldstr.Offset > 0)
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)fieldstr.Offset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
			}

			// Загружаем значение поля
			OpCodeSize fieldSizestr = CodeGenUtils.GetSizeForType(fieldstr.Type);
			_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSizestr.Uint));
			return;
		}
		else
		{
			throw new Exception("Unsupported object for member access");
		}

		if (structType == null)
			throw new Exception("Cannot determine struct type for member access");

		var layout = _structTable[structType];
		var field = layout.GetField(node.FieldName) ?? throw new Exception($"Field '{node.FieldName}' not found in struct '{structType}'");

		// 2. Загрузить адрес начала структуры в r0
		if (node.IsArrow)
		{
			// Для ptr->field: ptr содержит адрес структуры, загружаем его.
			GenerateExpression(node.Object);  // r0 = ptr (значение указателя)
		}
		else
		{
			if (node.Object is IdentifierNode identifer)
			{
				LoadAddressToR0(identifer.Name);
			}
			else
			{
				throw new Exception("Dot access only supported for simple variables currently");
			}
		}

		if (field.Offset > 0)
		{
			_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)field.Offset);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
		}

		// 4. Загрузить значение поля
		OpCodeSize fieldSize = CodeGenUtils.GetSizeForType(field.Type);
		_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, fieldSize.Uint));
	}

	/// <summary>
	/// Возвращает (StructLayout родительской структуры, FieldInfo поля, тип поля)
	/// </summary>
	private (StructLayout layout, FieldInfo field, string fieldType) ResolveMemberAccessType(MemberAccessNode node)
	{
		string? structType = null;

		if (node.Object is IdentifierNode id)
		{
			// локальная или глобальная переменная
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
			{
				if (loc.StructTypeName != null)
					structType = loc.StructTypeName;
				else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
					structType = loc.PointedType;
			}
			else if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (_structTable.ContainsKey(gInfo.Type))
					structType = gInfo.Type;
				else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
					structType = gInfo.PointedType;
			}

			if (structType == null)
				throw new Exception($"'{id.Name}' is not a struct or pointer to struct");
		}
		else if (node.Object is MemberAccessNode innerMember)
		{
			// рекурсивно получаем тип поля внутреннего доступа
			var (_, _, innerFieldType) = ResolveMemberAccessType(innerMember);
			structType = innerFieldType; // тип поля, к которому обращаемся дальше
		}
		else if (node.Object is ArrayAccessNode arrAcc)
		{
			structType = ResolveArrayStructType(arrAcc.ArrayName);
		}
		else if (node.Object is DereferenceNode)
		{
			throw new Exception("Dereference in member access not yet supported");
		}
		else
		{
			throw new Exception("Unsupported object for member access");
		}

		if (structType == null)
			throw new Exception("Cannot determine struct type for member access");

		if (!_structTable.TryGetValue(structType, out var layout))
			throw new Exception($"Unknown struct type '{structType}'");

		var field = layout.GetField(node.FieldName)
					?? throw new Exception($"Field '{node.FieldName}' not found in struct '{structType}'");

		return (layout, field, field.Type);
	}

	public string? ResolveArrayStructType(string arrayName)
	{
		if (_funcCtx.VarMap.TryGetValue(arrayName, out var loc))
		{
			if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
				return loc.PointedType;
			if (loc.StructTypeName != null)
				return loc.StructTypeName;
		}
		else if (_globalMem.Contains(arrayName))
		{
			var gInfo = _globalMem.GetInfo(arrayName)!.Value;
			if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
				return gInfo.PointedType;
			if (_structTable.ContainsKey(gInfo.Type))
				return gInfo.Type;
		}
		throw new Exception($"'{arrayName}' is not a struct array or pointer to struct");
	}


	public void LoadAddressToR0(string varName)
	{
		if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
		{
			if (loc.IsRegister)
				throw new Exception($"Cannot take address of register variable '{varName}'");
			_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
		}
		else if (_globalMem.TryGetAddress(varName, out var addr))
		{
			_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), addr);
		}
		else throw new Exception($"Undefined variable '{varName}'");
	}

	private void LoadVariableToR0(string varName)
	{
		if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
			throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
		if (_globalMem.Contains(varName))
		{
			var gInfo = _globalMem.GetInfo(varName)!.Value;
			if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
				throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
		}

		if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
		{
			if (loc.IsRegister)
			{
				_asm.EmitInstruction(InstructionEncoder.EncodeR(
					OpCode.MOV.Uint, (uint)RegType.r0, (uint)loc.Register));
			}
			else
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
				_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
			}
		}
		else if (_globalMem.TryGetAddress(varName, out var addr))
		{
			GlobalInfo? gInfo = _globalMem.GetInfo(varName)!;
			var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
			_asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)RegType.r0, (uint)opSize), addr);
		}
		else
			throw new Exception($"Undefined variable: {varName}");
	}

	public void StoreR0ToVariable(string varName)
	{
		if (_funcCtx.VarMap.TryGetValue(varName, out var loca) && loca.StructTypeName != null)
			throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
		if (_globalMem.Contains(varName))
		{
			var gInfo = _globalMem.GetInfo(varName)!.Value;
			if (CodeGenUtils.IsStructType(gInfo.Type, _structTable))
				throw new Exception($"Direct load/store of struct variable '{varName}' is not supported.");
		}

		if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
		{
			if (loc.IsRegister)
			{
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)loc.Register, (uint)RegType.r0));
			}
			else
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
				_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
			}
		}
		else if (_globalMem.TryGetAddress(varName, out var addr))
		{
			var gInfo = _globalMem.GetInfo(varName)!;
			var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
			_asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, (uint)opSize), addr);
		}
		else
			throw new Exception($"Undefined variable: {varName}");
	}

	public void LoadArrayElementToR0(string arrayName, ASTNode indexExpr)
	{
		if (_funcCtx.VarMap.TryGetValue(arrayName, out var loc))
		{
			if (loc.IsArray)
				LoadArrayElementFromLocal(loc, indexExpr);
			else if (loc.IsPointer)
				LoadPointerElementToR0(arrayName, loc, indexExpr);
			else
				throw new Exception($"{arrayName} is not an array or pointer");
		}
		else if (_globalMem.Contains(arrayName))
		{
			var gInfo = _globalMem.GetInfo(arrayName)!.Value;
			if (gInfo.IsArray)
				LoadArrayElementFromGlobal(gInfo, indexExpr);
			else if (gInfo.IsPointer)
				LoadPointerElementToR0(arrayName, null, indexExpr);
			else
				throw new Exception($"{arrayName} is not an array or pointer");
		}
		else
			throw new Exception($"Undefined variable: {arrayName}");
	}

	public void StoreR0ToArrayElement(string arrayName, ASTNode indexExpr)
	{
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));

		if (_funcCtx.VarMap.TryGetValue(arrayName, out var loc))
		{
			if (loc.IsArray)
				StoreArrayElementToLocal(loc, indexExpr);
			else if (loc.IsPointer)
				StorePointerElement(arrayName, loc, indexExpr);
			else
				throw new Exception($"{arrayName} is not an array or pointer");
		}
		else if (_globalMem.Contains(arrayName))
		{
			var gInfo = _globalMem.GetInfo(arrayName)!.Value;
			if (gInfo.IsArray)
				StoreArrayElementToGlobal(gInfo, indexExpr);
			else if (gInfo.IsPointer)
				StorePointerElement(arrayName, null, indexExpr);
			else
				throw new Exception($"{arrayName} is not an array or pointer");
		}
		else
			throw new Exception($"Undefined variable: {arrayName}");
	}

	private void LoadArrayElementFromLocal(VarLocation loc, ASTNode indexExpr)
	{
		var typeSize = loc.TypeSize;
		GenerateExpression(indexExpr);
		if (typeSize != OpCodeSize.S8)
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
		_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
		_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)typeSize));
	}

	private void LoadArrayElementFromGlobal(GlobalInfo gInfo, ASTNode indexExpr)
	{
		var typeSize = CodeGenUtils.GetSizeForType(gInfo.Type);
		GenerateExpression(indexExpr);
		if (typeSize != OpCodeSize.S8)
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
		_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), gInfo.Address);
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
		_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, CodeGenUtils.TMP_REG, (uint)typeSize));
	}

	private void LoadPointerElementToR0(string pointerName, VarLocation? loc, ASTNode indexExpr)
	{
		// Получаем размер pointed типа
		OpCodeSize pointedSize;
		if (loc != null)
			pointedSize = CodeGenUtils.GetSizeForType(loc.PointedType!);
		else if (_globalMem.Contains(pointerName))
			pointedSize = CodeGenUtils.GetSizeForType(_globalMem.GetInfo(pointerName)!.Value.PointedType!);
		else
			throw new Exception("Cannot determine pointed type");

		// 1. Вычисляем индекс → r1
		GenerateExpression(indexExpr);   // r0 = индекс
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
		// 2. Загружаем адрес указателя → r0
		LoadVariableToR0(pointerName);   // r0 = адрес
										 // 3. Умножаем индекс на размер элемента
		if (pointedSize != OpCodeSize.S8)
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r1, CodeGenUtils.GetSizeInBytes(pointedSize));
		// 4. r0 = адрес + смещение
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r1));
		// 5. Загружаем значение
		_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, (uint)pointedSize));
	}

	private void StoreArrayElementToLocal(VarLocation loc, ASTNode indexExpr)
	{
		var typeSize = loc.TypeSize;
		GenerateExpression(indexExpr);
		if (typeSize != OpCodeSize.S8)
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
		_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
		_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, CodeGenUtils.TMP_REG, (uint)typeSize));
	}

	private void StoreArrayElementToGlobal(GlobalInfo gInfo, ASTNode indexExpr)
	{
		var typeSize = CodeGenUtils.GetSizeForType(gInfo.Type);
		GenerateExpression(indexExpr);
		if (typeSize != OpCodeSize.S8)
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, CodeGenUtils.GetSizeInBytes(typeSize));
		_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), gInfo.Address);
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.r0));
		_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, CodeGenUtils.TMP_REG, (uint)typeSize));
	}

	private void StorePointerElement(string pointerName, VarLocation? loc, ASTNode indexExpr)
	{
		OpCodeSize pointedSize;
		if (loc != null)
			pointedSize = CodeGenUtils.GetSizeForType(loc.PointedType!);
		else if (_globalMem.Contains(pointerName))
			pointedSize = CodeGenUtils.GetSizeForType(_globalMem.GetInfo(pointerName)!.Value.PointedType!);
		else
			throw new Exception("Cannot determine pointed type");

		// 1. Вычисляем индекс → r2
		GenerateExpression(indexExpr);
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r2, (uint)RegType.r0));

		// 2. Умножаем индекс на размер элемента (в r2)
		if (pointedSize != OpCodeSize.S8)
		{
			int elemSize = CodeGenUtils.GetSizeInBytes(pointedSize);
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r2, elemSize);
		}

		// 3. Загружаем адрес указателя → r0
		LoadVariableToR0(pointerName);

		// 4. r0 = адрес + смещение
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r0, (uint)RegType.r2));

		// 5. Сохраняем значение из r1 (которое пришло из StoreR0ToArrayElement)
		_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, (uint)pointedSize));
	}

	public void GenerateCondition(ASTNode condition, string? trueLabel, string? falseLabel)
	{
		if (condition is BinaryOpNode binop && CodeGenUtils.IsComparisonOperator(binop.Operator))
		{
			GenerateExpression(binop.Left);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
			GenerateExpression(binop.Right);
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.r1, (uint)RegType.r0));
			GenerateComparisonJump(binop.Operator, trueLabel, falseLabel);
		}
		else
		{
			GenerateExpression(condition);
			if (trueLabel != null && falseLabel != null)
			{
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), falseLabel);
			}
			else if (trueLabel != null)
			{
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
			}
			else if (falseLabel != null)
			{
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), falseLabel);
			}
		}
	}

	private void GenerateComparisonJump(string op, string? trueLabel, string? falseLabel)
	{
		void EmitJmp(string label) => _asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), label);
		void EmitCond(OpCode jmpOp, string label) => _asm.EmitJump(InstructionEncoder.EncodeJ((uint)jmpOp), label);

		if (trueLabel != null && falseLabel == null)
		{
			string skip = _getLabel();
			GenerateComparisonJump(op, trueLabel, skip);
			_asm.MarkLabel(skip);
			return;
		}
		if (falseLabel != null && trueLabel == null)
		{
			string skip = _getLabel();
			GenerateComparisonJump(op, skip, falseLabel);
			_asm.MarkLabel(skip);
			return;
		}
		if (trueLabel == null && falseLabel == null)
			return;

		switch (op)
		{
			case "==": EmitCond(OpCode.JZ, trueLabel!); EmitJmp(falseLabel!); break;
			case "!=": EmitCond(OpCode.JNZ, trueLabel!); EmitJmp(falseLabel!); break;
			case "<": EmitCond(OpCode.JL, trueLabel!); EmitJmp(falseLabel!); break;
			case ">":
				EmitCond(OpCode.JL, falseLabel!);
				EmitCond(OpCode.JZ, falseLabel!);
				EmitJmp(trueLabel!);
				break;
			case "<=":
				EmitCond(OpCode.JL, trueLabel!);
				EmitCond(OpCode.JZ, trueLabel!);
				EmitJmp(falseLabel!);
				break;
			case ">=":
				EmitCond(OpCode.JL, falseLabel!);
				EmitJmp(trueLabel!);
				break;
		}
	}

	private void GenerateBinaryOp(BinaryOpNode binop)
	{
		GenerateExpression(binop.Left);
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
		GenerateExpression(binop.Right);

		switch (binop.Operator)
		{
			case "+":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.r1, (uint)RegType.r0));
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
				break;
			case "-":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.r1, (uint)RegType.r0));
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
				break;
			case "*":
				_asm.EmitInstruction(InstructionEncoder.EncodeMULT_INT((uint)RegType.r1, (uint)RegType.r0));
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
				break;
			case "/":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.DIV.Uint, (uint)RegType.r1, (uint)RegType.r0));
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));
				break;
			case "<":
			case ">":
			case "<=":
			case ">=":
			case "==":
			case "!=":
				GenerateComparison(binop.Operator);
				break;
			case "&&":
			case "||":
				// заглушка
				break;
			default:
				throw new Exception($"Unsupported operator: {binop.Operator}");
		}
	}

	private void GenerateComparison(string op)
	{
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.r1, (uint)RegType.r0));
		_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, (uint)RegType.r1));

		string trueLabel = _getLabel();
		string endLabel = _getLabel();
		_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 0);

		switch (op)
		{
			case "<":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 2);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			// ... остальные case'ы "<", ">", "<=", ">=", "==", "!=" – полностью сохранены, как в оригинале
			case ">":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 3);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			case "<=":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 3);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			case ">=":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 2);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			case "==":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 1);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			case "!=":
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.rFL));
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r2), 1);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.AND.Uint, (uint)RegType.r1, (uint)RegType.r2));
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JZ.Uint), trueLabel);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			default:
				throw new NotImplementedException($"Comparison '{op}' not implemented");
		}
	}

	private void GenerateUnaryOp(UnaryOpNode unop)
	{
		switch (unop.Operator)
		{
			case "-":
				GenerateExpression(unop.Operand);
				_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.NOT.Uint, (uint)RegType.r0));
				_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.INC.Uint, (uint)RegType.r0));
				break;
			case "!":
				GenerateExpression(unop.Operand);
				string trueLabel = _getLabel();
				string endLabel = _getLabel();
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JNZ.Uint), trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 0);
				_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
				_asm.MarkLabel(trueLabel);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), 1);
				_asm.MarkLabel(endLabel);
				break;
			case "~":
				GenerateExpression(unop.Operand);
				_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.NOT.Uint, (uint)RegType.r0));
				break;
			default:
				throw new Exception($"Unsupported unary operator: {unop.Operator}");
		}
	}

	private void GenerateAddressOf(AddressOfNode addrOf)
	{
		if (addrOf.Operand is IdentifierNode id)
		{
			string varName = id.Name;
			if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
			{
				if (loc.IsRegister)
					throw new Exception($"Cannot take address of register variable '{varName}'");
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r0, CodeGenUtils.TMP_REG));
			}
			else if (_globalMem.TryGetAddress(varName, out var addr))
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), addr);
			}
			else throw new Exception($"Undefined variable '{varName}'");
		}
		else
			throw new Exception("Address-of only supports simple variables currently");
	}

	private void GenerateDereference(DereferenceNode deref)
	{
		GenerateExpression(deref.Operand);
		OpCodeSize size = GetPointedSize(deref.Operand);
		_asm.EmitInstruction(InstructionEncoder.EncodeLOAD_IND((uint)RegType.r0, (uint)RegType.r0, size.Uint));
	}

	private OpCodeSize GetPointedSize(ASTNode expr)
	{
		if (expr is IdentifierNode id)
		{
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc) && loc.IsPointer)
				return CodeGenUtils.GetSizeForType(loc.PointedType!);
			if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (gInfo.IsPointer)
					return CodeGenUtils.GetSizeForType(gInfo.PointedType!);
			}
		}
		throw new Exception($"Cannot determine pointed type for dereference of '{expr}'");
	}

	private void GenerateNewOp(NewArrayNode newArr)
	{
		int elementSize;
		if (_structTable.TryGetValue(newArr.Type, out var layout))
			elementSize = layout.Size;
		else
			elementSize = CodeGenUtils.GetSizeInBytes(CodeGenUtils.GetSizeForType(newArr.Type));

		GenerateExpression(newArr.Size);   // r0 = количество элементов
		if (elementSize > 1)
			CodeGenUtils.EmitMultiplyByConstant(_asm, (uint)RegType.r0, elementSize);
		_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.ALLOC.Uint, 0));
	}

	private void GenerateFunctionCall(FunctionCallNode call)
	{
		if (!_functionTable.TryGetValue(call.Name, out var targetFunc))
			throw new Exception($"Function '{call.Name}' not found");

		if (call.Arguments.Count != targetFunc.Parameters.Count)
			throw new Exception($"Argument count mismatch for function '{call.Name}'");

		for (int i = 0; i < call.Arguments.Count; i++)
		{
			GenerateExpression(call.Arguments[i]);
			var param = targetFunc.Parameters[i];
			string globalName = $"__param_{targetFunc.Name}_{param.Name}";
			if (!_globalMem.TryGetAddress(globalName, out var addr))
				throw new Exception($"Parameter global not found: {globalName}");
			OpCodeSize opSize;
			if (param.IsPointer)
				opSize = OpCodeSize.S64;
			else if (_structTable.ContainsKey(param.Type))
				opSize = OpCodeSize.S64;   // значение-структура – пока не реализовано
			else
				opSize = CodeGenUtils.GetSizeForType(param.Type);
			_asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, opSize.Uint), addr);
		}

		_asm.EmitJump(InstructionEncoder.EncodeCALL(), $"func_{call.Name}");
	}

	public void StoreToVariable(string varName, RegType srcReg)
	{
		if (_funcCtx.VarMap.TryGetValue(varName, out var loc))
		{
			if (loc.IsRegister)
			{
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)loc.Register, (uint)srcReg));
			}
			else
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)loc.StackOffset);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, CodeGenUtils.TMP_REG, (uint)RegType.rSP));
				_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)srcReg, CodeGenUtils.TMP_REG, (uint)loc.TypeSize));
			}
		}
		else if (_globalMem.TryGetAddress(varName, out var addr))
		{
			var gInfo = _globalMem.GetInfo(varName)!;
			var opSize = CodeGenUtils.GetSizeForType(gInfo.Value.Type);
			_asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)srcReg, (uint)opSize), addr);
		}
		else
			throw new Exception($"Undefined variable: {varName}");
	}
}
````

## File: Compiller/C/CodeGenerator/GlobalMemoryManager.cs
````csharp
namespace Compiller.C.CodeGenerator;

// ============================================================
// GlobalMemoryManager – управление глобальной памятью
// ============================================================
public class GlobalMemoryManager(Dictionary<string, StructLayout> structTable)
{
	private readonly Dictionary<string, GlobalInfo> _globalAddresses = [];
	private ulong _nextGlobalAddress = 0x1000;
	private readonly Dictionary<string, StructLayout> _structTable = structTable; // <-- добавить

	public IReadOnlyDictionary<string, GlobalInfo> Addresses => _globalAddresses;

	public GlobalInfo Allocate(string name, string type, bool isArray = false, bool isPointer = false, string? pointedType = null, int arraySize = 0)
	{
		int size;
		string realType = type;
		if (CodeGenUtils.IsStructType(type, _structTable))
		{
			size = _structTable[type].Size;
			realType = type; // сохраняем имя структуры
		}
		else if (isPointer)
			size = 8;
		else
			size = CodeGenUtils.GetSizeInBytes(CodeGenUtils.GetSizeForType(type));

		if (isArray)
			size *= arraySize;

		var info = new GlobalInfo(_nextGlobalAddress, realType, isArray, isPointer, pointedType);
		_globalAddresses[name] = info;
		_nextGlobalAddress += (ulong)size;
		_nextGlobalAddress = (_nextGlobalAddress + 7) & ~7UL;
		return info;
	}
	public void AllocatePseudoGlobals(FunctionNode func)
	{
		foreach (var param in func.Parameters)
		{
			string globalName = $"__param_{func.Name}_{param.Name}";
			Allocate(globalName, param.Type);
		}
	}

	public void AllocateLocalGlobals(BlockNode block, string funcName)
	{
		foreach (var stmt in block.Statements)
		{
			if (stmt is VariableNode var)
			{
				string globalName = $"__local_{funcName}_{var.Name}";
				Allocate(globalName, var.Type, var.IsArray, var.IsPointer, var.PointedType, var.ArraySize);
			}
			else if (stmt is BlockNode nested)
				AllocateLocalGlobals(nested, funcName);
		}
	}

	public bool TryGetAddress(string name, out ulong address)
	{
		if (_globalAddresses.TryGetValue(name, out var info))
		{
			address = info.Address;
			return true;
		}
		address = 0;
		return false;
	}

	public GlobalInfo? GetInfo(string name)
	{
		return _globalAddresses.TryGetValue(name, out var info) ? info : null;
	}

	public bool Contains(string name) => _globalAddresses.ContainsKey(name);
}
````

## File: Compiller/C/CodeGenerator/VarLocation.cs
````csharp
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// 4. ГЕНЕРАТОР КОДА
// ============================================================

public class VarLocation
{
	public bool IsRegister;
	public RegType Register;
	public int StackOffset;
	public OpCodeSize TypeSize;
	public bool IsArray;
	public int ArraySize;
	public bool IsPointer;
	public string? PointedType;
	public string? StructTypeName;
}
````

## File: Compiller/C/ASTNode.cs
````csharp
namespace Compiller.C;

// ============================================================
// 2. АБСТРАКТНОЕ СИНТАКСИЧЕСКОЕ ДЕРЕВО (AST)
// ============================================================
public abstract class ASTNode { }

public class ProgramNode : ASTNode
{
	public List<StructDeclNode> Structs { get; } = [];
	public List<FunctionNode> Functions { get; } = [];
	public List<VariableNode> Globals { get; } = [];
	public List<string> Includes { get; } = [];   // <-- новое
}

public class FunctionNode(string name, string returnType, BlockNode body) : ASTNode
{
	public string Name { get; } = name;
	public string ReturnType { get; } = returnType;
	public List<ParameterNode> Parameters { get; set; } = [];
	public BlockNode Body { get; } = body;
	public int LocalSize { get; set; }
	public bool IsExternal { get; set; } = false;  // новое поле
}

public class ParameterNode(string type, string name) : ASTNode
{
	public string Type { get; } = type;
	public string Name { get; } = name;
	public bool IsPointer { get; set; }
	public string? PointedType { get; set; }
}

public class BlockNode : ASTNode
{
	public List<ASTNode> Statements { get; } = [];
}

public class VariableNode(string type, string name, ASTNode? initializer = null) : ASTNode
{
	public string? StructTypeName { get; } = null;
	public string Type { get; } = type;
	public string Name { get; } = name;
	public ASTNode? Initializer { get; } = initializer;
	public bool IsPointer { get; set; } = false;
	public string? PointedType { get; set; } // например "int"
	public bool IsArray { get; set; } = false;
	public int ArraySize { get; set; } = 0;   // количество элементов
}

public class AssignmentNode(string name, ASTNode value, ASTNode? indexExpr = null) : ASTNode
{
	public string Name = name;
	public ASTNode Value = value;
	public ASTNode? IndexExpr = indexExpr; // null если присваивание скаляру, иначе индекс для массива
	public ASTNode? LValue { get; set; }
}
public class BinaryOpNode(string op, ASTNode left, ASTNode right) : ASTNode
{
	public string Operator { get; } = op;
	public ASTNode Left { get; } = left;
	public ASTNode Right { get; } = right;
}

public class UnaryOpNode(string op, ASTNode operand) : ASTNode
{
	public string Operator { get; } = op; public ASTNode Operand { get; } = operand;
}

public class NumberNode(long value) : ASTNode
{
	public long Value { get; } = value;
}

public class IdentifierNode(string name) : ASTNode
{
	public string Name { get; } = name;
}

public class IfNode(ASTNode condition, BlockNode thenBlock, BlockNode? elseBlock = null) : ASTNode
{
	public ASTNode Condition { get; } = condition;
	public BlockNode ThenBlock { get; } = thenBlock;
	public BlockNode? ElseBlock { get; } = elseBlock;
}

public class WhileNode(ASTNode condition, BlockNode body) : ASTNode
{
	public ASTNode Condition { get; } = condition; public BlockNode Body { get; } = body;
}

public class ForNode(ASTNode? init, ASTNode? condition, ASTNode? increment, BlockNode body) : ASTNode
{
	public ASTNode? Init { get; } = init;
	public ASTNode? Condition { get; } = condition;
	public ASTNode? Increment { get; } = increment;
	public BlockNode Body { get; } = body;
}

public class ReturnNode(ASTNode? value = null) : ASTNode
{
	public ASTNode? Value { get; } = value;
}

public class FunctionCallNode(string name) : ASTNode
{
	public string Name { get; } = name;
	public List<ASTNode> Arguments { get; } = [];
}

public class ArrayAccessNode(string name, ASTNode index) : ASTNode
{
	public string ArrayName = name;       // имя переменной-массива
	public ASTNode Index = index;          // выражение для индекса
}

// Узел взятия адреса: &expr
public class AddressOfNode(ASTNode operand) : ASTNode
{
	public ASTNode Operand = operand;
}

// Узел разыменования: *expr
public class DereferenceNode(ASTNode operand) : ASTNode
{
	public ASTNode Operand = operand;
}

public class NewArrayNode(string type, ASTNode size) : ASTNode
{
	public string Type = type;      // "int", "byte" и т.д.
	public ASTNode Size = size;     // выражение, задающее количество элементов
}

public class InlineAsmNode(string asmCode) : ASTNode
{
	public string AsmCode = asmCode;   // текст между { и }
}

public class StructDeclNode(string name, List<(string type, string fieldName)> fields) : ASTNode
{
	public string Name = name;
	public List<(string type, string fieldName)> Fields = fields;
}

public class MemberAccessNode(ASTNode @object, string fieldName, bool isArrow) : ASTNode
{
	public ASTNode Object = @object;       // выражение, дающее объект (или указатель)
	public string FieldName = fieldName;
	public bool IsArrow = isArrow;         // true: ->, false: .
}
````

## File: Compiller/C/Lexer.cs
````csharp
using System.Collections.Frozen;

namespace Compiller.C;

// ============================================================
// 1. ЛЕКСИЧЕСКИЙ АНАЛИЗАТОР (без изменений)
// ============================================================
public enum TokenType
{
	Identifier, Number, String, Char,
	Keyword, Operator, Punctuation,
	Comment, Whitespace, EOF, Arrow   // оператор ->
}

public class Token(TokenType type, string value, int line, int column)
{
	public TokenType Type { get; } = type;
	public string Value { get; } = value;
	public int Line { get; } = line;
	public int Column { get; } = column;

	public override string ToString() => $"{Type}: '{Value}' at {Line}:{Column}";
}

public class Lexer(string source)
{
	private readonly string _source = source;
	private int _position;
	private int _line = 1;
	private int _column = 1;
	private readonly List<Token> _tokens = [];

	private static readonly FrozenSet<string> Keywords =
	[
		"int", "char", "void", "if", "else", "while", "for", "return",
		"break", "continue", "sizeof", "struct", "typedef", "enum",
		"byte", "ushort", "ulong", "new", "asm", "extern"
	];

	private static readonly FrozenSet<char> Operators =
	[
		'+', '-', '*', '/', '%', '=', '!', '<', '>', '&', '|', '^', '~','#'
	];

	private static readonly FrozenSet<char> Punctuation = ['(', ')', '{', '}', '[', ']', ';', ',', '.', ':'];
	public List<Token> Tokenize()
	{
		while (_position < _source.Length)
		{
			char c = _source[_position];

			if (char.IsWhiteSpace(c))
			{
				SkipWhitespace();
				continue;
			}

			if (c == '/' && Peek() == '/')
			{
				SkipLineComment();
				continue;
			}
			if (c == '/' && Peek() == '*')
			{
				SkipBlockComment();
				continue;
			}

			if (char.IsLetter(c) || c == '_')
			{
				ReadIdentifier();
				continue;
			}
			if (char.IsDigit(c))
			{
				ReadNumber();
				continue;
			}
			if (c == '"')
			{
				ReadString();
				continue;
			}
			if (c == '\'')
			{
				ReadChar();
				continue;
			}
			if (Operators.Contains(c))
			{
				ReadOperator();
				continue;
			}
			if (Punctuation.Contains(c))
			{
				_tokens.Add(new Token(TokenType.Punctuation, c.ToString(), _line, _column));
				_position++;
				_column++;
				continue;
			}

			throw new Exception($"Unexpected character '{c}' at {_line}:{_column}");
		}

		_tokens.Add(new Token(TokenType.EOF, "", _line, _column));
		return _tokens;
	}

	private char Peek(int offset = 1) =>
		_position + offset < _source.Length ? _source[_position + offset] : '\0';

	private void SkipWhitespace()
	{
		while (_position < _source.Length && char.IsWhiteSpace(_source[_position]))
		{
			if (_source[_position] == '\n') { _line++; _column = 1; }
			else _column++;
			_position++;
		}
	}

	private void SkipLineComment()
	{
		_position += 2;
		_column += 2;
		while (_position < _source.Length && _source[_position] != '\n')
		{
			_position++;
			_column++;
		}
	}

	private void SkipBlockComment()
	{
		_position += 2;
		_column += 2;
		while (_position < _source.Length - 1 && !(_source[_position] == '*' && _source[_position + 1] == '/'))
		{
			if (_source[_position] == '\n') { _line++; _column = 1; }
			else _column++;
			_position++;
		}
		_position += 2;
		_column += 2;
	}

	private void ReadIdentifier()
	{
		int start = _position;
		int startCol = _column;
		while (_position < _source.Length && (char.IsLetterOrDigit(_source[_position]) || _source[_position] == '_'))
		{
			_position++;
			_column++;
		}
		string value = _source[start.._position];
		TokenType type = Keywords.Contains(value) ? TokenType.Keyword : TokenType.Identifier;
		_tokens.Add(new Token(type, value, _line, startCol));
	}

	private void ReadNumber()
	{
		int start = _position;
		int startCol = _column;
		bool isHex = false, isFloat = false;
		if (_source[_position] == '0' && Peek() == 'x')
		{
			isHex = true;
			_position += 2;
			_column += 2;
		}
		while (_position < _source.Length)
		{
			char c = _source[_position];
			if (char.IsDigit(c) || (isHex && (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
			{
				_position++;
				_column++;
			}
			else if (c == '.' && !isFloat)
			{
				isFloat = true;
				_position++;
				_column++;
			}
			else break;
		}
		string value = _source[start.._position];
		_tokens.Add(new Token(TokenType.Number, value, _line, startCol));
	}

	private void ReadString()
	{
		int start = _position;
		int startCol = _column;
		_position++; _column++;
		while (_position < _source.Length && _source[_position] != '"')
		{
			if (_source[_position] == '\\') { _position += 2; _column += 2; }
			else { _position++; _column++; }
		}
		_position++; _column++;
		_tokens.Add(new Token(TokenType.String, _source[start.._position], _line, startCol));
	}

	private void ReadChar()
	{
		int start = _position;
		int startCol = _column;
		_position++; _column++;
		if (_source[_position] == '\\') { _position += 2; _column += 2; }
		else { _position++; _column++; }
		_position++; _column++;
		_tokens.Add(new Token(TokenType.Char, _source[start.._position], _line, startCol));
	}

	private void ReadOperator()
	{
		int start = _position;
		int startCol = _column;
		char c = _source[_position];
		// Двухсимвольные операторы (одинаковые символы)
		if ((c == '+' || c == '-' || c == '*' || c == '/' || c == '=' ||
			 c == '!' || c == '<' || c == '>' || c == '&' || c == '|') && Peek() == c)
		{
			_position += 2;
			_column += 2;
			_tokens.Add(new Token(TokenType.Operator, _source.Substring(start, 2), _line, startCol));
			return;
		}
		// Операторы присваивания с равенством (+=, -=, *=, /=, %=, &=, |=, ^=)
		if ((c == '+' || c == '-' || c == '*' || c == '/' || c == '%' ||
			 c == '&' || c == '|' || c == '^') && Peek() == '=')
		{
			_position += 2;
			_column += 2;
			_tokens.Add(new Token(TokenType.Operator, _source.Substring(start, 2), _line, startCol));
			return;
		}
		// Составные операторы сравнения: <=, >=, !=
		if ((c == '<' || c == '>' || c == '!') && Peek() == '=')
		{
			_position += 2;
			_column += 2;
			_tokens.Add(new Token(TokenType.Operator, _source.Substring(start, 2), _line, startCol));
			return;
		}

		// Обработка ->
		if (c == '-' && Peek() == '>')
		{
			_position += 2;
			_column += 2;
			_tokens.Add(new Token(TokenType.Arrow, "->", _line, startCol));
			return;
		}
		// Одиночные операторы
		_position++;
		_column++;
		_tokens.Add(new Token(TokenType.Operator, c.ToString(), _line, startCol));
	}
}
````

## File: Compiller/C/Parser.cs
````csharp
using System.Text;

namespace Compiller.C;

public class Parser(List<Token> tokens)
{
	private readonly List<Token> _tokens = tokens;
	private readonly HashSet<string> _structNames = new(StringComparer.Ordinal);
	private int _position;
	private Token Current => _tokens[_position];
	public Token CurrentToken => Current;

	public ProgramNode Parse()
	{
		var program = new ProgramNode();
		while (Current.Type != TokenType.EOF)
		{
			if (Current.Type == TokenType.Keyword && Current.Value == "struct")
			{
				Advance();
				string name = Expect(TokenType.Identifier).Value;
				_structNames.Add(name);

				Expect(TokenType.Punctuation, "{");
				var fields = new List<(string, string)>();
				while (Current.Value != "}")
				{
					string ftype = ParseType();
					string fname = Expect(TokenType.Identifier).Value;
					fields.Add((ftype, fname));
					Expect(TokenType.Punctuation, ";");
				}
				Expect(TokenType.Punctuation, "}");
				if (Current.Type == TokenType.Punctuation && Current.Value == ";")
					Advance();
				program.Structs.Add(new StructDeclNode(name, fields));
				continue;
			}
			if (Current.Type == TokenType.Operator && Current.Value == "#")
			{
				Expect(TokenType.Operator, "#");
				string directive = Expect(TokenType.Identifier).Value; // "include"
				if (directive != "include")
					throw new Exception("Unknown preprocessor directive: #" + directive);
				string filePath = Expect(TokenType.String).Value.Trim('"');
				program.Includes.Add(filePath);
				// точка с запятой не требуется
				continue;
			}

			if (Current.Type == TokenType.Keyword && Current.Value == "extern")
			{
				Advance(); // съедаем extern
				string type = ParseType(); // тип возврата или тип переменной

				if (Current.Value == "*")
				{
					Advance();
				}

				string name = Expect(TokenType.Identifier).Value;

				if (Current.Value == "(") // функция
				{
					Expect(TokenType.Punctuation, "(");
					var parameters = new List<ParameterNode>();
					if (Current.Value != ")")
					{
						while (true)
						{
							string paramType = ParseType();
							bool paramIsPtr = false;
							if (Current.Value == "*")
							{
								paramIsPtr = true;
								Advance();
							}
							string paramName = Expect(TokenType.Identifier).Value;
							parameters.Add(new ParameterNode(paramType, paramName)
							{
								IsPointer = paramIsPtr,
								PointedType = paramType
							});
							if (Current.Value != ",") break;
							Expect(TokenType.Punctuation, ",");
						}
					}
					Expect(TokenType.Punctuation, ")");
					Expect(TokenType.Punctuation, ";");

					var extFunc = new FunctionNode(name, type, null!)
					{
						IsExternal = true,
						Parameters = parameters
					};
					program.Functions.Add(extFunc);
				}
				else // переменная (пока не обрабатываем, но можно пропустить)
				{
					// Ожидаем ';'
					while (Current.Value != ";") Advance();
					Advance(); // пропускаем ';'
				}
				continue; // переходим к следующему токену
			}

			if (IsTypeSpecifier())
			{
				string type = ParseType();

				bool isPointer = false;
				string pointedType = null!;

				if (Current.Value == "*")
				{
					isPointer = true;
					pointedType = type; // указатель на данный тип
					Advance(); // съедаем '*'
				}

				string name = Expect(TokenType.Identifier).Value;

				if (Current.Type == TokenType.Punctuation && Current.Value == "(")
				{
					var func = ParseFunction(type, name);
					program.Functions.Add(func);
				}
				else
				{

					// Обработка массивов и обычных переменных
					VariableNode varNode;
					if (Current.Value == "[") // массив
					{
						Expect(TokenType.Punctuation, "[");
						if (Current.Type != TokenType.Number)
							throw new Exception("Array size must be constant");
						int size = int.Parse(Current.Value);
						Advance();
						Expect(TokenType.Punctuation, "]");
						varNode = new VariableNode(type, name)
						{
							IsArray = true,
							ArraySize = size,
							IsPointer = isPointer,
							PointedType = pointedType
						};
					}
					else
					{
						varNode = ParseVariable(type, name);
						varNode.IsPointer = isPointer;
						varNode.PointedType = pointedType;
					}
					program.Globals.Add(varNode);
					Expect(TokenType.Punctuation, ";");
				}
			}
			else
			{
				throw new Exception($"Unexpected token: {Current}");
			}
		}
		return program;
	}

	private bool IsTypeSpecifier()
	{
		if (Current.Type == TokenType.Keyword &&
			(Current.Value == "int" || Current.Value == "char" || Current.Value == "void" ||
			 Current.Value == "byte" || Current.Value == "ushort" || Current.Value == "ulong"))
			return true;

		if (Current.Type == TokenType.Identifier && _structNames.Contains(Current.Value))
			return true;

		// Разрешаем "struct TypeName"
		if (Current.Type == TokenType.Keyword && Current.Value == "struct")
			return true;

		return false;
	}
	private string ParseType()
	{
		if (Current.Type == TokenType.Keyword && Current.Value == "struct")
		{
			Advance();
			string name = Expect(TokenType.Identifier).Value;
			if (!_structNames.Contains(name))
				throw new Exception($"Unknown struct type '{name}'");
			return name;
		}
		if (Current.Type == TokenType.Identifier && _structNames.Contains(Current.Value))
			return Expect(TokenType.Identifier).Value;
		return Expect(TokenType.Keyword).Value;
	}
	private FunctionNode ParseFunction(string returnType, string name)
	{
		Expect(TokenType.Punctuation, "(");
		var parameters = new List<ParameterNode>();
		if (Current.Value != ")")
		{
			while (true)
			{
				string paramType = ParseType();
				bool isPointer = false;
				if (Current.Value == "*")
				{
					isPointer = true;
					Advance(); // съедаем '*'
				}
				string paramName = Expect(TokenType.Identifier).Value;
				// Сохраняем информацию о том, что параметр — указатель, в ParameterNode
				// Для этого потребуется расширить ParameterNode (см. ниже)
				parameters.Add(new ParameterNode(paramType, paramName)
				{
					IsPointer = isPointer,
					PointedType = paramType
				});
				if (Current.Value != ",") break;
				Expect(TokenType.Punctuation, ",");
			}
		}
		Expect(TokenType.Punctuation, ")");
		Expect(TokenType.Punctuation, "{");
		var body = ParseBlock();
		var func = new FunctionNode(name, returnType, body)
		{
			Parameters = parameters // Ключевая строка!
		};
		return func;
	}
	private VariableNode ParseVariable(string type, string name)
	{
		ASTNode? initializer = null;
		if (Current.Value == "=")
		{
			Expect(TokenType.Operator, "=");
			initializer = ParseExpression();
		}
		return new VariableNode(type, name, initializer);
	}

	private BlockNode ParseBlock()
	{
		BlockNode block = new();
		while (Current.Value != "}")
		{
			if (IsTypeSpecifier())
			{
				string type = ParseType();

				bool isPointer = false;
				string? pointedType = null;
				if (Current.Value == "*")
				{
					isPointer = true;
					pointedType = type;
					Advance();
				}

				string name = Expect(TokenType.Identifier).Value;

				VariableNode varNode;
				if (Current.Value == "[") // массив
				{
					Expect(TokenType.Punctuation, "[");
					// размер – только константа (пока)
					if (Current.Type != TokenType.Number)
						throw new Exception("Array size must be constant");
					int size = int.Parse(Current.Value);
					Advance();
					Expect(TokenType.Punctuation, "]");
					varNode = new VariableNode(type, name)
					{
						IsArray = true,
						ArraySize = size,
						IsPointer = isPointer,
						PointedType = pointedType
					};
				}
				else
				{
					varNode = ParseVariable(type, name);
					varNode.IsPointer = isPointer;
					varNode.PointedType = pointedType;
				}
				block.Statements.Add(varNode);
				Expect(TokenType.Punctuation, ";");
			}
			else
			{
				block.Statements.Add(ParseBlockType(Current.Value));
			}
		}
		Expect(TokenType.Punctuation, "}");
		return block;
	}

	private ASTNode ParseBlockType(string text) => text switch
	{
		"if" => ParseIf(),
		"while" => ParseWhile(),
		"for" => ParseFor(),
		"return" => ParseReturn(),
		"asm" => ParseInlineAsm(),
		_ => ParseStatement(),
	};


	private IfNode ParseIf()
	{
		Expect(TokenType.Keyword, "if");
		Expect(TokenType.Punctuation, "(");
		ASTNode condition = ParseExpression();
		Expect(TokenType.Punctuation, ")");
		Expect(TokenType.Punctuation, "{");
		var thenBlock = ParseBlock();
		BlockNode? elseBlock = null;
		if (Current.Value == "else")
		{
			Expect(TokenType.Keyword, "else");
			Expect(TokenType.Punctuation, "{");
			elseBlock = ParseBlock();
		}
		return new IfNode(condition, thenBlock, elseBlock);
	}

	private WhileNode ParseWhile()
	{
		Expect(TokenType.Keyword, "while");
		Expect(TokenType.Punctuation, "(");
		ASTNode condition = ParseExpression();
		Expect(TokenType.Punctuation, ")");
		Expect(TokenType.Punctuation, "{");
		var body = ParseBlock();
		return new WhileNode(condition, body);
	}

	private ForNode ParseFor()
	{
		Expect(TokenType.Keyword, "for");
		Expect(TokenType.Punctuation, "(");

		ASTNode? init = null;
		if (Current.Value != ";")
		{
			if (IsTypeSpecifier())
			{
				// Объявление переменной: int i = 0
				string type = ParseType();

				bool isPointer = false;
				string? pointedType = null;
				if (Current.Value == "*")
				{
					isPointer = true;
					pointedType = type;
					Advance();
				}

				string name = Expect(TokenType.Identifier).Value;

				ASTNode? initializer = null;
				if (Current.Value == "=")
				{
					Expect(TokenType.Operator, "=");
					initializer = ParseExpression();
				}
				init = new VariableNode(type, name, initializer)
				{
					IsPointer = isPointer,
					PointedType = pointedType
				};
			}
			else
			{
				init = ParseExpression();
			}
		}
		Expect(TokenType.Punctuation, ";");

		ASTNode? condition = null;
		if (Current.Value != ";")
			condition = ParseExpression();
		Expect(TokenType.Punctuation, ";");

		ASTNode? increment = null;
		if (Current.Value != ")")
			increment = ParseExpression();
		Expect(TokenType.Punctuation, ")");

		Expect(TokenType.Punctuation, "{");
		var body = ParseBlock();
		return new ForNode(init, condition, increment, body);
	}

	private InlineAsmNode ParseInlineAsm()
	{
		Expect(TokenType.Keyword, "asm");
		Expect(TokenType.Punctuation, "{");

		var sb = new StringBuilder();
		int braceDepth = 1;
		while (braceDepth > 0)
		{
			if (Current.Type == TokenType.Punctuation && Current.Value == "{")
				braceDepth++;
			else if (Current.Type == TokenType.Punctuation && Current.Value == "}")
			{
				braceDepth--;
				if (braceDepth == 0) break;
			}

			// Добавляем токен к строке asm-кода
			sb.Append(Current.Value);
			sb.Append(' '); // простейшее восстановление пробелов
			Advance();
		}

		Expect(TokenType.Punctuation, "}");

		string asmCode = sb.ToString().Trim();
		return new InlineAsmNode(asmCode);
	}

	private ReturnNode ParseReturn()
	{
		Expect(TokenType.Keyword, "return");
		ASTNode? value = null;
		if (Current.Value != ";") value = ParseExpression();
		Expect(TokenType.Punctuation, ";");
		return new ReturnNode(value);
	}

	private ASTNode ParseStatement()
	{
		ASTNode expr = ParseExpression();
		Expect(TokenType.Punctuation, ";");
		return expr;
	}

	private ASTNode ParseExpression() => ParseAssignment();

	private ASTNode ParseAssignment()
	{
		ASTNode left = ParseLogicalOr();
		if (Current.Type == TokenType.Operator && Current.Value == "=")
		{
			Advance();
			ASTNode right = ParseAssignment();
			return left switch
			{
				IdentifierNode id => new AssignmentNode(id.Name, right) { LValue = left },
				ArrayAccessNode arr => new AssignmentNode(arr.ArrayName, right, arr.Index) { LValue = left },
				MemberAccessNode => new AssignmentNode(null!, right) { LValue = left },
				DereferenceNode => new AssignmentNode(null!, right) { LValue = left },
				_ => throw new Exception("Invalid assignment target"),
			};
		}
		return left;
	}
	private ASTNode ParseLogicalOr()
	{
		ASTNode left = ParseLogicalAnd();
		while (Current.Type == TokenType.Operator && Current.Value == "||")
		{
			string op = Current.Value;
			Advance();
			ASTNode right = ParseLogicalAnd();
			left = new BinaryOpNode(op, left, right);
		}
		return left;
	}


	private ASTNode ParseLogicalAnd()
	{
		ASTNode left = ParseEquality();
		while (Current.Type == TokenType.Operator && Current.Value == "&&")
		{
			string op = Current.Value;
			Advance();
			ASTNode right = ParseEquality();
			left = new BinaryOpNode(op, left, right);
		}
		return left;
	}

	private ASTNode ParseEquality()
	{
		ASTNode left = ParseRelational();
		while (Current.Type == TokenType.Operator && (Current.Value == "==" || Current.Value == "!="))
		{
			string op = Current.Value;
			Advance();
			ASTNode right = ParseRelational();
			left = new BinaryOpNode(op, left, right);
		}
		return left;
	}

	private ASTNode ParseRelational()
	{
		ASTNode left = ParseAdditive();
		while (Current.Type == TokenType.Operator && (Current.Value == "<" || Current.Value == ">" ||
													 Current.Value == "<=" || Current.Value == ">="))
		{
			string op = Current.Value;
			Advance();
			ASTNode right = ParseAdditive();
			left = new BinaryOpNode(op, left, right);
		}
		return left;
	}

	private ASTNode ParseAdditive()
	{
		ASTNode left = ParseMultiplicative();
		while (Current.Type == TokenType.Operator && (Current.Value == "+" || Current.Value == "-"))
		{
			string op = Current.Value;
			Advance();
			ASTNode right = ParseMultiplicative();
			left = new BinaryOpNode(op, left, right);
		}
		return left;
	}

	private ASTNode ParseMultiplicative()
	{
		ASTNode left = ParseUnary();
		while (Current.Type == TokenType.Operator && (Current.Value == "*" || Current.Value == "/" || Current.Value == "%"))
		{
			string op = Current.Value;
			Advance();
			ASTNode right = ParseUnary();
			left = new BinaryOpNode(op, left, right);
		}
		return left;
	}

	private ASTNode ParseUnary()
	{
		if (Current.Type == TokenType.Operator && (Current.Value == "+" || Current.Value == "-" ||
												   Current.Value == "!" || Current.Value == "~" ||
												   Current.Value == "*" || Current.Value == "&"))
		{
			string op = Current.Value;
			Advance();
			ASTNode operand = ParseUnary();
			if (op == "*") return new DereferenceNode(operand);
			if (op == "&") return new AddressOfNode(operand);
			return new UnaryOpNode(op, operand);
		}
		return ParsePrimary();
	}

	private ASTNode ParsePrimary()
	{
		ASTNode expr;

		// --- Базовые первичные выражения ---
		if (Current.Type == TokenType.Keyword && Current.Value == "new")
		{
			Advance(); // съедаем "new"
			string type = ParseType();

			if (Current.Type == TokenType.Punctuation && Current.Value == "[")
			{
				// массив: new Type[размер]
				Expect(TokenType.Punctuation, "[");
				ASTNode sizeExpr = ParseExpression();
				Expect(TokenType.Punctuation, "]");
				expr = new NewArrayNode(type, sizeExpr);
			}
			else if (Current.Type == TokenType.Punctuation && Current.Value == "(")
			{
				// вызов конструктора: new Type()
				Expect(TokenType.Punctuation, "(");
				// Аргументы конструктора пока не поддерживаются – просто ждём закрывающую скобку
				Expect(TokenType.Punctuation, ")");
				expr = new NewArrayNode(type, new NumberNode(1));
			}
			else
			{
				// на случай `new Type` без скобок (нежелательно, но оставлено для совместимости)
				expr = new NewArrayNode(type, new NumberNode(1));
			}
		}
		else if (Current.Type == TokenType.Number)
		{
			string numStr = Current.Value;
			long value;
			if (numStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				value = Convert.ToInt64(numStr, 16);
			else
				value = long.Parse(numStr);
			Advance();
			expr = new NumberNode(value);
		}
		else if (Current.Type == TokenType.Identifier)
		{
			string name = Current.Value;
			Advance();
			if (Current.Type == TokenType.Punctuation && Current.Value == "[")
			{
				// доступ к массиву a[i]
				Expect(TokenType.Punctuation, "[");
				ASTNode index = ParseExpression();
				Expect(TokenType.Punctuation, "]");
				expr = new ArrayAccessNode(name, index);
			}
			else if (Current.Type == TokenType.Punctuation && Current.Value == "(")
			{
				// вызов функции
				var call = new FunctionCallNode(name);
				Expect(TokenType.Punctuation, "(");
				if (Current.Value != ")")
				{
					while (true)
					{
						call.Arguments.Add(ParseExpression());
						if (Current.Value != ",") break;
						Expect(TokenType.Punctuation, ",");
					}
				}
				Expect(TokenType.Punctuation, ")");
				expr = call;
			}
			else
			{
				expr = new IdentifierNode(name);
			}
		}
		else if (Current.Value == "(")
		{
			Advance();
			expr = ParseExpression();
			Expect(TokenType.Punctuation, ")");
		}
		else
		{
			throw new Exception($"Unexpected token: {Current}");
		}

		// --- ЦИКЛ ПОСТФИКСНЫХ ОПЕРАТОРОВ: . и -> ---
		while (Current.Type == TokenType.Punctuation && Current.Value == "."
			   || Current.Type == TokenType.Arrow)
		{
			bool isArrow = Current.Type == TokenType.Arrow;
			Advance(); // пропускаем '.' или '->'
			string fieldName = Expect(TokenType.Identifier).Value;
			expr = new MemberAccessNode(expr, fieldName, isArrow);
		}

		return expr;
	}
	private Token Expect(TokenType type, string? value = null)
	{
		if (Current.Type != type)
			throw new Exception($"Expected {type}, got {Current.Type} at {Current.Line}:{Current.Column}");
		if (value != null && Current.Value != value)
			throw new Exception($"Expected '{value}', got '{Current.Value}' at {Current.Line}:{Current.Column}");
		Token token = Current;
		Advance();
		return token;
	}

	private void Advance() => _position++;
}
````

## File: Compiller/BiosBuilder.cs
````csharp
using Kernel.Common;

namespace Compiller;

public class BiosBuilder
{
	private readonly MemoryStream _ms = new();
	private readonly BinaryWriter _writer;
	private readonly ulong _baseAddress;
	private readonly Dictionary<string, long> _labels = [];
	private readonly List<(long patchPos, string label)> _patches = [];

	public BiosBuilder(RamSize baseAddress)
	{
		_baseAddress = (ulong)baseAddress;
		_writer = new BinaryWriter(_ms);
	}

	public void EmitInstruction(uint instruction)
	{
		_writer.Write(instruction);
	}

	public void EmitInstruction64(uint instruction, ulong data)
	{
		_writer.Write(instruction);
		Align8();
		_writer.Write(data);
	}

	public void EmitJump(uint jmpOpcode, string label)
	{
		_writer.Write(jmpOpcode);
		Align8();
		long addrPos = _ms.Position;
		_patches.Add((addrPos, label));
		_writer.Write(0UL); // placeholder
	}

	public void MarkLabel(string label)
	{
		_labels[label] = _ms.Position;
	}

	public void EmitJumpToAbsolute(uint jmpOpcode, ulong absoluteTarget)
	{
		_writer.Write(jmpOpcode);
		Align8();
		_writer.Write(absoluteTarget); // сразу пишем нужный адрес
	}

	public byte[] Build()
	{
		byte[] bios = _ms.ToArray();
		foreach (var (patchPos, label) in _patches)
		{
			if (!_labels.TryGetValue(label, out long targetPos))
				throw new InvalidOperationException($"Undefined label: {label}");

			ulong absoluteAddr = _baseAddress + (ulong)targetPos;
			BitConverter.GetBytes(absoluteAddr).CopyTo(bios, (int)patchPos);
		}
		return bios;
	}

	private void Align8()
	{
		long pos = _ms.Position;
		long pad = ((pos + 7) & ~7) - pos;
		if (pad > 0) _writer.Write(new byte[pad]);
	}
}

// Использование:
//var builder = new BiosBuilder(0x100000);
//builder.EmitInstruction64(EncodeLDI(R0), 5);
//builder.EmitInstruction64(EncodeLDI(R1), 3);
//builder.EmitInstruction(EncodeR(ADD, R0, R1));
//builder.EmitJump(EncodeJ(JG), "halt");
//builder.MarkLabel("halt");
//builder.EmitInstruction(EncodeHALT());
//byte[] bios = builder.Build();
````

## File: VMApplication/CallBacks/CallBackOnLaunch.cs
````csharp
using Kernel.Common;

namespace VMApplication.CallBacks;

public readonly struct CallBackOnLaunch(Action<Action<string, LogLevel>>? onLaunch = null,
										Action<Action<string, LogLevel>>? onStart = null,
										Action<Action<string, LogLevel>>? onEnd = null)
{
	public readonly Action<Action<string, LogLevel>>? OnTitleLaunch = onLaunch;
	public readonly Action<Action<string, LogLevel>>? OnStart = onStart;
	public readonly Action<Action<string, LogLevel>>? OnEnd = onEnd;
}

public readonly struct CallBackOnStopDecidedThread(Action<Action<string, LogLevel>>? act1 = null,
										Action<Action<string, LogLevel>>? act2 = null,
										Action<Action<string, LogLevel>>? act3 = null,
										Action<Action<string, LogLevel>>? act4 = null)
{
	public readonly Action<Action<string, LogLevel>>? OnThreadIsDead = act1;
	public readonly Action<Action<string, LogLevel>>? OnThreadIsLiveTrue = act2;
	public readonly Action<Action<string, LogLevel>>? OnThreadStopedTrue = act3;
	public readonly Action<Action<string, LogLevel>>? OnThreadStopedFalse = act4;
}

public readonly struct CallBackOnStop(Action<Action<string, LogLevel>>? act3 = null,
										Action<Action<string, LogLevel>>? act4 = null)
{
	public readonly Action<Action<string, LogLevel>>? OnThreadStopedTrue = act3;
	public readonly Action<Action<string, LogLevel>>? OnThreadStopedFalse = act4;
}
````

## File: VMApplication/Emulator/LaunchModeDevice.cs
````csharp
using Kernel.BiosSystem;
using Kernel.Common;
using ThreadingSystem.ThreadControl;
using VMApplication.CallBacks;
using VMApplication.Project;

namespace VMApplication.Emulator;

public class LaunchModeDevice
{
	private readonly Device _device;

	internal LaunchModeDevice(Device device) => _device = device;

	public void SetHeapAddress(ulong hp) => _device.InitHeap(hp);

	[Obsolete("""
	Используйте метод 'LaunchDeviceAsThreadTask', работающий через ThreadScheduler.
	
	Внимание: Прямой запуск потока может конфликтовать с планировщиком в режиме MaxThreadCPU, 
	вызывая просадки производительности. Сохраняйте возвращаемый 'ThreadHandle' для последующей остановки.
	""", error: false)]
	public void LaunchDeviceOnDedicatedThread(ulong startAddress = ProjectBuilder.BaseAdressProgramm,
							 bool debug = false,
							 int delayMs = 0,
							 bool showTimer = false,
							 CallBackOnLaunch callBack = default)
	{
		_device.LaunchDeviceOnDedicatedThread(startAddress,
							 debug,
							 delayMs,
							 showTimer,
							 callBack.OnTitleLaunch,
							 callBack.OnStart,
							 callBack.OnEnd);
	}

	[Obsolete("""
	Используйте новую перегрузку 'StopAndReset(ThreadHandle, ...)' и затем вручную сбросьте состояние.
	
	Внимание: Этот метод жестко завязан на старый механизм 'StopDevice'. 
	Попытка вызвать его для задачи из ThreadScheduler приведет к утечке ресурсов или зависанию, 
	так как у него нет доступа к дескриптору потока (ThreadHandle).
	""", error: false)]
	public DeviceContext StopAndReset(int stopTime = 3000, CallBackOnStopDecidedThread callBack = default)
	{
		_device.StopDevice(stopTime,
						   callBack.OnThreadIsLiveTrue,
						   callBack.OnThreadIsDead,
						   callBack.OnThreadStopedTrue,
						   callBack.OnThreadIsDead);
		return new DeviceContext(_device);
	}
	public DeviceContext StopAndReset(ThreadHandle handle, TimeSpan stopTime, CallBackOnStopDecidedThread callBack = default)
	{
		_device.Stop(handle,
					 stopTime,
					 callBack.OnThreadStopedTrue,
					 callBack.OnThreadIsDead);
		return new DeviceContext(_device);
	}

	public DeviceStepMode StepMode(ulong startAddress = ProjectBuilder.BaseAdressProgramm, Action<Action<string, LogLevel>>? titleAct = null)
	{
		_device.BreakPointerLaunchDevice(startAddress, titleAct);
		return new DeviceStepMode(_device);
	}
}
````

## File: VMApplication/Project/CompilationResult.cs
````csharp
using System.Text;

namespace VMApplication.Project;

// Результат компиляции
public readonly struct CompilationResult(byte[]? program, ulong startAdress, IReadOnlyList<string>? errors, OptimizationResultLog? log)
{
	public byte[]? Program { get; } = program;
	public ulong StartAdress { get; } = startAdress;
	public OptimizationResultLog? OptimizationResultLog { get; } = log;
	public IReadOnlyList<string>? Errors { get; } = errors;
	public bool Success => Errors == null || Errors.Count == 0;
}

public readonly struct OptimizationResultLog(StringBuilder inlinedFunc, StringBuilder removedNodes)
{
	public readonly StringBuilder InlinedFunc = inlinedFunc;
	public readonly StringBuilder RemovedNodes = removedNodes;
}
````

## File: VMApplication/Project/VMHostProject.cs
````csharp
using Kernel.Common;
using VMApplication.Logger;

namespace VMApplication.Project;

public sealed class VMHostProject(IProjectFilesConfig paths, IProjectService proj, VMHostLogger logger)
{
	private readonly IProjectFilesConfig _projectPaths = paths;
	private readonly IProjectService _projectService = proj;
	private readonly VMHostLogger _logger = logger;

	public string IncludePath => _projectPaths.IncludePath;

	public void OpenProject() => _projectService.OpenProject();

	public CompilationResult Compile(ulong baseAddress, bool optimize)
	{
		try
		{
			var (program, resLog) = ProjectBuilder.BuildProject(
				_projectService.FileService,
				_projectService.EditorService,
				_projectPaths,
				baseAddress,
				optimize);

			OptimizationResultLog? res = optimize
				? new(resLog.InlinedFunc, resLog.RemovedNodes)
				: null;

			return new CompilationResult(program, baseAddress, null, res);
		}
		catch (Exception ex)
		{
			_logger.Append(ex.Message, LogLevel.Error);
			return new CompilationResult(null, 0, [ex.Message], null);
		}
	}
}
````

## File: ASM gen/Highlight/SyntaxHighlighter.cs
````csharp
using System.Text.RegularExpressions;

namespace ASM_gen.Highlight
{
	public static partial class SyntaxHighlighter
	{
		[GeneratedRegex(@"^\s*#include\s+([<""][^>""]+[>""])", RegexOptions.Multiline)]
		public static partial Regex IncludeDirective();

		// Однострочный комментарий // ...
		[GeneratedRegex(@"//.*")]
		public static partial Regex CommentSingleLine();

		// Многострочный комментарий /* ... */ (поддерживает переводы строк)
		[GeneratedRegex(@"/\*[\s\S]*?\*/")]
		public static partial Regex CommentMultiLine();

		// Ключевые слова языка
		[GeneratedRegex(@"\b(byte|ushort|ulong|int|char|void|if|else|while|for|return|break|continue|extern|include|struct)\b")]
		public static partial Regex Keyword();

		// Числовые литералы: десятичные и шестнадцатеричные (0x...)
		[GeneratedRegex(@"\b0x[0-9a-fA-F]+|\b\d+\b")]
		public static partial Regex Number();

		// Строковый литерал в двойных кавычках с поддержкой escape-последовательностей
		[GeneratedRegex(@"""(?:\\.|[^""\\])*""")]
		public static partial Regex StringLiteral();

		// Символьный литерал в одинарных кавычках
		[GeneratedRegex(@"'(?:\\.|[^'\\])'")]
		public static partial Regex CharLiteral();

		// Операторы (составные и одиночные)
		[GeneratedRegex(@"[+\-*/%<>=!&|^~]+")]
		public static partial Regex Operator();

		// Знаки пунктуации (скобки, запятые, точка с запятой и пр.)
		[GeneratedRegex(@"[{}()\[\];,.:]")]
		public static partial Regex Punctuation();


		[GeneratedRegex(@";.*|//.*")]
		public static partial Regex RegexASMCommentColor();

		[GeneratedRegex(@"rZ|\b(r[0-9]|r1[0-9]|r2[0-2]|rCD|rFL|rLP|rCL|rRT|rSP|rHP|rIP)\b", RegexOptions.IgnoreCase, "ru-RU")]
		public static partial Regex RegexASMRegisterColor();

		[GeneratedRegex(@"\b(NOP|HALT|MOV|LOAD|STORE|LDI|ADD|SUB|INC|DEC|AND|OR|XOR|NOT|JMP|JZ|JNZ|JG|JL|PRINT|PUSH|POP|CALL|RET)\b", RegexOptions.IgnoreCase, "ru-RU")]
		public static partial Regex RegexASMKeywordColor();

		[GeneratedRegex(@"\b0x[0-9a-fA-F]+\b|\b\d+\b")]
		public static partial Regex RegexASMNumberColor();
	}
}
````

## File: ASM gen/Information Window/InformationWindow.xaml.cs
````csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ASM_gen.Information_Window;

/// <summary>
/// Логика взаимодействия для InformationWindow.xaml
/// </summary>
public partial class InformationWindow : Window
{
	private readonly DispatcherTimer _refreshTimer;
	private readonly MemoryDataProvider? _provider;
	private VirtualizingStackPanel? _panel;

	public InformationWindow(ReadOnlyMemory<byte> deviceMemory)
	{
		InitializeComponent();

		_provider = new MemoryDataProvider(deviceMemory);
		MemoryListView.ItemsSource = _provider;

		// Настраиваем таймер обновления (60 мс ≈ 15 FPS, оптимально для глаз и процессора)
		_refreshTimer = new DispatcherTimer(DispatcherPriority.Render)
		{
			Interval = TimeSpan.FromMilliseconds(60)
		};
		_refreshTimer.Tick += OnRefreshTick;
		_refreshTimer.Start();
	}

	private void OnRefreshTick(object? sender, EventArgs e)
	{
		if (_provider == null) return;

		// Находим VirtualizingStackPanel внутри ListView (достаточно сделать один раз)
		_panel ??= FindVisualChild<VirtualizingStackPanel>(MemoryListView);
		if (_panel == null) return;

		// Получаем индексы первой и последней видимой строки на экране
		int first = (int)Math.Floor(_panel.VerticalOffset);
		int last = (int)Math.Ceiling(_panel.VerticalOffset + _panel.ViewportHeight);

		// Ограничиваем диапазоны во избежание выхода за границы
		first = Math.Max(0, first);
		last = Math.Min(_provider.Count - 1, last);

		// Обновляем только те строки, на которые смотрит пользователь!
		for (int i = first; i <= last; i++)
		{
			if (_provider.GetCachedRow(i) is RowViewModel row)
			{
				row.RefreshIfChanged();
			}
		}
	}

	// Вспомогательный метод для обхода Visual Tree WPF
	private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
	{
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(obj, i);
			if (child is T t) return t;

			T? childOfChild = FindVisualChild<T>(child);
			if (childOfChild != null) return childOfChild;
		}
		return null;
	}

	protected override void OnClosed(EventArgs e)
	{
		_refreshTimer.Stop(); // Обязательно останавливаем таймер при закрытии окна IW
		_provider?.Clear();
		base.OnClosed(e);
	}
}
````

## File: ASM gen/StartWindow/MainMenu.xaml.cs
````csharp
using ASM_gen.NewProjectManage;
using ASM_gen.ProjectManage.Managers.Static;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using VMApplication.Project;

namespace ASM_gen.StartWindow
{
	public partial class MainMenu : Page
	{
		private Dictionary<string, FileReadResult> _projects = null!;
		public MainMenu()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// Вызываем ваш метод из DirectoryManager
			_projects = await DirManager.SearchProjects();

			// Привязываем словарь к ListBox
			LstFiles.ItemsSource = _projects;
		}

		private void ProjectButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button clickedButton)
			{
				string? filePath = clickedButton.Tag as string;
				if (!string.IsNullOrEmpty(filePath) && _projects.TryGetValue(filePath, out FileReadResult result))
				{
					if (result.Error != ErrorFile.None)
					{
						MessageBox.Show($"Невозможно открыть проект: {result.FileName}\nПричина: {result.Error}",
										"Ошибка чтения", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					string projectFolder = System.IO.Path.GetDirectoryName(filePath)!;
					NavigationService.Navigate(new IDEPage(projectFolder));
				}
			}
		}
		private async void BtnNewProj_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new NewProjectDialog { Owner = Window.GetWindow(this) };
			if (dialog.ShowDialog() == true && dialog.CreatedProjectPath != null)
			{
				// Обновляем список проектов
				_projects = await DirManager.SearchProjects();
				LstFiles.ItemsSource = _projects;
			}
		}
	}
}
````

## File: ASM gen/Utils/ErrorFileHelper.cs
````csharp
using VMApplication.Project;

namespace ASM_gen.Utils;

public static class ErrorFileHelper
{
	extension(ErrorFile error)
	{
		public bool IsSucced()
		{
			return error == ErrorFile.None;
		}
	}
}
````

## File: ASM gen/ViewModels/CreateDeviceViewModel.cs
````csharp
using ASM_gen.Utils;
using Kernel.Common;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ASM_gen.ViewModels;

public class CreateDeviceViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	// Списки для ComboBox (только RamSize, так как порты задаются в эмуляторе)
	public static IEnumerable<EnumItem> RamSizes => EnumExtensions.GetEnumItems<RamSize>();

	private RamSize _selectedRamSize = RamSize.Size1MB;
	private uint _sector = 0;
	private string _deviceName = "Device";
	private string _procName = "CPU";
	private string _ramName = "RAM";
	private string _portBusName = "PortBus";
	private string _biosPath = string.Empty;

	public RamSize SelectedRamSize
	{
		get => _selectedRamSize;
		set { _selectedRamSize = value; OnPropertyChanged(); }
	}

	public uint Sector
	{
		get => _sector;
		set { _sector = value; OnPropertyChanged(); }
	}

	public string DeviceName
	{
		get => _deviceName;
		set { _deviceName = value; OnPropertyChanged(); }
	}

	public string ProcName
	{
		get => _procName;
		set { _procName = value; OnPropertyChanged(); }
	}

	public string RamName
	{
		get => _ramName;
		set { _ramName = value; OnPropertyChanged(); }
	}

	public string PortBusName
	{
		get => _portBusName;
		set { _portBusName = value; OnPropertyChanged(); }
	}

	public string BiosPath
	{
		get => _biosPath;
		set { _biosPath = value; OnPropertyChanged(); }
	}

	// Результат после создания
	public DeviceCreationResult? Result { get; private set; }

	// Команды
	public ICommand BrowseBiosCommand { get; }
	public ICommand CreateCommand { get; }
	public ICommand CancelCommand { get; }

	// События для закрытия окна
	public event EventHandler<DeviceCreationResult>? DeviceCreated;
	public event EventHandler? Cancelled;

	public CreateDeviceViewModel()
	{
		BrowseBiosCommand = new RelayCommand(_ => BrowseBios());
		CreateCommand = new RelayCommand(_ => CreateDevice());
		CancelCommand = new RelayCommand(_ => Cancel());
	}

	private void BrowseBios()
	{
		var dialog = new Microsoft.Win32.OpenFileDialog
		{
			Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
			Title = "Select BIOS firmware"
		};
		if (dialog.ShowDialog() == true)
		{
			BiosPath = dialog.FileName;
		}
	}

	private void CreateDevice()
	{
		byte[] bios = [];
		if (!string.IsNullOrEmpty(BiosPath) && File.Exists(BiosPath))
		{
			bios = File.ReadAllBytes(BiosPath);
		}

		Result = new DeviceCreationResult
		{
			RamSize = SelectedRamSize,
			Sector = Sector,
			DeviceName = DeviceName,
			ProcName = ProcName,
			RamName = RamName,
			PortBusName = PortBusName,
			Bios = bios
		};
		DeviceCreated?.Invoke(this, Result);
	}

	private void Cancel() => Cancelled?.Invoke(this, EventArgs.Empty);

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class DeviceCreationResult : EventArgs
{
	public RamSize RamSize { get; init; }
	public uint Sector { get; init; }
	public string DeviceName { get; init; } = string.Empty;
	public string ProcName { get; init; } = string.Empty;
	public string RamName { get; init; } = string.Empty;
	public string PortBusName { get; init; } = string.Empty;
	public byte[] Bios { get; init; } = [];
}

public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) : ICommand
{
	private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
	private readonly Predicate<object?>? _canExecute = canExecute;

	public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
	public void Execute(object? parameter) => _execute(parameter);
	public event EventHandler? CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}
}
````

## File: ASM gen/ASM gen.slnx
````
<Solution>
  <Folder Name="/Элементы решения/">
	<File Path="../README.md" />
	<File Path="../Tests.md" />
  </Folder>
  <Folder Name="/Элементы решения/includes/">
	<File Path="../include/lib.vma" />
	<File Path="../include/math.vma" />
	<File Path="../include/ops.vma" />
	<File Path="../include/std.vma" />
	<File Path="../include/sys.vma" />
  </Folder>
  <Project Path="../Compiller/Compiller.csproj" Id="75871bb8-1afc-4dbd-a928-24eef852977b" />
  <Project Path="../Controllers/Kernel.csproj" Id="9d1173c7-0c5f-402c-a10c-a5f7752b4b31" />
  <Project Path="../Kernel.Common/Kernel.Common.csproj" Id="e0c23665-b03d-415f-8e85-c395a482ef5b" />
  <Project Path="../Tests/Tests.csproj" Id="c8fc1914-3fb2-432d-81b8-4c2764e89862" />
  <Project Path="../ThreadingSystem/ThreadingSystem.csproj" Id="92c1680c-9009-4b76-9979-b1777599d213" />
  <Project Path="../VMApplication/VMApplication.csproj" />
  <Project Path="ASM gen.csproj" />
</Solution>
````

## File: ASM gen/NewFileDialog.xaml.cs
````csharp
using System.Windows;
using System.Windows.Controls;
using VMApplication.Project;

namespace ASM_gen
{
	/// <summary>
	/// Логика взаимодействия для NewFileDialog.xaml
	/// </summary>
	public partial class NewFileDialog : Window
	{
		public (string fileName, SourceLanguage language)? Result { get; private set; }

		public NewFileDialog()
		{
			InitializeComponent();
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			string name = FileNameBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				MessageBox.Show("Введите имя файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			bool isC = ((ComboBoxItem)LanguageBox.SelectedItem).Content.ToString() == "C";
			Result = (name, isC ? SourceLanguage.C : SourceLanguage.Asm);
			DialogResult = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
````

## File: Compiller/ASM/Disassembler.cs
````csharp
using Kernel.Common;
using System.Text;

namespace Compiller.ASM;

public static class Disassembler
{

	/// <summary>
	/// Метод для декодировани и представления программы Assembler в текстовом виде
	/// </summary>
	/// <param name="program"> Байт код программы </param>
	/// <param name="length"> [OUTPUT] количество строк кода программы </param>
	/// <param name="size"> [OUTPUT] размер программы в byte</param>
	/// <param name="baseAddress"> Начальный адресс чтения программы </param>
	/// <returns> Текстовое представление Assembler</returns>
	public static string Disassemble(byte[] program, out int length, out int size, ulong baseAddress = 0)
	{
		StringBuilder sb = new();
		int pos = 0;
		size = program.Length;
		length = 0;

		// Имитируем программный счетчик (IP) как в CPU
		ulong currentIp = baseAddress;

		while (pos < size)
		{
			// Проверяем, хватает ли байт на саму инструкцию в массиве
			if (pos + 4 > size)
			{
				sb.AppendLine($"{currentIp:X8}: <неполная инструкция, пропущено {size - pos} байт>");
				break;
			}

			uint instruction = BitConverter.ToUInt32(program, pos);
			var opcode = InstructionDecoder.GetOpCode(instruction);

			string decoded = InstructionEncoder.Decode(instruction);
			sb.Append($"{currentIp:X8}: {instruction:X8}   {decoded}");

			// Симулируем шаг процессора: прочитали инструкцию, продвинули IP и POS
			ulong nextIp = currentIp + 4;
			int nextPos = pos + 4;

			if (InstructionDecoder.HasNeed64IntData(opcode))
			{
				// Выравнивание строго по абсолютному адресу IP, как в CPU
				ulong alignedIp = (nextIp + 7) & ~7UL;
				long pad = (long)(alignedIp - nextIp);

				// Сдвигаем позицию в массиве на величину паддинга
				nextPos += (int)pad;
				nextIp = alignedIp;

				if (nextPos + 8 <= size)
				{
					ulong data = BitConverter.ToUInt64(program, nextPos);
					sb.Append($"  data = 0x{data:X16} ({data})");
					nextPos += 8;
					nextIp += 8;
				}
				else
				{
					sb.Append($"  <недостаточно байт для данных>");
					sb.AppendLine();
					break;
				}
			}

			currentIp = nextIp;
			pos = nextPos;
			length++;
			sb.AppendLine();
		}

		return sb.ToString();
	}

	public static string Disassemble(ReadOnlyMemory<byte> program, out int length, out int size, ulong baseAddress = 0)
	{
		StringBuilder sb = new();
		int pos = 0;
		size = program.Length;
		length = 0;

		// Имитируем программный счетчик (IP) как в CPU
		ulong currentIp = baseAddress;

		while (pos < size)
		{
			// Проверяем, хватает ли байт на саму инструкцию в массиве
			if (pos + 4 > size)
			{
				sb.AppendLine($"{currentIp:X8}: <неполная инструкция, пропущено {size - pos} байт>");
				break;
			}
			uint instruction = BitConverter.ToUInt32(program.Span.Slice(pos, 4));
			var opcode = InstructionDecoder.GetOpCode(instruction);


			string decoded = InstructionEncoder.Decode(instruction);
			sb.Append($"{currentIp:X8}: {instruction:X8}   {decoded}");

			// Симулируем шаг процессора: прочитали инструкцию, продвинули IP и POS
			ulong nextIp = currentIp + 4;
			int nextPos = pos + 4;

			if (InstructionDecoder.HasNeed64IntData(opcode))
			{
				// Выравнивание строго по абсолютному адресу IP, как в CPU
				ulong alignedIp = (nextIp + 7) & ~7UL;
				long pad = (long)(alignedIp - nextIp);

				// Сдвигаем позицию в массиве на величину паддинга
				nextPos += (int)pad;
				nextIp = alignedIp;

				if (nextPos + 8 <= size)
				{
					ulong data = BitConverter.ToUInt64(program.Span.Slice(nextPos, 8));
					sb.Append($"  data = 0x{data:X16} ({data})");
					nextPos += 8;
					nextIp += 8;
				}
				else
				{
					sb.Append($"  <недостаточно байт для данных>");
					sb.AppendLine();
					break;
				}
			}

			currentIp = nextIp;
			pos = nextPos;
			length++;
			sb.AppendLine();
		}

		return sb.ToString();
	}

	public static string Disassemble(ReadOnlySpan<byte> program, out int length, out int size, ulong baseAddress = 0)
	{
		StringBuilder sb = new();
		int pos = 0;
		size = program.Length;
		length = 0;

		// Имитируем программный счетчик (IP) как в CPU
		ulong currentIp = baseAddress;

		while (pos < size)
		{
			// Проверяем, хватает ли байт на саму инструкцию в массиве
			if (pos + 4 > size)
			{
				sb.AppendLine($"{currentIp:X8}: <неполная инструкция, пропущено {size - pos} байт>");
				break;
			}

			uint instruction = BitConverter.ToUInt32(program.Slice(pos, 4));
			var opcode = InstructionDecoder.GetOpCode(instruction);


			string decoded = InstructionEncoder.Decode(instruction);
			sb.Append($"{currentIp:X8}: {instruction:X8}   {decoded}");

			// Симулируем шаг процессора: прочитали инструкцию, продвинули IP и POS
			ulong nextIp = currentIp + 4;
			int nextPos = pos + 4;

			if (InstructionDecoder.HasNeed64IntData(opcode))
			{
				// Выравнивание строго по абсолютному адресу IP, как в CPU
				ulong alignedIp = (nextIp + 7) & ~7UL;
				long pad = (long)(alignedIp - nextIp);

				// Сдвигаем позицию в массиве на величину паддинга
				nextPos += (int)pad;
				nextIp = alignedIp;

				if (nextPos + 8 <= size)
				{
					ulong data = BitConverter.ToUInt64(program.Slice(nextPos, 8));
					sb.Append($"  data = 0x{data:X16} ({data})");
					nextPos += 8;
					nextIp += 8;
				}
				else
				{
					sb.Append($"  <недостаточно байт для данных>");
					sb.AppendLine();
					break;
				}
			}

			currentIp = nextIp;
			pos = nextPos;
			length++;
			sb.AppendLine();
		}

		return sb.ToString();
	}

}
````

## File: Compiller/C/CodeGenerator/StatementGenerator.cs
````csharp
using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// StatementGenerator – генерация инструкций
// ============================================================
public class StatementGenerator(Assembler asm, ExpressionGenerator exprGen, FunctionContext funcCtx, Func<string> getLabel, GlobalMemoryManager globalMem, Dictionary<string, StructLayout> structTable)
{
	private readonly GlobalMemoryManager _globalMem = globalMem;
	private readonly Assembler _asm = asm;
	private readonly ExpressionGenerator _exprGen = exprGen;
	private readonly FunctionContext _funcCtx = funcCtx;
	private readonly Dictionary<string, StructLayout> _structTable = structTable;
	private readonly Func<string> _getLabel = getLabel;

	public void GenerateBlock(BlockNode block)
	{
		foreach (var stmt in block.Statements)
		{
			switch (stmt)
			{
				case VariableNode var:
					if (var.Initializer != null)
					{
						_exprGen.GenerateExpression(var.Initializer);
						_exprGen.StoreR0ToVariable(var.Name);
					}
					break;
				case AssignmentNode assign:
					GenerateAssignment(assign);
					break;
				case BinaryOpNode binop:
					_exprGen.GenerateExpression(binop);   // выражение как инструкция
					break;
				case UnaryOpNode unop:
					_exprGen.GenerateExpression(unop);
					break;
				case IfNode ifNode:
					GenerateIf(ifNode);
					break;
				case WhileNode whileNode:
					GenerateWhile(whileNode);
					break;
				case ForNode forNode:
					GenerateFor(forNode);
					break;
				case ReturnNode ret:
					GenerateReturn(ret);
					break;
				case FunctionCallNode call:
					_exprGen.GenerateExpression(call);
					break;
				case InlineAsmNode asm:
					GenerateInlineAsm(asm);
					break;
			}
		}
	}

	private void GenerateInlineAsm(InlineAsmNode asm)
	{
		// Парсим asm-строку и вставляем в текущий Assembler
		var asmParser = new AssemblerParser();
		asmParser.Assemble(asm.AsmCode, _asm);   // _asm должен быть доступен
	}

	private void GenerateAssignment(AssignmentNode assign)
	{
		// Присваивание элементу массива
		if (assign.IndexExpr != null)
		{
			_exprGen.GenerateExpression(assign.Value);
			_exprGen.StoreR0ToArrayElement(assign.Name, assign.IndexExpr);
			return;
		}

		if (assign.LValue != null)
		{
			if (assign.LValue is DereferenceNode deref)
			{
				_exprGen.GenerateExpression(assign.Value);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));
				_exprGen.GenerateExpression(deref.Operand);
				OpCodeSize size = GetPointedSize(deref.Operand);
				_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, (uint)size));
				return;
			}
		}

		if (assign.LValue is MemberAccessNode memberAccess)
		{
			// Генерируем значение правой части в r0, затем сохраняем в поле
			_exprGen.GenerateExpression(assign.Value);   // r0 = значение
														 // Сохраняем значение во временный регистр r1
			_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.MOV.Uint, (uint)RegType.r1, (uint)RegType.r0));

			// Вычисляем адрес поля
			_exprGen.GenerateMemberAddress(memberAccess); // нужно написать метод, возвращающий адрес в r0
														  // Сохраняем
			OpCodeSize size = GetFieldSize(memberAccess);
			_asm.EmitInstruction(InstructionEncoder.EncodeSTORE_IND((uint)RegType.r1, (uint)RegType.r0, size.Uint));
			return;
		}

		// Оптимизация x = x +/- ...
		if (assign.Value is BinaryOpNode binop &&
			(binop.Operator == "+" || binop.Operator == "-") &&
			binop.Left is IdentifierNode leftId &&
			leftId.Name == assign.Name &&
			_funcCtx.VarMap.TryGetValue(assign.Name, out var xLoc) &&
			xLoc.IsRegister)
		{
			RegType xReg = xLoc.Register;
			if (binop.Right is NumberNode num)
			{
				if (num.Value == 1 && binop.Operator == "+")
					_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.INC.Uint, (uint)xReg));
				else if (num.Value == 1 && binop.Operator == "-")
					_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.DEC.Uint, (uint)xReg));
				else
				{
					_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)RegType.r0), (ulong)num.Value);
					if (binop.Operator == "+")
						_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)xReg, (uint)RegType.r0));
					else
						_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)xReg, (uint)RegType.r0));
				}
				return;
			}
			else if (binop.Right is IdentifierNode)
			{
				// Загружаем значение y в r0
				_exprGen.GenerateExpression(binop.Right);
				// Теперь r0 содержит y, напрямую делаем ADD/SUB с xReg
				if (binop.Operator == "+")
					_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)xReg, (uint)RegType.r0));
				else
					_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)xReg, (uint)RegType.r0));
				return;
			}
			else
			{
				_exprGen.GenerateExpression(binop.Right);
				if (binop.Operator == "+")
					_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)xReg, (uint)RegType.r0));
				else
					_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)xReg, (uint)RegType.r0));
				return;
			}
		}

		// Обычное скалярное присваивание
		_exprGen.GenerateExpression(assign.Value);
		_exprGen.StoreR0ToVariable(assign.Name);
	}


	private OpCodeSize GetFieldSize(MemberAccessNode node)
	{
		var (_, field, _) = ResolveFieldInfo(node);
		return CodeGenUtils.GetSizeForType(field.Type);
	}

	private (StructLayout layout, FieldInfo field, string fieldType) ResolveFieldInfo(MemberAccessNode node)
	{
		// 1. Собираем цепочку полей от корневого объекта до самого вложенного
		var chain = new List<string>();
		MemberAccessNode? current = node;
		ASTNode? rootObject = null;

		while (current != null)
		{
			chain.Add(current.FieldName);
			if (current.Object is MemberAccessNode inner)
			{
				current = inner;
			}
			else
			{
				rootObject = current.Object;
				break;
			}
		}
		chain.Reverse(); // от внешнего к внутреннему

		// 2. Определяем тип корневого объекта
		string? rootStructType = null;
		if (rootObject is IdentifierNode id)
		{
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
			{
				if (loc.StructTypeName != null)
					rootStructType = loc.StructTypeName;
				else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
					rootStructType = loc.PointedType;
			}
			else if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (_structTable.ContainsKey(gInfo.Type))
					rootStructType = gInfo.Type;
				else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
					rootStructType = gInfo.PointedType;
			}
		}
		else if (rootObject is ArrayAccessNode arrAcc)
		{
			rootStructType = _exprGen.ResolveArrayStructType(arrAcc.ArrayName);
		}

		if (rootStructType == null)
			throw new Exception("Cannot determine struct type for member access");

		// 3. Проходим по цепочке, получая раскладку и поле на каждом уровне
		StructLayout? currentLayout = null;
		FieldInfo? currentField = null;
		string currentType = rootStructType;

		foreach (var fieldName in chain)
		{
			if (!_structTable.TryGetValue(currentType, out var layout))
				throw new Exception($"Unknown struct type '{currentType}'");
			currentLayout = layout;
			currentField = layout.GetField(fieldName)
						   ?? throw new Exception($"Field '{fieldName}' not found in struct '{currentType}'");
			currentType = currentField.Type;
		}

		return (currentLayout!, currentField!, currentType);
	}
	// Вспомогательный метод – определение типа структуры для данного MemberAccessNode
	private string? ResolveStructType(MemberAccessNode node)
	{
		// Собираем цепочку полей: для o.y.a -> ["y", "a"]
		var chain = new List<string>();
		MemberAccessNode? current = node;
		ASTNode? rootObject = null;

		// Раскручиваем цепочку MemberAccessNode до корневого объекта
		while (current != null)
		{
			chain.Add(current.FieldName);
			if (current.Object is MemberAccessNode inner)
			{
				current = inner;
			}
			else
			{
				rootObject = current.Object;
				break;
			}
		}
		chain.Reverse(); // теперь от корня к последнему полю

		// Определяем тип корневого объекта
		string? structType = null;
		if (rootObject is IdentifierNode id)
		{
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc))
			{
				if (loc.StructTypeName != null)
					structType = loc.StructTypeName;
				else if (loc.IsPointer && loc.PointedType != null && _structTable.ContainsKey(loc.PointedType))
					structType = loc.PointedType;
			}
			else if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (_structTable.ContainsKey(gInfo.Type))
					structType = gInfo.Type;
				else if (gInfo.IsPointer && gInfo.PointedType != null && _structTable.ContainsKey(gInfo.PointedType))
					structType = gInfo.PointedType;
			}
		}
		else if (rootObject is ArrayAccessNode arrAcc)
		{
			structType = _exprGen.ResolveArrayStructType(arrAcc.ArrayName);
		}

		if (structType == null)
			return null;

		// Проходим по цепочке полей, обновляя тип
		foreach (var fieldName in chain)
		{
			if (!_structTable.TryGetValue(structType, out var layout))
				return null;
			var field = layout.GetField(fieldName);
			if (field == null)
				return null;
			structType = field.Type;
		}

		return structType;
	}

	private OpCodeSize GetPointedSize(ASTNode expr)
	{
		if (expr is IdentifierNode id)
		{
			if (_funcCtx.VarMap.TryGetValue(id.Name, out var loc) && loc.IsPointer)
				return CodeGenUtils.GetSizeForType(loc.PointedType!);
			if (_globalMem.Contains(id.Name))
			{
				var gInfo = _globalMem.GetInfo(id.Name)!.Value;
				if (gInfo.IsPointer)
					return CodeGenUtils.GetSizeForType(gInfo.PointedType!);
			}
		}
		throw new Exception("Cannot determine pointed type");
	}
	private void GenerateIf(IfNode ifNode)
	{
		string elseLabel = _getLabel();
		string endLabel = ifNode.ElseBlock != null ? _getLabel() : elseLabel;

		_exprGen.GenerateCondition(ifNode.Condition, null, elseLabel);
		GenerateBlock(ifNode.ThenBlock);

		if (ifNode.ElseBlock != null)
		{
			_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), endLabel);
			_asm.MarkLabel(elseLabel);
			GenerateBlock(ifNode.ElseBlock);
			_asm.MarkLabel(endLabel);
		}
		else
		{
			_asm.MarkLabel(elseLabel);
		}
	}

	private void GenerateWhile(WhileNode whileNode)
	{
		string startLabel = _getLabel();
		string endLabel = _getLabel();

		_asm.MarkLabel(startLabel);
		_exprGen.GenerateCondition(whileNode.Condition, null, endLabel);
		GenerateBlock(whileNode.Body);
		_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), startLabel);
		_asm.MarkLabel(endLabel);
	}

	private void GenerateFor(ForNode forNode)
	{
		if (forNode.Init != null)
		{
			if (forNode.Init is VariableNode varInit)
			{
				if (varInit.Initializer != null)
				{
					_exprGen.GenerateExpression(varInit.Initializer);
					_exprGen.StoreR0ToVariable(varInit.Name);
				}
			}
			else if (forNode.Init is AssignmentNode assignInit)
			{
				GenerateAssignment(assignInit);
			}
			else
			{
				_exprGen.GenerateExpression(forNode.Init);
			}
		}

		string startLabel = _getLabel();
		string endLabel = _getLabel();

		_asm.MarkLabel(startLabel);
		if (forNode.Condition != null)
			_exprGen.GenerateCondition(forNode.Condition, null, endLabel);
		GenerateBlock(forNode.Body);
		if (forNode.Increment != null)
		{
			if (forNode.Increment is AssignmentNode assignIncr)
				GenerateAssignment(assignIncr);
			else
				_exprGen.GenerateExpression(forNode.Increment);
		}
		_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), startLabel);
		_asm.MarkLabel(endLabel);
	}

	private void GenerateReturn(ReturnNode ret)
	{
		if (ret.Value != null)
			_exprGen.GenerateExpression(ret.Value);
		if (_funcCtx.FunctionName == "main")
			_asm.EmitInstruction(InstructionEncoder.EncodeEND());
		else
			_asm.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), _funcCtx.EpilogueLabel);
	}
}
````

## File: Controllers/RamSystem/RAMResultInt64.cs
````csharp
using Kernel.Common;

namespace Kernel.RamSystem;

public readonly struct RAMResultInt64
{
	public readonly ulong Data;
	private readonly ulong _statusAndAddress;
	public readonly NameDeviceToken NameDeviceToken;

	public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

	public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

	public bool IsSuccess => Status == BiosStatus.Success;

	/// <summary>
	/// Конструктор для успеха
	/// </summary>
	/// <param name="data"> Данные для передачи </param>
	public RAMResultInt64(ulong data)
	{
		Data = data;
		_statusAndAddress = 0;
	}

	/// <summary>
	/// Конструктор для ошибки
	/// </summary>
	/// <param name="status"> Статус ошибки </param>
	/// <param name="adress"> Адресс ошибки в памяти</param>
	public RAMResultInt64(BiosStatus status, ulong adress, NameDeviceToken nameDeviceToken)
	{
		NameDeviceToken = nameDeviceToken;
		Data = 0;
		_statusAndAddress = ((ulong)status << 48) | (adress & 0x0000FFFFFFFFFFFFUL);
	}
}


public readonly struct RAMResultInt32
{
	public readonly uint Data;
	private readonly ulong _statusAndAddress;
	public readonly NameDeviceToken NameDeviceToken;

	public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

	public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

	public bool IsSuccess => Status == BiosStatus.Success;

	/// <summary>
	/// Конструктор для успеха
	/// </summary>
	/// <param name="data"> Данные для передачи </param>
	public RAMResultInt32(uint data)
	{
		Data = data;
		_statusAndAddress = 0;
	}

	/// <summary>
	/// Конструктор для ошибки
	/// </summary>
	/// <param name="status"> Статус ошибки </param>
	/// <param name="adress"> Адресс ошибки в памяти</param>
	public RAMResultInt32(BiosStatus status, ulong adress, NameDeviceToken nameDeviceToken)
	{
		NameDeviceToken = nameDeviceToken;
		Data = 0;
		_statusAndAddress = ((ulong)status << 48) | (adress & 0x0000FFFFFFFFFFFFUL);
	}
}

public readonly struct RAMResultInt16
{
	public readonly ushort Data;
	private readonly ulong _statusAndAddress;
	public readonly NameDeviceToken NameDeviceToken;
	public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

	public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

	public bool IsSuccess => Status == BiosStatus.Success;

	/// <summary> Конструктор для успеха </summary>
	public RAMResultInt16(ushort data)
	{
		Data = data;
		_statusAndAddress = 0;
	}

	/// <summary> Конструктор для ошибки </summary>
	public RAMResultInt16(BiosStatus status, ulong faultAddress, NameDeviceToken nameDeviceToken)
	{
		NameDeviceToken = nameDeviceToken;
		Data = 0;
		_statusAndAddress = ((ulong)status << 48) | (faultAddress & 0x0000FFFFFFFFFFFFUL);

	}
}

public readonly struct RAMResultInt8
{
	public readonly byte Data;
	private readonly ulong _statusAndAddress;
	public readonly NameDeviceToken NameDeviceToken;

	public BiosStatus Status => (BiosStatus)(_statusAndAddress >> 48);

	public ulong FaultAddress => _statusAndAddress & 0x0000FFFFFFFFFFFFUL;

	public bool IsSuccess => Status == BiosStatus.Success;

	/// <summary> Конструктор для успеха </summary>
	public RAMResultInt8(byte data)
	{
		Data = data;
		_statusAndAddress = 0;
	}

	/// <summary> Конструктор для ошибки </summary>
	public RAMResultInt8(BiosStatus status, ulong faultAddress, NameDeviceToken nameDeviceToken)
	{
		NameDeviceToken = nameDeviceToken;
		Data = 0;
		_statusAndAddress = ((ulong)status << 48) | (faultAddress & 0x0000FFFFFFFFFFFFUL);
	}
}
````

## File: Controllers/Kernel.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<TargetFramework>net10.0</TargetFramework>
	<ImplicitUsings>enable</ImplicitUsings>
	<Nullable>enable</Nullable>
	<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup>
	<ProjectReference Include="..\Kernel.Common\Kernel.Common.csproj" />
	<ProjectReference Include="..\ThreadingSystem\ThreadingSystem.csproj" />
  </ItemGroup>

</Project>
````

## File: VMApplication/Emulator/VMEmulator.cs
````csharp
using Kernel.Common;
using Kernel.LocalMemorySystem;
using VMApplication.Logger;
using static Kernel.Utilites.ManagerDevices;

namespace VMApplication.Emulator;

public sealed class VMEmulator(Compiller.Emulation.Emulator emulator, VMHostLogger outputView, SizePortOnDevice portsPerDevice)
{
	private readonly Compiller.Emulation.Emulator _emulator = emulator;
	private readonly VMHostLogger _outputView = outputView;
	private readonly SizePortOnDevice _portsPerDevice = portsPerDevice;

	public uint PortsPerDevice => (uint)_portsPerDevice;

	public event Action<IEnumerable<DeviceInfo>>? DeviceListChanged;

	public DeviceContext? CreateDeviceContext(int? id = null)
	{
		var device = id != null ? _emulator.GetDevice(id.Value) : _emulator.MainDevice;
		if (device == null)
		{
			_outputView.Append("Выбраное устройство/основное не инициализированы", LogLevel.Error);
			return null;
		}
		return new DeviceContext(device);
	}

	public bool ChangeDeviceSector(int id, uint newSector) => _emulator.ChangeDeviceSector(id, newSector);

	public DeviceContext? GetDeviceData(int id)
	{
		var d = _emulator.GetDevice(id);
		return d != null ? new(d) : null;
	}

	public IEnumerable<DeviceView> GetAllDevices()
	{
		return _emulator.AllDevices.Select(VMHostHelper.ConvertDeviceInfo);
	}

	public string GetDumpRegisters()
	{
		var res = _emulator.DumpRegisters();
		if (string.IsNullOrEmpty(res))
		{
			return "Не получилось получить дамб регистров";
		}
		return res;
	}

	public string GetDumpRegisters(int id)
	{
		var res = _emulator.DumpRegisters(id);
		if (string.IsNullOrEmpty(res))
		{
			return $"Не получилось получить дамб регистров для устройства {id}";
		}
		return res;
	}

	public int CreateDevice(byte[] bios, RamSize ramSize, uint sector, string? name = null, string? procName = null, string? ramName = null, string? portBusName = null)
	{
		int id = _emulator.CreateDevice(bios, ramSize, sector, name, procName, ramName, portBusName);
		if (id != -1)
			DeviceListChanged?.Invoke(_emulator.AllDevices);
		return id;
	}

	public void SetMainDevice(int deviceId)
	{
		_emulator.SetMainDevice(deviceId);
	}

	public DeviceView? MainDevice()
	{
		if (_emulator.MainDeviceId == -1 || _emulator.MainDevice == null) return null;
		var info = _emulator.GetDeviceInfo(_emulator.MainDeviceId);
		if (info == null) return null;
		return info.Value.ConvertDeviceInfo();
	}

	public int CreateDisk(string imagePath)
	{
		int sector = _emulator.CreateDisk(imagePath);
		if (sector != -1)
			_outputView.Append($"Disk created in sector {sector} (base port address: {sector * (int)_portsPerDevice})", LogLevel.Log);
		else
			_outputView.Append("Failed to create disk (no free port sectors available)", LogLevel.Error);
		return sector;
	}

	public int CreateDisk(string imagePath, uint sector)
	{
		int flag = _emulator.CreateDisk(imagePath, sector);
		if (flag < 0)
			_outputView.Append($"Failed to create disk (sector is occupied), Code Error {flag}", LogLevel.Error);
		return flag;
	}


	public bool RemoveDevice(int id)
	{
		bool removed = _emulator.RemoveDevice(id);
		if (removed)
			DeviceListChanged?.Invoke(_emulator.AllDevices);
		return removed;
	}

	public bool UpdateDeviceBios(int id, byte[] bios)
	{
		var device = _emulator.GetDevice(id);
		if (device == null) return false;

		device.UpdateBios(bios);
		DeviceListChanged?.Invoke(_emulator.AllDevices);
		return true;
	}
	public void WriteBootableProgram(string imagePath, byte[] program)
	{
		int sectorCount = Math.Max(1, (program.Length + 8 + DiskDevice.SectorSize - 1) / DiskDevice.SectorSize);

		using (var fs = File.Create(imagePath))
		{
			fs.SetLength(DiskDevice.SectorSize * sectorCount);
		}

		using var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Write);
		Span<byte> lengthBytes = stackalloc byte[8];
		BitConverter.TryWriteBytes(lengthBytes, (ulong)program.Length);
		stream.Write(lengthBytes);
		stream.Write(program);
	}
	public int CreateDisk(string imagePath, int sectorCount, uint sector)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sectorCount);

		long diskSize = DiskDevice.SectorSize * sectorCount;

		// Создаём файл нужного размера (можно перезаписать существующий)
		using (var fs = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
		{
			fs.SetLength(diskSize);
		}

		return _emulator.CreateDisk(imagePath, sector);
	}

	public int CreateDisk(string imagePath, int sectorCount)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sectorCount);

		long size = DiskDevice.SectorSize * sectorCount;
		using var fs = File.Create(imagePath);
		fs.SetLength(size);

		return _emulator.CreateDisk(imagePath);
	}

	// Получение обёртки диска
	public DiskData? GetDiskData(uint sector)
	{
		var disk = _emulator.GetDisk(sector);
		if (disk == null)
			return null;
		// Нужно знать путь к файлу образа – либо хранить его в DiskDevice, либо передавать при создании
		// Можно добавить в DiskDevice свойство ImagePath
		string imagePath = disk.ImagePath; // предположим, мы добавили такое свойство
		return new DiskData(disk, sector, imagePath, this);
	}

	// Удаление диска
	public bool RemoveDisk(uint sector)
	{
		bool success = _emulator.RemoveDisk(sector);
		if (success)
			_outputView.Append($"Диск в секторе {sector} удалён", LogLevel.Log);
		return success;
	}

	// Список всех дисков (секторов)
	public IEnumerable<int> ListDisksSectors()
	{
		return _emulator.GetDiskSectors();
	}

	public IEnumerable<DiskInfo> GetAllDiskInfo()
	{
		return _emulator.GetAllDiskData();
	}

	public void Reset()
	{
		_emulator.Reset();
	}

}
````

## File: VMApplication/Project/ProjectBuilder.cs
````csharp
using Compiller.ASM;
using Compiller.C;
using Compiller.C.CodeGenerator;
using Compiller.C.Optimizators;
using Kernel.Common;

namespace VMApplication.Project;

public static class ProjectBuilder
{
	private static readonly string[] DefaultAsmExtensions = [".soe", ".asm"];
	private static readonly string[] DefaultCExtensions = [".mic", ".c"];

	public const int BaseAdressProgramm = 0x0;

	internal static (byte[] ByteCode, OutPutOptimizeText ResLog) Build(List<SourceFile> files, IProjectFilesConfig path, ulong baseAddress, bool optimize)
	{
		var assembler = new Assembler(baseAddress: baseAddress);
		var asmParser = new AssemblerParser();

		var cAsts = new List<ProgramNode>();
		var includes = new List<string>();

		// 1. Разбираем все C‑файлы, собираем их AST и списки include
		foreach (var file in files)
		{
			if (file.Language == SourceLanguage.C)
			{
				var lexer = new Lexer(file.Content);
				var tokens = lexer.Tokenize();
				var parser = new Parser(tokens);
				var ast = parser.Parse();
				cAsts.Add(ast);
				includes.AddRange(ast.Includes);
			}
		}

		// 2. Объединяем C-функции и глобальные переменные в один AST
		var combinedAst = new ProgramNode();
		foreach (var ast in cAsts)
		{
			combinedAst.Functions.AddRange(ast.Functions);
			combinedAst.Globals.AddRange(ast.Globals);
			combinedAst.Structs.AddRange(ast.Structs);
		}
		// Оптимизация AST перед кодогенерацией
		OutPutOptimizeText resLog = default;

		if (optimize)
			resLog = AstOptimizer.Optimize(combinedAst);

		var allStructDecls = new List<StructDeclNode>();
		foreach (var ast in cAsts)
			allStructDecls.AddRange(ast.Structs);

		var structLayouts = StructLayout.Resolve(allStructDecls);

		// Перемещаем main в начало
		var mainFunc = combinedAst.Functions.FirstOrDefault(f => f.Name == "main");
		if (mainFunc != null)
		{
			combinedAst.Functions.Remove(mainFunc);
			combinedAst.Functions.Insert(0, mainFunc);
		}

		// 5. Теперь, когда все asm-метки известны, вставляем стартовый код
		if (combinedAst.Functions.Any(f => f.Name == "main"))
		{
			assembler.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), "func_main");
		}

		foreach (string inc in includes)
		{
			string localPath = Path.Combine(path.ProjectPath, inc);
			string sharedPath = Path.Combine(path.IncludePath, inc);

			string? chosenPath = null;
			if (File.Exists(localPath))
				chosenPath = localPath;
			else if (File.Exists(sharedPath))
				chosenPath = sharedPath;

			if (chosenPath == null)
				throw new Exception($"Included file not found: '{inc}'. Searched in project folder and in '{path.IncludePath}'.");

			string asmCode = File.ReadAllText(chosenPath);
			asmParser.Assemble(asmCode, assembler);
		}

		// 4. Добавляем обычные ассемблерные файлы проекта (если есть)
		foreach (var file in files)
		{
			if (file.Language == SourceLanguage.Asm)
			{
				asmParser.Assemble(file.Content, assembler);
			}
		}

		// 6. Генерируем код всех C‑функций (определит метку func_main)
		var funcGen = new FunctionGenerator(assembler, structLayouts);
		funcGen.Generate(combinedAst);

		// 7. Один завершающий HALT
		assembler.EmitInstruction(InstructionEncoder.EncodeEND());

		return (assembler.Build(), resLog);
	}

	internal static (byte[] ByteCode, OutPutOptimizeText ResLog) BuildProject(IFileService fileService, IEditorService editorService, IProjectFilesConfig paths, ulong baseAddress, bool optimize)
	{
		var files = new List<SourceFile>();

		foreach (string fileName in fileService.GetSourceFiles())
		{
			// Пытаемся получить текст из открытой вкладки
			string source = editorService.GetText(fileName) ?? fileService.ReadFile(fileName);

			SourceLanguage lang = FindLang(paths, fileName);
			files.Add(new SourceFile(fileName, source, lang));
		}

		// Вызываем существующий метод Build с базовым адресом (можно параметризовать)
		return Build(files, paths, baseAddress: baseAddress, optimize);
	}

	private static bool Has(string[] extens, string name)
	{
		foreach (var ext in extens)
			if (name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
				return true;
		return false;
	}

	private static bool IsAsm(IProjectFilesConfig config, string name)
	{
		var extens = config.ExtensionsAsm;
		extens ??= DefaultAsmExtensions;
		return Has(extens, name);
	}

	private static bool IsMiniC(IProjectFilesConfig config, string name)
	{
		var extens = config.ExtensionsMiniC;
		extens ??= DefaultCExtensions;
		return Has(extens, name);
	}

	private static SourceLanguage FindLang(IProjectFilesConfig config, string name)
	{
		if (IsAsm(config, name)) return SourceLanguage.Asm;
		else if (IsMiniC(config, name)) return SourceLanguage.C;
		else return SourceLanguage.None;
	}
}
````

## File: ASM gen/App.xaml.cs
````csharp
using System.IO;
using System.Reflection;
using System.Windows;

namespace ASM_gen
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
		{
			// Имя сборки, которую ищет среда
			string assemblyName = new AssemblyName(args.Name).Name + ".dll";

			// Путь к нашей кастомной папке (используем AppPaths)
			string probePath = Path.Combine(AppPaths.CurrentTemplatesPath, assemblyName);

			if (File.Exists(probePath))
			{
				return Assembly.LoadFrom(probePath);
			}

			return null;
		}
	}
}
````

## File: ASM gen/ASM gen.csproj
````
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
	<OutputType>Exe</OutputType>
	<TargetFramework>net10.0-windows</TargetFramework>
	<RootNamespace>ASM_gen</RootNamespace>
	<Nullable>enable</Nullable>
	<ImplicitUsings>enable</ImplicitUsings>
	<UseWPF>true</UseWPF>
	<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup>
	<PackageReference Include="AvalonEdit" Version="6.3.1.120" />
	<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.4078.44" />
	<PackageReference Include="Serilog.Sinks.Async" Version="2.1.0" />
	<PackageReference Include="Serilog.Sinks.File" Version="7.0.0" />
  </ItemGroup>

  <ItemGroup>
	<ProjectReference Include="..\VMApplication\VMApplication.csproj" />
  </ItemGroup>

  <ItemGroup>
	<None Update="Tests.md">
	  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
	</None>
  </ItemGroup>

	<PropertyGroup>
		<!-- Включаем перемещение сборок в подпапку -->
		<IncludeNativeLibrariesInSubdirectory>true</IncludeNativeLibrariesInSubdirectory>
		<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
		<EnableDefaultApplicationDefinition>false</EnableDefaultApplicationDefinition>

		<!-- Задаем имя папки, куда уйдут все DLL (в данном случае "bin") -->
		<AssemblyNameSubdirectory>bin</AssemblyNameSubdirectory>
	</PropertyGroup>
</Project>
````

## File: Compiller/ASM/AssemblerParser.cs
````csharp
using Kernel.Common;
using System.Collections.Frozen;

namespace Compiller.ASM;

/// <summary>
/// Парсер ассемблерного текста, преобразующий его в байт-код.
/// Поддерживаемые инструкции:
///   NOP, END, RET,
///   MOV, ADD, SUB, AND, OR, XOR (два регистра),
///   INC, DEC, NOT, PUSH, POP (один регистр),
///   LDI (регистр, константа),
///   LOAD.S8|.S16|.S32|.S64 (регистр, адрес),
///   STORE.S8|.S16|.S32|.S64 (регистр, адрес),
///   JMP, JZ, JNZ, JG, JL, CALL (метка или абсолютный адрес).
/// </summary>
public class AssemblerParser
{
	private static readonly Dictionary<string, RegType> _regMapLex = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "rZ", RegType.rZ },
		{ "r0", RegType.r0 },
		{ "r1", RegType.r1 },
		{ "r2", RegType.r2 },
		{ "r3", RegType.r3 },
		{ "r4", RegType.r4 },
		{ "r5", RegType.r5 },
		{ "r6", RegType.r6 },
		{ "r7", RegType.r7 },
		{ "r8", RegType.r8 },
		{ "r9", RegType.r9 },
		{ "r10", RegType.r10 },
		{ "r11", RegType.r11 },
		{ "r12", RegType.r12 },
		{ "r13", RegType.r13 },
		{ "r14", RegType.r14 },
		{ "r15", RegType.r15 },
		{ "r16", RegType.r16 },
		{ "r17", RegType.r17 },
		{ "r18", RegType.r18 },
		{ "r19", RegType.r19 },
		{ "r20", RegType.r20 },
		{ "r21", RegType.r21 },
		{ "rTB", RegType.rTB },
		{ "rCD", RegType.rCD },
		{ "rFL", RegType.rFL },
		{ "rLP", RegType.rLP },
		{ "rCL", RegType.rCL },
		{ "rRT", RegType.rRT },
		{ "rSP", RegType.rSP },
		{ "rHP", RegType.rHP },
		{ "rIP", RegType.rIP }
	};


	private static readonly Dictionary<string, OpCode> _opMapLex = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "NOP", OpCode.NOP },
		{ "END", OpCode.END },
		{ "RET", OpCode.RET },
		{ "PRINT", OpCode.PRINT },
		{ "MOV", OpCode.MOV },
		{ "ADD", OpCode.ADD },
		{ "SUB", OpCode.SUB },
		{ "AND", OpCode.AND },
		{ "OR", OpCode.OR },
		{ "XOR", OpCode.XOR },
		{ "INC", OpCode.INC },
		{ "DEC", OpCode.DEC },
		{ "NOT", OpCode.NOT },
		{ "PUSH", OpCode.PUSH },
		{ "POP", OpCode.POP },
		{ "LDI", OpCode.LDI },
		{ "LOAD", OpCode.LOAD },
		{ "STORE", OpCode.STORE },
		{ "JMP", OpCode.JMP },
		{ "JZ", OpCode.JZ },
		{ "JNZ", OpCode.JNZ },
		{ "JG", OpCode.JG },
		{ "JL", OpCode.JL },
		{ "CALL", OpCode.CALL },
		{ "LOAD_IND", OpCode.LOAD_IND },
		{ "STORE_IND", OpCode.STORE_IND },
		{ "PRINT_INT", OpCode.PRINT_INT },
		{ "ALLOC", OpCode.ALLOC },
		{ "IN", OpCode.IN },
		{ "OUT", OpCode.OUT },
		{ "INT", OpCode.INT },
		{ "IRET", OpCode.IRET },
		{ "SHR", OpCode.SHR },
		{ "MULT_INT", OpCode.MULT_INT },
		{ "DIV", OpCode.DIV },
		{ "HALT", OpCode.HALT },
		{ "WAKE", OpCode.WAKE},
		{ "WAKE_INT", OpCode.WAKE_INT}
	};

	private Assembler _asm = null!;
	private readonly FrozenDictionary<string, RegType> _regMap = _regMapLex.ToFrozenDictionary();
	private readonly FrozenDictionary<string, OpCode> _opMap = _opMapLex.ToFrozenDictionary();

	public byte[] Assemble(string code, ulong baseAddress = 0)
	{
		var asm = new Assembler(baseAddress);
		Assemble(code, asm);
		return asm.Build();
	}

	public void Assemble(string code, Assembler asm)
	{
		_asm = asm;
		ProcessLines(code);
	}

	private void ProcessLines(string code)
	{
		var lines = code.Split(['\n', ';'], StringSplitOptions.RemoveEmptyEntries);
		foreach (var rawLine in lines)
		{
			string line = rawLine.Trim();
			if (string.IsNullOrEmpty(line)) continue;

			// Удаление комментариев
			int commentIdx = line.IndexOf(';');
			if (commentIdx < 0) commentIdx = line.IndexOf("//");
			if (commentIdx >= 0)
				line = line[..commentIdx].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			// Обработка меток
			if (line.EndsWith(':'))
			{
				string label = line[..^1].Trim();
				_asm.MarkLabel(label);
				continue;
			}
			if (line.Contains(':'))
			{
				string[] parts = line.Split([':'], 2);
				string label = parts[0].Trim();
				string rest = parts[1].Trim();
				_asm.MarkLabel(label);
				if (!string.IsNullOrEmpty(rest))
					ParseInstruction(rest);
				continue;
			}

			ParseInstruction(line);
		}
	}
	private void ParseInstruction(string instruction)
	{
		var tokens = instruction.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) return;

		string mnemonic = tokens[0].ToUpperInvariant();
		OpCodeSize size = OpCodeSize.S64;
		string baseMnemonic = mnemonic;

		// Обработка суффиксов размера для LOAD/STORE
		if (mnemonic.StartsWith("LOAD."))
		{
			baseMnemonic = mnemonic[..4]; // Выделяем "LOAD" как срез (0 аллокаций!)
			size = ParseSize(mnemonic[5..]); // Передаем в парсер всё, что после точки
		}
		else if (mnemonic.StartsWith("STORE."))
		{
			baseMnemonic = mnemonic[..5]; // "STORE"
			size = ParseSize(mnemonic[6..]);
		}
		else if (mnemonic.StartsWith("LOAD_IND."))
		{
			baseMnemonic = mnemonic[..8]; // "LOAD_IND"
			size = ParseSize(mnemonic[9..]);
		}
		else if (mnemonic.StartsWith("STORE_IND."))
		{
			baseMnemonic = mnemonic[..9]; // "STORE_IND"
			size = ParseSize(mnemonic[10..]); // Точка на 9-й позиции, размер начинается с 10
		}

		if (!_opMap.TryGetValue(baseMnemonic, out OpCode opCode))
			throw new Exception($"Неизвестная инструкция: {mnemonic}");

		switch (baseMnemonic)
		{
			// Без операндов
			case "NOP": _asm.EmitInstruction(InstructionEncoder.EncodeNOP()); break;
			case "END": _asm.EmitInstruction(InstructionEncoder.EncodeEND()); break;
			case "RET": _asm.EmitInstruction(InstructionEncoder.EncodeRET()); break;
			case "HALT": _asm.EmitInstruction(InstructionEncoder.EncodeHALT()); break;
			case "WAKE": _asm.EmitInstruction(InstructionEncoder.EncodeWAKE()); break;
			// Формат R (два регистра)
			case "MOV":
			case "ADD":
			case "SUB":
			case "AND":
			case "OR":
			case "XOR":
			case "IN":
			case "OUT":
			case "SHR":
			case "DIV":
			case "MULT_INT":
				if (tokens.Length < 3)
					throw new Exception($"Инструкция {baseMnemonic} требует два регистра");
				RegType r1 = ParseReg(tokens[1]);
				RegType r2 = ParseReg(tokens[2]);
				_asm.EmitInstruction(InstructionEncoder.EncodeR((uint)opCode, (uint)r1, (uint)r2));
				break;

			// Формат U (один регистр)
			case "WAKE_INT":
			case "PRINT":
			case "INC":
			case "DEC":
			case "NOT":
			case "PUSH":
			case "POP":
			case "PRINT_INT":
				if (tokens.Length < 2)
					throw new Exception($"Инструкция {baseMnemonic} требует один регистр");
				RegType rU = ParseReg(tokens[1]);
				_asm.EmitInstruction(InstructionEncoder.EncodeU((uint)opCode, (uint)rU));
				break;

			// LDI: регистр, константа
			case "LDI":
				if (tokens.Length < 3)
					throw new Exception("LDI требует регистр и значение");
				RegType rLdi = ParseReg(tokens[1]);
				string operand = tokens[2];
				if (IsNumber(operand))
				{
					ulong value = ParseNumber(operand);
					_asm.EmitInstruction64(InstructionEncoder.EncodeLDI((uint)rLdi), value);
				}
				else
				{
					// Операнд — метка, откладываем разрешение адреса
					uint encodedldi = InstructionEncoder.EncodeLDI((uint)rLdi);
					_asm.EmitInstruction64(encodedldi, 0UL);  // временный 0
															  // Добавляем патч: (позиция в потоке, где записан 0, метка)
					long patchPos = _asm.GetStreamPosition() - 8; // нужно получить позицию в MemoryStream
					_asm.AddPatch(patchPos, operand);
				}
				break;

			// LOAD/STORE: регистр, адрес (константа)
			case "LOAD":
			case "STORE":
				if (tokens.Length < 3)
					throw new Exception($"{baseMnemonic} требует регистр и адрес");
				RegType rMem = ParseReg(tokens[1]);
				ulong addr = ParseNumber(tokens[2]);
				uint encoded;
				if (baseMnemonic == "LOAD")
					encoded = InstructionEncoder.EncodeLOAD((uint)rMem, (uint)size);
				else
					encoded = InstructionEncoder.EncodeSTORE((uint)rMem, (uint)size);
				_asm.EmitInstruction64(encoded, addr);
				break;

			case "LOAD_IND":
			case "STORE_IND":
				if (tokens.Length < 3)
					throw new Exception($"{baseMnemonic} требует два регистра");
				RegType rInd1 = ParseReg(tokens[1]);
				RegType rInd2 = ParseReg(tokens[2]);
				// EncodeR теперь принимает размер (добавьте соответствующий метод в InstructionEncoder)
				_asm.EmitInstruction(InstructionEncoder.EncodeRS((uint)opCode, (uint)rInd1, (uint)rInd2, (uint)size));
				break;
			// Переходы и CALL: метка или абсолютный адрес
			case "JMP":
			case "JZ":
			case "JNZ":
			case "JG":
			case "JL":
			case "CALL":
				if (tokens.Length < 2)
					throw new Exception($"{baseMnemonic} требует целевой адрес");
				string target = tokens[1];
				uint jmpOpcode = InstructionEncoder.EncodeJ((uint)opCode);
				if (IsNumber(target))
				{
					ulong absTarget = ParseNumber(target);
					_asm.EmitJumpToAbsolute(jmpOpcode, absTarget);
				}
				else
				{
					_asm.EmitJump(jmpOpcode, target);
				}
				break;
			case "ALLOC":
				_asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.ALLOC, 0));
				break;

			case "INT":
				if (tokens.Length < 2) throw new Exception("INT требует регистр с номером прерывания");
				RegType rInt = ParseReg(tokens[1]);
				_asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.INT, (uint)rInt));
				break;
			case "IRET":
				_asm.EmitInstruction(InstructionEncoder.EncodeU((uint)OpCode.IRET, 0));
				break;

			default:
				throw new Exception($"Инструкция {baseMnemonic} не реализована в парсере");
		}
	}

	private RegType ParseReg(string s)
	{
		if (_regMap.TryGetValue(s, out RegType reg))
			return reg;
		throw new Exception($"Неизвестный регистр: {s}");
	}

	private static ulong ParseNumber(string s)
	{
		s = s.Trim();
		if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			return Convert.ToUInt64(s[2..], 16);
		return ulong.Parse(s);
	}

	private static bool IsNumber(string s)
	{
		s = s.Trim();
		if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			return ulong.TryParse(s.AsSpan(2), System.Globalization.NumberStyles.HexNumber, null, out _);
		return ulong.TryParse(s, out _);
	}

	private static OpCodeSize ParseSize(string s) => s.ToUpperInvariant() switch
	{
		"S8" => OpCodeSize.S8,
		"S16" => OpCodeSize.S16,
		"S32" => OpCodeSize.S32,
		"S64" => OpCodeSize.S64,
		_ => throw new Exception($"Некорректный размер данных: {s}")
	};
}
````

## File: Compiller/C/CodeGenerator/CodeGenUtils.cs
````csharp
using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// CodeGenUtils – статические утилиты
// ============================================================

// ============================================================
// CodeGenUtils – статические утилиты
// ============================================================
public static class CodeGenUtils
{
	public const uint TMP_REG = (uint)RegType.r2;

	public static OpCodeSize GetSizeForType(string? type) => type switch
	{
		"byte" or "char" => OpCodeSize.S8,
		"ushort" => OpCodeSize.S16,
		"ulong" => OpCodeSize.S64,
		"int" => OpCodeSize.S32,
		_ => throw new Exception($"Unknown type '{type}' for memory size")
	};

	public static int GetSizeInBytes(OpCodeSize size) => size switch
	{
		OpCodeSize.S8 => 1,
		OpCodeSize.S16 => 2,
		OpCodeSize.S32 => 4,
		OpCodeSize.S64 => 8,
		_ => throw new Exception("Unknown size")
	};

	public static void EmitMultiplyByConstant(Assembler asm, uint reg, int multiplier)
	{
		for (int i = 1; i < multiplier; i *= 2)
		{
			asm.EmitInstruction(InstructionEncoder.EncodeR((uint)OpCode.ADD, reg, reg));
		}
	}

	public static bool IsComparisonOperator(string op) => op is "==" or "!=" or "<" or ">" or "<=" or ">=";

	public static int GetAlignment(string type) => type switch
	{
		"byte" or "char" => 1,
		"ushort" => 2,
		"int" => 4,
		"ulong" => 8,
		_ => 8  // структуры/указатели выравниваются на 8
	};

	public static bool IsStructType(string type, Dictionary<string, StructLayout> structTable)
		=> structTable.ContainsKey(type);

	public static StructLayout GetStructLayout(string type, Dictionary<string, StructLayout> structTable)
		=> structTable.TryGetValue(type, out var layout) ? layout : throw new Exception($"Unknown struct type: {type}");

	public static bool IsPrimitiveType(string type) => type switch
	{
		"int" or "char" or "void" or "byte" or "ushort" or "ulong" => true,
		_ => false
	};

	public static int GetTypeSize(string type, Dictionary<string, StructLayout> structTable)
	{
		if (structTable.TryGetValue(type, out var layout))
			return layout.Size;
		return GetSizeInBytes(GetSizeForType(type));
	}

	public static int GetAlignment(string type, Dictionary<string, StructLayout> structTable)
	{
		if (structTable.ContainsKey(type)) // структура
			return 8; // наибольшее выравнивание, можно брать максимальное из полей, но 8 ок
		return type switch
		{
			"byte" or "char" => 1,
			"ushort" => 2,
			"int" => 4,
			"ulong" => 8,
			_ => 8
		};
	}
}
````

## File: Compiller/C/CodeGenerator/FunctionContext.cs
````csharp
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// FunctionContext – контекст текущей функции
// ============================================================
public class FunctionContext
{
	public Dictionary<string, VarLocation> VarMap { get; private set; } = null!;
	public int TotalLocalSize { get; private set; }
	public string FunctionName { get; private set; } = null!;
	public string EpilogueLabel { get; private set; } = null!;
	public List<RegType> UsedRegisters { get; private set; } = [];


	public static FunctionContext Create(FunctionNode func, Dictionary<string, VariableNode> localVarNodes, Dictionary<string, StructLayout> structTable)
	{
		var ctx = new FunctionContext
		{
			FunctionName = func.Name,
			EpilogueLabel = $"__epilogue_{func.Name}"
		};

		// Собрать все переменные
		var allVars = new List<(string name, string type, bool isArray, int arraySize, bool isPointer, string? pointedType)>();
		foreach (var param in func.Parameters)
			allVars.Add((param.Name, param.Type, false, 0, param.IsPointer, param.PointedType));

		foreach (var kv in localVarNodes)
		{
			var varDecl = kv.Value;
			if (!allVars.Any(v => v.name == kv.Key))
				allVars.Add((kv.Key, varDecl.Type, varDecl.IsArray, varDecl.ArraySize, varDecl.IsPointer, varDecl.PointedType));
		}

		// Распределение регистров и стека
		var varMap = new Dictionary<string, VarLocation>();
		int nextReg = 4;
		int stackOffset = 0;

		foreach (var (name, type, isArray, arraySize, isPointer, pointedType) in allVars)
		{
			StructLayout layout = null!;
			bool isStruct = !isPointer && structTable.TryGetValue(type, out layout!);
			int varSize;
			OpCodeSize size = OpCodeSize.S64;

			if (isPointer)
			{
				varSize = 8;
				size = OpCodeSize.S64;
			}
			else if (isStruct)
			{
				varSize = layout.Size;
			}
			else
			{
				// Обычные типы (int, char, …)
				size = CodeGenUtils.GetSizeForType(type);
				if (isArray)
				{
					varSize = CodeGenUtils.GetSizeInBytes(size) * arraySize;
					varSize = (varSize + 7) & ~7;   // выравнивание
				}
				else
				{
					varSize = 8; // скалярное значение на стеке/регистре занимает 8 байт (выравнивание)
				}
			}

			int myOffset = stackOffset;
			stackOffset += varSize;

			if (isArray && structTable.TryGetValue(type, out StructLayout? layoutArr))
			{
				varSize = layoutArr.Size * arraySize;
				varSize = (varSize + 7) & ~7;
			}

			// Выделение регистра или стека
			if (nextReg <= 21 && !isArray && !isStruct)
			{
				// Только для простых типов (не массивы и не структуры)
				varMap[name] = new VarLocation
				{
					IsRegister = true,
					Register = (RegType)nextReg,
					TypeSize = size,
					IsArray = false,
					ArraySize = 0,
					IsPointer = isPointer,
					PointedType = pointedType
				};
				nextReg++;
			}
			else if (isStruct)
			{
				varMap[name] = new VarLocation
				{
					IsRegister = false,
					StackOffset = myOffset,
					TypeSize = OpCodeSize.S64,
					StructTypeName = type,
					IsArray = false,
					IsPointer = false
				};
			}
			else
			{
				varMap[name] = new VarLocation
				{
					IsRegister = false,
					StackOffset = myOffset,
					TypeSize = isPointer ? OpCodeSize.S64 : size,
					StructTypeName = null,
					IsArray = isArray,
					ArraySize = arraySize,
					IsPointer = isPointer,
					PointedType = pointedType
				};
			}
		}
		ctx.VarMap = varMap;
		ctx.TotalLocalSize = stackOffset;
		ctx.UsedRegisters = [.. varMap.Values.Where(v => v.IsRegister).Select(v => v.Register)];
		return ctx;
	}
}
````

## File: Compiller/C/CodeGenerator/FunctionGenerator.cs
````csharp
using Compiller.ASM;
using Kernel.Common;

namespace Compiller.C.CodeGenerator;

// ============================================================
// FunctionGenerator – генерация программы и функций
// ============================================================
public class FunctionGenerator(Assembler asm, Dictionary<string, StructLayout> structTable)
{
	private readonly Assembler _asm = asm;
	private GlobalMemoryManager _globalMem = null!;
	private readonly Dictionary<string, FunctionNode> _functionTable = [];
	private readonly Dictionary<string, StructLayout> _structTable = structTable;
	private int _labelCounter;

	public void Generate(ProgramNode program)
	{
		_globalMem = new GlobalMemoryManager(_structTable);

		//foreach (var s in program.Structs)
		//{
		//    _structTable[s.Name] = new(s.Name, s.Fields, _structTable);
		//}

		foreach (var global in program.Globals)
		{
			_globalMem.Allocate(global.Name, global.Type, global.IsArray, global.IsPointer, global.PointedType, global.ArraySize);
		}


		_functionTable.Clear();
		foreach (var func in program.Functions)
			_functionTable[func.Name] = func;

		foreach (var func in program.Functions)
			_globalMem.AllocatePseudoGlobals(func);

		GenerateGlobalInit(program);

		foreach (var func in program.Functions)
		{
			if (func.IsExternal) continue;
			GenerateFunction(func);
		}
	}

	private void GenerateGlobalInit(ProgramNode program)
	{
		// Временный контекст для вычисления глобальных инициализаторов
		var dummyCtx = new FunctionContext(); // не используется для varMap
		var getLabel = GetLabel;
		var exprGen = new ExpressionGenerator(_asm, _globalMem, dummyCtx, _functionTable, getLabel, _structTable);

		foreach (var global in program.Globals)
		{
			if (global.Initializer != null)
			{
				// Пропускаем структуры – их инициализация пока не поддерживается
				if (CodeGenUtils.IsStructType(global.Type, _structTable))
					continue;

				exprGen.GenerateExpression(global.Initializer);
				if (_globalMem.TryGetAddress(global.Name, out var addr))
				{
					var gInfo = _globalMem.GetInfo(global.Name)!.Value;
					var opSize = global.IsPointer ? OpCodeSize.S64 : CodeGenUtils.GetSizeForType(global.Type);
					_asm.EmitInstruction64(InstructionEncoder.EncodeSTORE((uint)RegType.r0, opSize.Uint), addr);
				}
			}
		}
	}

	private void GenerateFunction(FunctionNode func)
	{
		_asm.MarkLabel($"func_{func.Name}");

		// Сбор локальных переменных
		var localVarNodes = new Dictionary<string, VariableNode>();
		CollectLocalVars(func.Body, localVarNodes);

		// Выделение псевдо‑глобальных адресов для локальных переменных
		_globalMem.AllocateLocalGlobals(func.Body, func.Name);

		// Создание контекста функции
		var funcCtx = FunctionContext.Create(func, localVarNodes, _structTable);

		// Генераторы для тела функции
		var getLabel = GetLabel;
		var exprGen = new ExpressionGenerator(_asm, _globalMem, funcCtx, _functionTable, getLabel, _structTable);
		var stmtGen = new StatementGenerator(_asm, exprGen, funcCtx, getLabel, _globalMem, _structTable);

		bool isMain = func.Name == "main";

		// Пролог
		if (!isMain)
		{
			foreach (var reg in funcCtx.UsedRegisters.OrderBy(r => (int)r))
				_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.PUSH.Uint, (uint)reg));

			foreach (var param in func.Parameters)
			{
				if (!funcCtx.VarMap.TryGetValue(param.Name, out var loc) || !loc.IsRegister)
					throw new Exception($"Parameter '{param.Name}' not allocated to a register");
				string globalName = $"__param_{func.Name}_{param.Name}";
				var addr = _globalMem.GetInfo(globalName)!.Value.Address;
				OpCodeSize size;
				if (param.IsPointer)
					size = OpCodeSize.S64;
				else if (_structTable.ContainsKey(param.Type))
					size = OpCodeSize.S64;   // значение-структура пока не передаётся в регистр, но на всякий случай
				else
					size = CodeGenUtils.GetSizeForType(param.Type);
				_asm.EmitInstruction64(InstructionEncoder.EncodeLOAD((uint)loc.Register, size.Uint), addr);
			}

			if (funcCtx.TotalLocalSize > 0)
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
			}
		}
		else
		{
			if (funcCtx.TotalLocalSize > 0)
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.SUB.Uint, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
			}
		}

		// Тело
		stmtGen.GenerateBlock(func.Body);

		// Эпилог
		if (isMain)
		{
			_asm.EmitInstruction(InstructionEncoder.EncodeEND());
		}
		else
		{
			_asm.MarkLabel(funcCtx.EpilogueLabel);
			if (funcCtx.TotalLocalSize > 0)
			{
				_asm.EmitInstruction64(InstructionEncoder.EncodeLDI(CodeGenUtils.TMP_REG), (ulong)funcCtx.TotalLocalSize);
				_asm.EmitInstruction(InstructionEncoder.EncodeR(OpCode.ADD.Uint, (uint)RegType.rSP, CodeGenUtils.TMP_REG));
			}
			for (int i = funcCtx.UsedRegisters.Count - 1; i >= 0; i--)
				_asm.EmitInstruction(InstructionEncoder.EncodeU(OpCode.POP.Uint, (uint)funcCtx.UsedRegisters[i]));
			_asm.EmitInstruction(InstructionEncoder.EncodeRET());
		}
	}

	private static void CollectLocalVars(BlockNode block, Dictionary<string, VariableNode> map)
	{
		foreach (var stmt in block.Statements)
		{
			switch (stmt)
			{
				case VariableNode varNode:
					map[varNode.Name] = varNode;
					break;
				case BlockNode nested:
					CollectLocalVars(nested, map);
					break;
				case IfNode ifn:
					CollectLocalVars(ifn.ThenBlock, map); if (ifn.ElseBlock != null) CollectLocalVars(ifn.ElseBlock, map); break;
				case WhileNode wh:
					CollectLocalVars(wh.Body, map);
					break;
				case ForNode fr:
					{
						if (fr.Init is VariableNode forVar)
							map[forVar.Name] = forVar;
						CollectLocalVars(fr.Body, map);
						break;
					}
			}
		}
	}

	private string GetLabel() => $"L{_labelCounter++}";
}
````

## File: Controllers/ProcessorSystem/ProcessorHelpers.cs
````csharp
using Kernel.Common;
using System.Collections.Immutable;
using static Kernel.ProcessorSystem.Processor;

namespace Kernel.ProcessorSystem;

public static class ProcessorHelpers
{
	public static bool TryContinueAfterStatus(ResultInstruction dat, ulong ip, Lock regLock)
	{
		if (dat.BiosStatus == BiosStatus.EndProgramm)
		{
			lock (regLock)
			{
				LoggerProvider.Info($"\n [INFO] Программа успешно завершила работу (HALT). Ip [{ip}]");
				return false;
			}
		}

		string errorMessage = dat.BiosStatus switch
		{
			BiosStatus.ReadViolation => $"Попытка чтения из защищенной области памяти, адрес [{dat.Adress}]",
			BiosStatus.AlignmentFault => $"Попытка прочесть целочисленные данные по невыравненной памяти, адрес [{dat.Adress}]",
			BiosStatus.SegmentationFault => $"Ошибка выхода за границы ОЗУ, по обращению, адрес [{dat.Adress}]",
			BiosStatus.NotImplementedOpCode => $"Неизвестный код операции OpCode [{dat.Adress}]",
			BiosStatus.NullDeviceInput => $"Попытка записать данные в отсутствующий девайс, адрес обращения [{dat.Adress}],\n проверьте таблицу секторов портов, формула: [Adress / AdressPerSector]",
			BiosStatus.NullDeviceOutput => $"Попытка прочесть данные из отсутсвующего девайса, адрес обращения [{dat.Adress}],\n проверьте таблицу секторов портов, формула: [Adress / AdressPerSector]",
			_ => $"НЕПРЕДВИДЕННАЯ ОШИБКА СИМУЛЯЦИИ адрес [{dat.Adress}]"
		};

		return OutputLog(dat.BiosStatus, regLock, errorMessage, ip);
	}


	public static LogLevel TypeNotification(BiosStatus status) => status switch
	{
		BiosStatus.Success => LogLevel.Log,
		BiosStatus.SegmentationFault => LogLevel.Error,
		BiosStatus.AlignmentFault => LogLevel.Error,
		BiosStatus.ReadViolation => LogLevel.Error,
		BiosStatus.EndProgramm => LogLevel.Log,
		BiosStatus.NotImplementedOpCode => LogLevel.Error,
		BiosStatus.NullDeviceInput => LogLevel.Warning,
		BiosStatus.NullDeviceOutput => LogLevel.Error,
		_ => LogLevel.Error
	};

	public static bool CanContinue(BiosStatus status) => status switch
	{
		BiosStatus.ReadViolation => false,
		BiosStatus.AlignmentFault => false,
		BiosStatus.SegmentationFault => false,
		BiosStatus.NotImplementedOpCode => false,
		BiosStatus.NullDeviceInput => true,
		BiosStatus.NullDeviceOutput => false,
		BiosStatus.EndProgramm => false,
		_ => false
	};

	public static bool OutputLog(BiosStatus status, Lock regLock, string text, ulong ip)
	{
		bool can = CanContinue(status);
		var logLevel = TypeNotification(status);
		return logLevel switch
		{
			LogLevel.Log => LogNotification(regLock, text, ip, can),
			LogLevel.Warning => WarningNotification(regLock, text, ip, can),
			LogLevel.Error => ErrorNotification(regLock, text, ip, can),
			_ => NoneNotification(regLock, text, can),
		};
	}
	public static bool ErrorNotification(Lock regLock, string errorMessage, ulong ip, bool canContinue)
	{
		lock (regLock)
		{
			LoggerProvider.Error($"\n [КРИТИЧЕСКАЯ ОШИБКА ПРОЦЕССОРА] {errorMessage}, Ip [{ip}]");
		}
		return canContinue;
	}

	public static bool WarningNotification(Lock regLock, string errorMessage, ulong ip, bool canContinue)
	{
		lock (regLock)
		{
			LoggerProvider.Warning($"\n [ПРЕДУПРЕЖДЕНИЕ РАБОТЫ ПРОГРАММЫ] {errorMessage}, Ip [{ip}]");
		}
		return canContinue;
	}

	public static bool LogNotification(Lock regLock, string errorMessage, ulong ip, bool canContinue)
	{
		lock (regLock)
		{
			LoggerProvider.Info($"\n [ВЫПОЛНЕНИЕ УСПЕШНО] {errorMessage}, Ip [{ip}]");
		}
		return canContinue;
	}

	public static bool NoneNotification(Lock regLock, string text, bool canContinue)
	{
		lock (regLock)
		{
			LoggerProvider.Info(text);
		}
		return canContinue;
	}
}
````

## File: ASM gen/Analizator/AnalizatorOnErrors.cs
````csharp
using Kernel.Common;
using VMApplication;
using VMApplication.Logger;
using VMApplication.Project;

namespace ASM_gen.Analizator;

public class AnalizatorOnErrors : IDisposable
{
	private const string _nameSystem = "Analizator On Errors";
	private readonly IEditorService _editorService;
	private readonly IOutputView _outputView;

	private bool _haveError;
	private CancellationTokenSource? _cts;
	private readonly TimeSpan _validationDelay = TimeSpan.FromMilliseconds(500);
	private readonly Lock _lock = new();

	public bool HaveError => _haveError;

	public AnalizatorOnErrors(IEditorService editorService, IOutputView outputView)
	{
		_editorService = editorService;
		_outputView = outputView;
		_editorService.TextChanged += OnTextChanged;
	}

	public void Dispose()
	{
		_editorService.TextChanged -= OnTextChanged;
		CancelPendingValidation();
		_cts?.Dispose();
		GC.SuppressFinalize(this);
	}

	private void OnTextChanged(object? sender, EventArgs e)
	{
		CancelPendingValidation();
		ScheduleValidation();
	}

	private void CancelPendingValidation()
	{
		lock (_lock)
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}
	}

	private void ScheduleValidation()
	{
		CancellationTokenSource cts = new();
		lock (_lock)
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = cts;
		}

		// Берём текст активной вкладки (вызывается из UI-потока, т.к. событие TextChanged в UI)
		string textToCheck = _editorService.GetCurrentText();

		Task.Delay(_validationDelay, cts.Token).ContinueWith(async _ =>
		{
			if (cts.Token.IsCancellationRequested) return;
			var errorLines = await Task.Run(() => CheckTextForErrors(textToCheck), cts.Token);

			// Применяем подсветку в UI-потоке
			if (!cts.Token.IsCancellationRequested)
			{
				_editorService.ClearHighlights();
				if (errorLines.Count > 0)
					_editorService.HighlightErrors(errorLines);
				_haveError = errorLines.Count > 0;
			}
		}, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
	}

	private static int ExtractLineFromException(Exception ex)
	{
		string msg = ex.Message;
		int atIndex = msg.IndexOf(" at ", StringComparison.Ordinal);
		if (atIndex != -1)
		{
			ReadOnlySpan<char> span = msg.AsSpan(atIndex + 4);
			int colon = span.IndexOf(':');
			if (colon != -1 && int.TryParse(span[..colon], out int line))
				return line;
		}
		return 0;
	}

	private List<int> CheckTextForErrors(string text)
	{
		if (text.StartsWith('\uFEFF'))
			text = text[1..];
		var errorLines = new List<int>();
		try
		{
			VMHostHelper.LaunchUnsafeParse(text);
		}
		catch (Exception ex) when (ex is not OperationCanceledException)
		{
			_outputView.Append($"[{_nameSystem}] {ex.Message}", LogLevel.Error);
			int line = ExtractLineFromException(ex);
			if (line > 0) errorLines.Add(line);
		}
		return errorLines;
	}
}
````

## File: ASM gen/Output/WpfLogger.cs
````csharp
using Kernel.Common;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using VMApplication.Logger;

namespace ASM_gen.Output;

public class WpfOutputView : IOutputView   // ILogger оставлен для совместимости с Kernel
{
	private readonly Dispatcher _dispatcher;
	private readonly RichTextBox _outputBox;
	private readonly Paragraph _paragraph;
	private static readonly ConcurrentDictionary<Color, SolidColorBrush> BrushCache = new();

	public WpfOutputView(RichTextBox outputBox)
	{
		_outputBox = outputBox ?? throw new ArgumentNullException(nameof(outputBox));
		_dispatcher = outputBox.Dispatcher;
		_paragraph = new Paragraph();

		_outputBox.Document.Blocks.Clear();
		_outputBox.Document.Blocks.Add(_paragraph);
	}

	public void Append(string message, LogLevel level = LogLevel.Log)
	{
		Color color = level switch
		{
			LogLevel.Log => Colors.WhiteSmoke,
			LogLevel.Warning => Colors.Yellow,
			LogLevel.Error => Colors.Red,
			_ => Colors.Gray
		};
		AppendMessage(message, color);
	}

	public void Clear()
	{
		if (_dispatcher.CheckAccess())
			ClearInternal();
		else
			_dispatcher.Invoke(ClearInternal);
	}

	private void AppendMessage(string message, Color color)
	{
		if (_dispatcher.CheckAccess())
			AppendInternal(message, color);
		else
			_dispatcher.BeginInvoke(new Action(() => AppendInternal(message, color)));
	}

	private void AppendInternal(string message, Color color)
	{
		var brush = BrushCache.GetOrAdd(color, c =>
		{
			var b = new SolidColorBrush(c);
			if (b.CanFreeze) b.Freeze();
			return b;
		});
		_paragraph.Inlines.Add(new Run(message) { Foreground = brush });
		_paragraph.Inlines.Add(new LineBreak());
		// Удаляем старые блоки, если нужно
		if (_outputBox.Document.Blocks.Count == 0)
			_outputBox.Document.Blocks.Add(_paragraph);
		_outputBox.ScrollToEnd();
	}

	private void ClearInternal()
	{
		_outputBox.Document.Blocks.Clear();
		_paragraph.Inlines.Clear();
		_outputBox.Document.Blocks.Add(_paragraph);
	}
}
````

## File: ASM gen/Program.cs
````csharp
using ASM_gen.ProjectManage.Managers.Static;
using ASM_gen.StartWindow;
using Serilog;
using System.IO;
using VMApplication.Project;

namespace ASM_gen;

internal static class Program
{
	[STAThread]
	public static void Main()
	{
		Log.Logger = new LoggerConfiguration().WriteTo.File("logs/app-log.txt").CreateLogger();

		try
		{
			DirManager.InitializeDirectories();
			Log.Information("Приложение запускается...");

			App app = new();

			MainWindow mainWindow = new();
			app.Run(mainWindow);
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Критическая ошибка при запуске приложения");
		}
		finally
		{
			Log.Information("Приложение завершило работу.");
			Log.CloseAndFlush();
		}
	}
}

public static class AppPaths
{
	public const string ExtensionProj = "*.vmproj";

	private class PathSystem(string projectPath, string include) : IProjectFilesConfig
	{
		public string ProjectPath => projectPath;
		public string IncludePath => include;

		public string[] ExtensionsAsm => [".asm", ".soe", ".vma"];

		public string[] ExtensionsMiniC => [".c", ".mic", ".cll"];
	}

	public static IProjectFilesConfig ProjectSystemPaths(string path) => new PathSystem(path, sharedIncludePath);
	/// <summary> Базовая директория приложения </summary>
	private static readonly string CurrentDir = AppDomain.CurrentDomain.BaseDirectory;

	private static readonly string currentBinPath = Path.Combine(CurrentDir, "bin");

	private static readonly string currentSysDataPath = Path.Combine(CurrentDir, "sysData");
	private static readonly string currentTemplatesPath = Path.Combine(CurrentSysDataPath, "templates");

	private static readonly string currentLocalDataPath = Path.Combine(CurrentDir, "localData");
	private static readonly string currentMetaDataPath = Path.Combine(CurrentLocalDataPath, "metaData");
	private static readonly string currentProjectsDataPath = Path.Combine(CurrentMetaDataPath, "pojectsData");

	private static readonly string currentUserProjectsPath = Path.Combine(CurrentDir, "userProjects");
	private static readonly string sharedIncludePath = Path.Combine(CurrentLocalDataPath, "include");

	public static string CurrentBinPath => currentBinPath;
	public static string CurrentSysDataPath => currentSysDataPath;
	public static string CurrentTemplatesPath => currentTemplatesPath;
	public static string CurrentLocalDataPath => currentLocalDataPath;
	public static string CurrentMetaDataPath => currentMetaDataPath;
	public static string CurrentProjectsDataPath => currentProjectsDataPath;
	public static string CurrentUserProjectsPath => currentUserProjectsPath;
	public static string SharedIncludePath => sharedIncludePath;
}
````

## File: Compiller/ASM/InstructionEncoder.cs
````csharp
using Kernel.Common;
using static Kernel.Common.InstructionDecoder;

namespace Compiller.ASM;

/// <summary>
/// Энкодер инструкций для процессора.
/// Формат инструкции (32 бита):
///   Bits 0-7:   OpCode
///   Bits 8-12:  Reg1 (целевой/основной регистр)
///   Bits 13-17: Reg2 (второй регистр-источник)
///   Bits 18-19: OpCodeSize (размер данных: 0=S8, 1=S16, 2=S32, 3=S64)
///   Bits 20-31: Зарезервированы
/// </summary>
public static class InstructionEncoder
{
	extension(OpCode c)
	{
		public uint ToUint() => (uint)c;
		public uint Uint => (uint)c;
	}

	extension(OpCodeSize s)
	{
		public uint ToUint() => (uint)s;
		public uint Uint => (uint)s;
	}
	#region Методы кодирования

	/// <summary>
	/// Кодирует инструкцию формата R (регистр-регистр): ADD, SUB, MOV, AND, OR, XOR
	/// </summary>
	public static uint EncodeR(uint opcode, uint regDst, uint regSrc)
	{
		return opcode | (regDst << 8) | (regSrc << 13);
	}

	/// <summary>
	/// Кодирует инструкцию формата R (регистр-регистр): ADD, SUB, MOV, AND, OR, XOR
	/// </summary>
	public static uint EncodeRS(uint opcode, uint regDst, uint regSrc, uint sizeCode)
	{
		return opcode | (regDst << 8) | (regSrc << 13) | (sizeCode << 18);
	}
	/// <summary>
	/// Кодирует инструкцию формата I (регистр-константа/адрес): LDI, LOAD, STORE
	/// sizeCode - размер операнда (S8, S16, S32, S64)
	/// Для LDI: sizeCode определяет, сколько байт значащие
	/// Для LOAD/STORE: sizeCode определяет размер загружаемых/сохраняемых данных
	/// </summary>
	public static uint EncodeI(uint opcode, uint reg, uint sizeCode)
	{
		return opcode | (reg << 8) | (sizeCode << 18);
	}

	/// <summary>
	/// Кодирует инструкцию LDI (загрузка константы) с автоматическим S64
	/// </summary>
	public static uint EncodeLDI(uint reg)
	{
		return EncodeI(OpCode.LDI.Uint, reg, (uint)OpCodeSize.S64);
	}

	/// <summary>
	/// Кодирует инструкцию LOAD с указанием размера данных
	/// </summary>
	public static uint EncodeLOAD(uint regDst, uint sizeCode)
	{
		return EncodeI(OpCode.LOAD.Uint, regDst, sizeCode);
	}

	/// <summary>
	/// Кодирует инструкцию STORE с указанием размера данных
	/// </summary>
	public static uint EncodeSTORE(uint regSrc, uint sizeCode)
	{
		return EncodeI(OpCode.STORE.Uint, regSrc, sizeCode);
	}

	/// <summary>
	/// Кодирует инструкцию формата U (один регистр): INC, DEC, NOT, PUSH, POP
	/// </summary>
	public static uint EncodeU(uint opcode, uint reg)
	{
		return opcode | (reg << 8);
	}

	/// <summary>
	/// Кодирует инструкцию перехода (JMP, JZ, JNZ, JG, JL)
	/// Адрес всегда 64-битный (S64)
	/// </summary>
	public static uint EncodeJ(uint opcode)
	{
		return opcode | (OpCodeSize.S64.Uint << 18);
	}

	/// <summary>
	/// Кодирует инструкцию CALL (вызов подпрограммы)
	/// </summary>
	public static uint EncodeCALL() => EncodeJ(OpCode.CALL.Uint);

	/// <summary>
	/// Кодирует инструкцию RET (возврат из подпрограммы)
	/// </summary>
	public static uint EncodeRET() => OpCode.RET.Uint;

	/// <summary>
	/// Кодирует инструкцию HALT (вызов сна)
	/// </summary>
	public static uint EncodeHALT() => OpCode.HALT.Uint;

	/// <summary>
	/// Кодирует инструкцию WAKE (возврат из сна)
	/// </summary>
	public static uint EncodeWAKE() => OpCode.WAKE.Uint;
	/// <summary>
	/// Кодирует END
	/// </summary>
	public static uint EncodeEND() => OpCode.END.Uint;

	/// <summary>
	/// Кодирует NOP
	/// </summary>
	public static uint EncodeNOP() => OpCode.NOP.Uint;

	#endregion

	#region Вспомогательные методы для работы с буфером

	/// <summary>
	/// Записывает инструкцию и выравнивает поток под 8 байт (для последующего 64-битного данного)
	/// </summary>
	public static void WriteInstruction(BinaryWriter writer, uint instruction)
	{
		writer.Write(instruction);
	}

	/// <summary>
	/// Выравнивает поток до границы 8 байт (для 64-битных операндов)
	/// </summary>
	public static void Align8(BinaryWriter writer)
	{
		long pos = writer.BaseStream.Position;
		long pad = ((pos + 7) & ~7) - pos;
		if (pad > 0)
			writer.Write(new byte[pad]);
	}

	/// <summary>
	/// Записывает инструкцию + 64-битный операнд (с выравниванием)
	/// </summary>
	public static void WriteInstructionWithData64(BinaryWriter writer, uint instruction, ulong data)
	{
		writer.Write(instruction);
		Align8(writer);
		writer.Write(data);
	}

	public static uint EncodeIN(uint regDst, uint regPort) => EncodeR((uint)OpCode.IN, regDst, regPort);

	public static uint EncodeOUT(uint regSrc, uint regPort) => EncodeR((uint)OpCode.OUT, regSrc, regPort);

	public static uint EncodeLOAD_IND(uint regDst, uint regAddr, uint sizeCode)
	{
		return EncodeRS(OpCode.LOAD_IND.Uint, regDst, regAddr, sizeCode);
	}

	public static uint EncodeSTORE_IND(uint regSrc, uint regAddr, uint sizeCode)
	{
		return EncodeRS(OpCode.STORE_IND.Uint, regSrc, regAddr, sizeCode);
	}

	public static uint EncodeSHR(uint regDst, uint regSrc) => EncodeR(OpCode.SHR.Uint, regDst, regSrc);
	public static uint EncodeMULT_INT(uint regDst, uint regSrc) => EncodeR(OpCode.MULT_INT.Uint, regDst, regSrc);
	#endregion

	#region Декодирование (для отладки)

	/// <summary>
	/// Декодирует инструкцию в читаемый вид (для отладки)
	/// </summary>
	public static string Decode(uint instruction)
	{
		OpCode opcode = GetOpCode(instruction);

		return opcode switch
		{
			OpCode.NOP => "NOP",
			OpCode.END => "END",
			OpCode.PRINT => $"PRINT {GetReg1(instruction).Name}",
			OpCode.HALT => "HALT",
			OpCode.WAKE => "WAKE",
			OpCode.WAKE_INT => $"WAKE_INT {GetReg1(instruction).Name}",

			OpCode.MOV => $"MOV {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.LOAD => $"LOAD.{GetDataSizeCode(instruction).SizeName} {GetReg1(instruction).Name}, [data64]",
			OpCode.STORE => $"STORE.{GetDataSizeCode(instruction).SizeName} [data64], {GetReg1(instruction).Name}",
			OpCode.LOAD_IND => $"LOAD_IND.{GetDataSizeCode(instruction).SizeName} {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.STORE_IND => $"STORE_IND.{GetDataSizeCode(instruction).SizeName} {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.LDI => $"LDI {GetReg1(instruction).Name}, data64",

			OpCode.ADD => $"ADD {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.SUB => $"SUB {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.INC => $"INC {GetReg1(instruction).Name}",
			OpCode.DEC => $"DEC {GetReg1(instruction).Name}",

			OpCode.AND => $"AND {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.OR => $"OR {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.XOR => $"XOR {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.NOT => $"NOT {GetReg1(instruction).Name}",

			OpCode.JMP => $"JMP data64",
			OpCode.JZ => $"JZ data64",
			OpCode.JNZ => $"JNZ data64",
			OpCode.JG => $"JG data64",
			OpCode.JL => $"JL data64",

			OpCode.PUSH => $"PUSH {GetReg1(instruction).Name}",
			OpCode.POP => $"POP {GetReg1(instruction).Name}",
			OpCode.CALL => $"CALL data64",
			OpCode.RET => $"RET",

			OpCode.IN => $"IN {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",
			OpCode.OUT => $"OUT {GetReg1(instruction).Name}, {GetReg2(instruction).Name}",

			OpCode.PRINT_INT => $"PRINT_INT {GetReg1(instruction).Name}",
			OpCode.ALLOC => $"ALLOC {GetReg1(instruction).Name}",
			OpCode.INT => $"INT {GetReg1(instruction).Name}",
			OpCode.IRET => "IRET",

			_ => $"UNKNOWN 0x{opcode:X2}"
		};
	}
	extension(OpCodeSize size)
	{
		private string SizeName => size switch
		{
			OpCodeSize.S8 => "S8",
			OpCodeSize.S16 => "S16",
			OpCodeSize.S32 => "S32",
			OpCodeSize.S64 => "S64",
			_ => "???"
		};
	}
	extension(RegType r)
	{
		private string Name => r switch
		{
			RegType.rZ => "rZ",
			RegType.r0 => "r0",
			RegType.r1 => "r1",
			RegType.r2 => "r2",
			RegType.r3 => "r3",
			RegType.r4 => "r4",
			RegType.r5 => "r5",
			RegType.r6 => "r6",
			RegType.r7 => "r7",
			RegType.r8 => "r8",
			RegType.r9 => "r9",
			RegType.r10 => "r10",
			RegType.r11 => "r11",
			RegType.r12 => "r12",
			RegType.r13 => "r13",
			RegType.r14 => "r14",
			RegType.r15 => "r15",
			RegType.r16 => "r16",
			RegType.r17 => "r17",
			RegType.r18 => "r18",
			RegType.r19 => "r19",
			RegType.r20 => "r20",
			RegType.r21 => "r21",
			RegType.rTB => "rTB",
			RegType.rCD => "rCD",
			RegType.rFL => "rFL",
			RegType.rLP => "rLP",
			RegType.rCL => "rCL",
			RegType.rRT => "rRT",
			RegType.rSP => "rSP",
			RegType.rHP => "rHP",
			RegType.rIP => "rIP",
			_ => $"r?"
		};
	}

	#endregion

}
````

## File: ASM gen/DeviceManagerWindow.xaml.cs
````csharp
using ASM_gen.Information_Window;
using ASM_gen.ViewModels;
using Kernel.Common;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VMApplication.Emulator;
using VMApplication.Logger;

namespace ASM_gen;

public partial class DeviceManagerWindow : Window
{
	private const string NameSystem = "Manager Device UI Component";
	private readonly VMEmulator _host;
	private readonly IOutputView _outputView;
	private InformationWindow? infoWindow;
	private DeviceView? _selectedDevice;
	private DeviceContext? _device;

	public ObservableCollection<DeviceView> Devices { get; set; } = [];

	public readonly Action<DeviceContext> CurrentDevice;
	public readonly Action<LaunchModeDevice> CurrenLaunchModel;
	public DeviceManagerWindow(IOutputView outputView, VMEmulator host, Action<DeviceContext> returned, Action<LaunchModeDevice> currenLaunchModel)
	{
		InitializeComponent();
		DeviceGrid.ItemsSource = Devices;

		Activated += UpdateTable!;
		_host = host;
		_outputView = outputView;

		CurrentDevice = returned;
		CurrenLaunchModel = currenLaunchModel;
	}

	public void SetDeviceData(DeviceContext device) => _device = device;
	private void UpdateTable(object sender, EventArgs e) => RefreshDeviceList();

	private void Window_Loaded(object sender, RoutedEventArgs e) => RefreshDeviceList();
	public void RefreshDeviceList()
	{
		var arr = _host.GetAllDevices().ToList();

		Devices.Clear();
		foreach (var device in arr)
		{
			Devices.Add(device);
		}
	}


	private void DeviceGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_selectedDevice = DeviceGrid.SelectedItem as DeviceView?;
	}

	private void ShowMemory_Click(object sender, RoutedEventArgs e)
	{
		var selected = _selectedDevice;
		if (selected == null) return;
		ReadOnlyMemory<byte> memory = selected.Value.Ram;
		infoWindow?.Close();
		infoWindow = null;
		infoWindow = new InformationWindow(memory)
		{
			Owner = this
		};
		infoWindow.Show();
	}

	private void LoadBin_Click(object sender, RoutedEventArgs e)
	{
		var selected = _selectedDevice;
		if (selected == null || _device == null)
		{
			_outputView.Append($" {NameSystem} Выберите устройство в списке.", LogLevel.Error);
			return;
		}

		var dialog = new OpenFileDialog
		{
			Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
			Title = "Выберите .bin файл программы"
		};

		if (dialog.ShowDialog() == true)
		{
			int deviceId = selected.Value.Id;
			byte[] program = System.IO.File.ReadAllBytes(dialog.FileName);
			LaunchModeDevice d = _device.LoadProgram(program);
			CurrentDevice.Invoke(_device);
			CurrenLaunchModel.Invoke(d);
			_outputView?.Append($" {NameSystem} Программа загружена в устройство {deviceId}.");
		}
	}

	private void ChangeSector_Click(object sender, RoutedEventArgs e)
	{
		if (!_selectedDevice.HasValue)
		{
			_outputView.Append($" {NameSystem}Выберите устройство в списке.", LogLevel.Error);
			return;
		}
		var device = _selectedDevice.Value;

		if (!uint.TryParse(NewSectorBox.Text, out uint newSector))
		{
			_outputView.Append($"{NameSystem} Введите корректный номер сектора.", LogLevel.Error);
			return;
		}

		bool success = _host.ChangeDeviceSector(device.Id, newSector);
		if (success)
		{
			_outputView.Append($" {NameSystem}Сектор изменён на {newSector}.");
			RefreshDeviceList();
		}
		else
		{
			_outputView.Append($" {NameSystem} Не удалось изменить сектор (возможно, занят).", LogLevel.Error);
		}

	}

	private void SetMainDevice_Click(object sender, RoutedEventArgs e)
	{
		if (!_selectedDevice.HasValue)
		{
			_outputView.Append($" {NameSystem} Сначала выберите устройство.", LogLevel.Error);
			return;
		}
		var device = _selectedDevice.Value;

		_host.SetMainDevice(device.Id);
		DeviceContext? d = _host.GetDeviceData(device.Id);
		_device = d;
		if (d == null)
		{
			_outputView.Append($" {NameSystem} Выбранное устройство не имеет технической логики в программе, возврат из API вернул null", LogLevel.Error);
			return;
		}

		CurrentDevice.Invoke(d);
		_outputView.Append($" {NameSystem} Устройство {device.Id} теперь основное.");
	}

	private void BtnCreateDevice_Click(object sender, RoutedEventArgs e)
	{
		RefreshDeviceList();
		CreateDevice();
		infoWindow?.Close();
		infoWindow = null;
	}

	public void CreateDevice()
	{
		var dialog = new CreateDeviceDialog
		{
			Owner = this
		};
		if (dialog.ShowDialog() == true)
		{
			DeviceCreationResult? res = dialog.ViewModel.Result;
			if (res != null)
			{
				int deviceId = _host.CreateDevice(res.Bios, res.RamSize, res.Sector,
												  res.DeviceName, res.ProcName,
												  res.RamName, res.PortBusName);
				if (deviceId != -1)
				{
					_outputView.Append($" {NameSystem} Устройство создано с ID: {deviceId}");
					RefreshDeviceList();
				}
				else
				{
					_outputView.Append($" {NameSystem} Не удалось создать устройство", LogLevel.Error);
				}
			}
		}
	}

	private void BtnUpdateGrid_Click(object sender, RoutedEventArgs e)
	{
		RefreshDeviceList();
	}

	private void UpdateBios_Click(object sender, RoutedEventArgs e)
	{
		if (!_selectedDevice.HasValue)
		{
			_outputView.Append("Выберите устройство в списке.", LogLevel.Error);
			return;
		}
		var device = _selectedDevice.Value;

		var dialog = new OpenFileDialog
		{
			Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*",
			Title = "Выберите новый BIOS"
		};
		if (dialog.ShowDialog() == true)
		{
			byte[] bios = System.IO.File.ReadAllBytes(dialog.FileName);
			bool success = _host.UpdateDeviceBios(device.Id, bios);
			_outputView.Append(success
				? $"BIOS устройства {device.Id} обновлён."
				: "Не удалось обновить BIOS.", success ? LogLevel.Log : LogLevel.Error);
			RefreshDeviceList();
		}
	}

	private void RemoveDevice_Click(object sender, RoutedEventArgs e)
	{
		if (!_selectedDevice.HasValue)
		{
			_outputView.Append("Выберите устройство в списке.", LogLevel.Error);
			return;
		}
		var device = _selectedDevice.Value;
		MessageBoxResult confirm = MessageBox.Show(
			$"Удалить устройство {device.Id}?",
			"Подтверждение удаления",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning);
		if (confirm != MessageBoxResult.Yes) return;

		infoWindow?.Close();
		infoWindow = null;

		bool success = _host.RemoveDevice(device.Id); // это 222 строчка кода
		if (success)
		{
			_outputView.Append($"Устройство {device.Id} удалено.");
			RefreshDeviceList();
		}
		else
		{
			_outputView.Append("Не удалось удалить устройство.", LogLevel.Error);
		}
	}

	private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
	{
		this.Close();
	}
}
````

## File: Compiller/Emulation/Emulator.cs
````csharp
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.LocalMemorySystem;
using Kernel.Utilites;
using static Kernel.Utilites.ManagerDevices;

namespace Compiller.Emulation;

/// <summary>
/// Эмулятор устройства: управляет загрузкой программ, запуском, пошаговым выполнением.
/// Теперь поддерживает несколько устройств через ManagerDevices.
/// </summary>
public class Emulator
{
	private readonly PortBus _portBus;
	private readonly ManagerDevices _manager;
	private readonly DiskManager _diskManager;

	private int _mainDeviceId = -1;

	public Emulator(SizePort totalPorts = SizePort.Size16KB, SizePortOnDevice portsPerDevice = SizePortOnDevice.Size16B)
	{
		_portBus = new PortBus(totalPorts, portsPerDevice, new NameDeviceToken("Emulator".AsSpan()), "PortBus");
		_manager = new ManagerDevices(_portBus);
		_diskManager = new DiskManager(_portBus);
	}

	public PortBus PortBus => _portBus;
	public Device? CurrentDevice => MainDevice;
	public int MainDeviceId => _mainDeviceId;
	public int CountDevice => _manager.Count;
	public DeviceInfo? GetDeviceInfo(int i) => _manager.GetDeviceInfo(i);
	public IEnumerable<DeviceInfo> AllDevices => _manager.GetAllDevices();
	public void SetMainDevice(int id) => _mainDeviceId = id;

	public int CreateDevice(
		byte[] biosFirmware,
		RamSize size,
		uint sector,
		string? name = null,
		string? nameProc = null,
		string? nameRam = null,
		string? namePortBus = null)
	{
		int id = _manager.CreateNewDevice(
			size,
			name,
			nameProc,
			nameRam,
			namePortBus,
			sector,
			biosFirmware
		);

		if (id != -1 && _mainDeviceId == -1)
			_mainDeviceId = id;

		return id;
	}

	public Device? GetDevice(int id) => _manager.GetDevice(id);

	public Device? MainDevice => _manager.GetDevice(_mainDeviceId);

	public void Step(bool debugMode) => MainDevice?.NextStepProcessor(debugMode);
	public void Step(int count, bool debugMode) => MainDevice?.NextStepProcessorCount(count, debugMode);

	public string? DumpRegisters() => MainDevice?.GetAllData();

	public string? DumpRegisters(int id) => _manager.GetDevice(id)?.GetAllData();


	public void Reset()
	{
		_manager.Clear();
		_mainDeviceId = -1;
		_diskManager.Clear();
	}

	public int CreateDisk(string imagePath)
	{
		if (!Path.Exists(imagePath)) return -2;
		return _diskManager.CreateDisk(imagePath);
	}

	public int CreateDisk(string imagePath, uint sector)
	{
		if (!Path.Exists(imagePath)) return -3;
		return _diskManager.CreateDisk(imagePath, sector);
	}

	public bool RemoveDevice(int id)
	{
		bool removed = _manager.RemoveDevice(id);
		if (removed && _mainDeviceId == id)
			_mainDeviceId = -1;
		return removed;
	}
	public bool RemoveDisk(uint sector) => _diskManager.RemoveDisk(sector);
	public DiskDevice? GetDisk(uint sector) => _diskManager.GetDisk(sector);
	public bool ChangeDeviceSector(int id, uint newSector) => _manager.ChangeSector(id, newSector);
	public IEnumerable<int> GetDiskSectors() => _diskManager.GetAllDisks();

	public IEnumerable<DiskInfo> GetAllDiskData()
	{
	   return  _diskManager.GetAllDiskInfo();
	}
}
````

## File: Controllers/BiosSystem/Device.cs
````csharp
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.ProcessorSystem;
using Kernel.RamSystem;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using ThreadingSystem;
using ThreadingSystem.ThreadControl;

namespace Kernel.BiosSystem;

public sealed class Device : IDisposable, IPortUse
{
	private readonly byte[] _ioMemory;
	private static readonly StringBuilder _sharedBuilder = new(1024);
	private readonly ManualResetEventSlim _wakeSignal = new(false);
	private readonly NameDeviceToken _nameDevice;
	private readonly MemoryBus _ram;
	private readonly Processor _processor;
	private readonly PortBus _portBus;
	private Thread? _simulationThread;
	private readonly Lock _consoleLock = new();
	private long stepCounter = 0;

	public Action<Action<string, LogLevel>>? ActionOnWake = null;
	public DateTime CreatedAt { get; } = DateTime.Now;
	public Span<byte> AsRamSpan(int start, int length) => _ram.AsSpan(start, length);
	public Span<byte> RamSpan => _ram.Span;
	public Memory<byte> AsRamMemory() => _ram.Memory;
	public Memory<byte> AsRamMemory(int start, int length) => _ram.AsMemory(start, length);
	public ReadOnlyMemory<byte> RamArray => _ram.ReadOnlyMemory;
	public bool IsRunning => _processor.IsRunning;
	public bool IsSleeping => _processor.IsSleeping;
	public bool HaveBios => _ram.HaveBios;
	public ulong MaxRamSize => _ram.RamSize;
	public long? StepCount => _processor.IsRunning ? null : stepCounter;
	public ulong CurrentIP => _processor.GetRegValue(RegType.rIP);

	public Device(byte[] biosFirmware, PortBus portBus, RamSize size, string? name = null!, string? nameProc = null!, string? nameRam = null!)
	{
		_nameDevice = new(name);
		_portBus = portBus;
		_ram = new MemoryBus(size, _nameDevice, nameRam, biosFirmware);
		_processor = new Processor(_ram, _nameDevice, _portBus, _consoleLock, nameProc);
		_ioMemory = new byte[_portBus.PortsOnDevice];
	}

	public string GetAllData()
	{
		return _processor.IsRunning ? string.Empty : _processor.DumbRegs();
	}

	private void ConsoleLock(string text, LogLevel level = LogLevel.Log)
	{
		LoggerKernel.LogFromDevice(in _nameDevice, text, level);
	}

	public void LoadProgram(byte[] program, ulong loadAddress)
	{
		for (ulong i = 0; i < (ulong)program.Length; i++)
		{
			RAMResultInt8 result = _ram.WriteInt8LE(loadAddress + i, program[i]);
			if (!result.IsSuccess)
				throw new InvalidOperationException(
					$"Не удалось записать программу по адресу 0x{loadAddress + i:X}");
		}
	}

	public bool TryLoadProgramFast(ReadOnlySpan<byte> program, ulong loadAddress)
	{
		Memory<byte> ram = _ram.Memory;
		ulong ramLength = (ulong)ram.Length;
		ulong programLength = (ulong)program.Length;

		if (loadAddress + programLength < loadAddress || loadAddress + programLength > ramLength)
		{
			return false;
		}

		Span<byte> target = ram.Span.Slice((int)loadAddress, program.Length);
		program.CopyTo(target);

		return true;
	}

	[Obsolete(
	"""
	Рекомендуется использовать 'LaunchDeviceAsThreadTask' с ThreadScheduler. 
	Он эффективнее управляет потоками.
	
	Внимание: Прямой запуск потока безопасен, но если ThreadScheduler 
	работает в режиме 'MaxThreadCPU', это может вызвать сильные 
	просадки производительности из-за конкуренции за ядра процессора.
	""", false)]
	public void LaunchDeviceOnDedicatedThread(ulong? start,
							 bool isDebug,
							 int delay,
							 bool snowTimer,
							 Action<Action<string, LogLevel>>? titleAct,
							 Action<Action<string, LogLevel>>? startAct,
							 Action<Action<string, LogLevel>>? endAct)
	{

		titleAct?.Invoke(ConsoleLock);

		ulong startIndex = start ?? _ram.RamSize;
		stepCounter = 0;

		_simulationThread = new Thread(() =>
		RunSimulationLoop(startIndex, isDebug, delay, snowTimer, startAct, endAct))
		{
			Name = $"VM_Thread_{_nameDevice}",
			IsBackground = true
		};

		_simulationThread.Start();
	}

	public ThreadHandle? LaunchManagedDeviceThread(ThreadScheduler scheduler, ulong? start,
							 bool isDebug,
							 int delay,
							 bool snowTimer,
							 Action<Action<string, LogLevel>>? titleAct,
							 Action<Action<string, LogLevel>>? startAct,
							 Action<Action<string, LogLevel>>? endAct)
	{
		titleAct?.Invoke(ConsoleLock);

		ulong startIndex = start ?? _ram.RamSize;
		stepCounter = 0;

		var task = ThreadScheduler.CreateAsTask(() => RunSimulationLoop(startIndex, isDebug, delay, snowTimer, startAct, endAct));
		var id = scheduler.FoundFreeWorkerId();

		if (id == null) return null;

		return scheduler.TryAddNewTaskToWorker(id.Value, task, out var handle) ? handle : null;
	}
	public void BreakPointerLaunchDevice(ulong? start, 
							Action<Action<string, LogLevel>>? titleAct)
	{
		titleAct?.Invoke(ConsoleLock);
		stepCounter = 0;
		ulong startIndex = start ?? _ram.RamSize;

		_processor.LaunchProgramm(startIndex);
	}
	[Obsolete("""
	Используйте перегрузку 'Stop(ThreadHandle, ...)' для работы через ThreadScheduler.
	
	Внимание: Этот метод предназначен только для потоков, запущенных через 'StartOnDedicatedThread'.
	Смешивание вызовов (например, запуск через Scheduler, а остановка этим методом) 
	приведет к зависанию задачи или утечке ресурсов в пуле воркеров.
	""", error: false)]
	public void StopDevice(int timeoutMilliseconds,
						   Action<Action<string, LogLevel>>? threadLiveTrue,
						   Action<Action<string, LogLevel>>? threadLiveFalse,
						   Action<Action<string, LogLevel>>? threadStopTrue,
						   Action<Action<string, LogLevel>>? threadStopFalse)
	{
		// 1. Проверяем, запущен ли поток вообще
		if (_simulationThread == null || !_simulationThread.IsAlive)
		{
			threadLiveFalse?.Invoke(ConsoleLock);
			return;
		}

		threadLiveTrue?.Invoke(ConsoleLock);

		_processor.EnqueueBiosStatus(BiosStatus.EndProgramm);

		if (_simulationThread.Join(timeoutMilliseconds))
		{
			threadStopTrue?.Invoke(ConsoleLock);
		}
		else
		{
			threadStopFalse?.Invoke(ConsoleLock);
		}

		_simulationThread = null;
	}

	public void Stop(ThreadHandle handle,
					 TimeSpan timeout,
					 Action<Action<string, LogLevel>>? onSuccess = null,
					 Action<Action<string, LogLevel>>? onTimeout = null)
	{
		_processor.EnqueueBiosStatus(BiosStatus.EndProgramm);

		if (handle.Wait(timeout))
		{
			onSuccess?.Invoke(ConsoleLock);
			handle.Dispose();
		}
		else
		{
			onTimeout?.Invoke(ConsoleLock);
		}
	}

	public void Stop(ThreadHandle handle,
					 int timeoutMilliseconds,
					 Action<Action<string, LogLevel>>? onSuccess = null,
					 Action<Action<string, LogLevel>>? onTimeout = null) => Stop(handle, TimeSpan.FromMilliseconds(timeoutMilliseconds), onSuccess, onTimeout);


	public void InitHeap(ulong hp) => _processor.InitRegHP(hp);

	public void RunSimulationLoop(ulong start,
								  bool isDebug,
								  int delay,
								  bool launchTimer,
								  Action<Action<string, LogLevel>>? startAct,
								  Action<Action<string, LogLevel>>? endAct)
	{
		startAct?.Invoke(ConsoleLock);
		bool useSleepMode = delay != 0;
		Stopwatch? sw = null;
		if (launchTimer)
		{
			sw = Stopwatch.StartNew();
		}

		_processor.LaunchProgramm(start);

		while (_processor.IsRunning)
		{
			_processor.ExternalCommandExecute();
			if (_processor.IsSleeping)
			{
				_wakeSignal.Wait(10);
				_wakeSignal.Reset();
				continue;
			}
			stepCounter++;
			_processor.Step();

			if (isDebug) DebugOutput();
			if (useSleepMode) Thread.Sleep(delay);
		}

		if (sw != null)
		{
			sw.Stop();
			TimeSpan ts = sw.Elapsed;
			string elapsedTime = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
			PrintFullTimeSpanInfo(ts);
			ConsoleLock($"Время симуляции: {elapsedTime}");
		}

		endAct?.Invoke(ConsoleLock);
	}

	public void PrintFullTimeSpanInfo(TimeSpan ts)
	{
		lock (_consoleLock)
		{
			_sharedBuilder.Clear();

			// Используем ISpanFormattable под капотом .NET, который пишет числа прямо в буфер
			_sharedBuilder.Append($"=== ПОДРОБНАЯ СТАТИСТИКА ВРЕМЕНИ ===\n" +
								  $"Дни:          {ts.Days}\n" +
								  $"Часы:         {ts.Hours}\n" +
								  $"Минуты:       {ts.Minutes}\n" +
								  $"Секунды:      {ts.Seconds}\n" +
								  $"Миллисекунды: {ts.Milliseconds}\n" +
								  $"Микросекунды: {ts.Microseconds}\n" +
								  $"Наносекунды:  {ts.Nanoseconds}\n" +
								  "------------------------------------\n" +
								  $"Всего дней:          {ts.TotalDays:F6}\n" +
								  $"Всего часов:         {ts.TotalHours:F4}\n" +
								  $"Всего минут:         {ts.TotalMinutes:F2}\n" +
								  $"Всего секунд:        {ts.TotalSeconds:F3}\n" +
								  $"Всего миллисекунд:   {ts.TotalMilliseconds:F0}\n" +
								  $"Всего микросекунд:   {ts.TotalMicroseconds:F0}\n" +
								  $"Всего наносекунд:    {ts.TotalNanoseconds:F0}\n" +
								  $"Всего тиков (.NET):  {ts.Ticks}\n" +
								  "------------------------------------\n" +
								  $"Общее количество тактов: {StepCount}\n" +
								  "====================================\n");

			ConsoleLock(_sharedBuilder.ToString());
		}
	}

	public byte ReadPort(ulong offset)
	{
		if (offset >= (ulong)_ioMemory.Length)
			return 0; // или ошибка
		return _ioMemory[offset];
	}

	public void WritePort(ulong offset, byte value)
	{
		if (offset < (ulong)_ioMemory.Length)
			_ioMemory[offset] = value;
	}

	public void WakeProcessor()
	{
		_processor.EnqueueExternalCommand((uint)OpCode.WAKE);
		ActionOnWake?.Invoke(ConsoleLock);
		_wakeSignal.Set();
	}

	public void EnqueueExternalCommand(uint instruction)
	{
		_processor.EnqueueExternalCommand(instruction);
		_wakeSignal.Set();
	}
	public void NextStepProcessor(bool isDebug = false)
	{
		if (!_processor.IsRunning) return;

		_processor.Step();
		stepCounter++;
		if (isDebug) DebugOutput();

	}

	public void NextStepProcessorCount(int count = 5, bool isDebug = false)
	{
		if (!_processor.IsRunning) return;
		for (int i = 0; i < count; i++)
		{
			_processor.Step();
			stepCounter++;
			if (isDebug) DebugOutput();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void DebugOutput()
	{
		ulong currentIp = _processor.GetRegValue(RegType.rIP);
		ConsoleLock($"[Такт {stepCounter}] Выполнен IP: {currentIp} -> Следующий IP: {_processor.GetRegValue(RegType.rIP)}");
		ConsoleLock($"r0: {_processor.GetRegValue(RegType.r0)} | r1: {_processor.GetRegValue(RegType.r1)} | rFL: {_processor.GetRegValue(RegType.rFL)}");
	}

	public void UpdateBios(byte[] newBios)
	{
		_ram.SetBios(newBios);
	}

	public void ResetMemoryRam()
	{
		_ram.ClearMemory();
	}

	private int _disposed;

	public void Dispose()
	{
		if (Interlocked.Exchange(ref _disposed, 1) != 0)
			return;

		// Попросим поток остановиться, если он ещё жив
		var simulationThread = _simulationThread;
		if (simulationThread != null && simulationThread.IsAlive)
		{
			_processor.EnqueueBiosStatus(BiosStatus.EndProgramm);
			_processor.EnqueueExternalCommand((uint)OpCode.WAKE);
			_wakeSignal.Set();
			simulationThread.Join(TimeSpan.FromSeconds(10));
		}

		_ram?.Dispose();
		ProcessorPoolEmulator.Return(_processor);
		GC.SuppressFinalize(this);
	}
}
````

## File: Controllers/ControllersData/PortBus.cs
````csharp
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.RamSystem;

namespace Kernel.ControllersData;

public interface IPortUse
{
	public byte ReadPort(ulong offset);
	public void WritePort(ulong offset, byte value);
	public void WakeProcessor();
}

public sealed class PortBus(SizePort ports, SizePortOnDevice portsOnDev, NameDeviceToken nameDevice, ReadOnlySpan<char> name)
{
	private readonly ulong _totalPorts = 1UL << (byte)ports;
	private readonly uint _portsOnDevice = 1U << (byte)portsOnDev;

	private readonly uint _sectorCount = 1U << ((byte)ports - (byte)portsOnDev);
	private readonly IPortUse[] _devices = new IPortUse[1U << ((byte)ports - (byte)portsOnDev)];

	private readonly NameDeviceToken _nameDevice = nameDevice.CreateChild(name);

	private readonly byte _deviceShift = (byte)portsOnDev;
	private readonly ulong _offsetMask = (1U << (byte)portsOnDev) - 1U;

	public NameDeviceToken NameDevice => _nameDevice;
	public uint SectorCount => _sectorCount;
	public uint PortsOnDevice => _portsOnDevice;
	public ulong TotalPorts => _totalPorts;

	public int AllocateFreeSector()
	{
		for (int i = 0; i < _sectorCount; i++)
			if (_devices[i] == null) return i;
		return -1;
	}

	public bool IsFreeSector(uint sector) => sector < _sectorCount && _devices[sector] == null;

	public bool RegisterDevice(IPortUse device, uint sector)
	{
		if (!IsFreeSector(sector)) return false;

		_devices[sector] = device;
		return true;
	}

	public void UnregisterDevice(uint sector)
	{
		if (sector < _sectorCount)
			_devices[sector] = null!;
	}

	private (IPortUse? device, ulong offset) ResolveAddress(ulong address)
	{
		uint sector = (uint)(address >> _deviceShift);
		if (sector >= _sectorCount) return (null, 0);

		var dev = _devices[sector];
		if (dev != null)
		{
			ulong offset = address & _offsetMask;
			return (dev, offset);
		}
		return (null, address);
	}


	public RAMResultInt8 ReadPort(ulong address)
	{
		var (device, offset) = ResolveAddress(address);
		return device != null
			? new(device.ReadPort(offset))
			: new(BiosStatus.NullDeviceOutput, address, _nameDevice);
	}

	public RAMResultInt8 WritePort(ulong address, byte value)
	{
		var (device, offset) = ResolveAddress(address);
		if (device != null)
		{
			device.WritePort(offset, value);
			return new(value);
		}
		return new(BiosStatus.NullDeviceInput, address, _nameDevice);
	}

	public bool WakeProcessor(ulong address)
	{
		var (device, _) = ResolveAddress(address);
		if (device != null)
		{
			device.WakeProcessor();
			return true;
		}
		return false;
	}

	public IPortUse? GetDevice(uint sector) => _devices[sector];
}
````

## File: Controllers/ProcessorSystem/Processor.cs
````csharp
using Kernel.Common;
using Kernel.ControllersData;
using Kernel.RamSystem;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using static Kernel.Common.InstructionDecoder;

namespace Kernel.ProcessorSystem;

public sealed class Processor(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
{
	[InlineArray(CountReg)]
	private struct ArrayRegisters
	{
		public ulong _element0;
	}

	private const int CountReg = 32;
	private const ulong NegativeMask = 0x8000000000000000;

	private ArrayRegisters _registers;

	private readonly ConcurrentQueue<uint> _externalCommands = new();
	private readonly ConcurrentQueue<BiosStatus> _statusFromSimulation = [];

	private NameDeviceToken _nameDevice = nameDeviceToken.CreateChild(name);
	private MemoryBus _ram = ram;
	private PortBus _portBus = portBus;
	private Lock _regLock = regLock;

	private volatile bool _isRunning = false;
	private volatile bool _hasSimulationStatus = false;
	private volatile bool _hasExternalCommand = false;
	private bool _sleeping = false;

	private Action<ulong, uint, OpCode>? _snowOpcodeCallback;

	public bool IsSleeping => _sleeping;
	public bool IsRunning => _isRunning;
	private ulong IsZero => _registers[RegType.rFL.Int] & 1UL;
	private ulong IsNegative => _registers[RegType.rFL.Int] & 2UL;

	public bool TryInitInPool(MemoryBus ram, NameDeviceToken nameDeviceToken, PortBus portBus, Lock regLock, ReadOnlySpan<char> name)
	{
		if (_isRunning) return false;

		Reset();

		_nameDevice = nameDeviceToken.CreateChild(name);
		_ram = ram;
		_portBus = portBus;
		_regLock = regLock;

		return true;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong GetRegValue(RegType reg)
	{
		return reg == RegType.rZ ? 0 : _registers[reg.Int];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetRegValue(RegType reg, ulong value)
	{
		if (reg == RegType.rZ) return;

		_registers[(int)reg] = (reg == RegType.rFL) ? (value & 0x3UL) : value;
	}



	public void EnqueueBiosStatus(BiosStatus status)
	{
		_hasSimulationStatus = true;
		_statusFromSimulation.Enqueue(status);
	}

	private void ConsoleLock(string text, LogLevel level = LogLevel.Log)
	{
		LoggerKernel.LogFromDevice(in _nameDevice, text, level);
	}

	public string DumbRegs()
	{
		StringBuilder stringBuilder = new(128);
		for (int i = 0; i < CountReg; i++)
		{
			stringBuilder.AppendLine($"[{(RegType)i}]= {_registers[i]}");
		}
		stringBuilder.AppendLine($"IP {_registers[RegType.rIP.Int]}");
		return stringBuilder.ToString();
	}
	private void UpdateFlags(ulong result)
	{
		ulong currentFlags = _registers[RegType.rFL.Int];

		currentFlags &= ~(1UL | 2UL);

		if (result == 0) currentFlags |= 1UL;
		if ((result & NegativeMask) != 0) currentFlags |= 2UL;

		_registers[RegType.rFL.Int] = currentFlags;
	}

	public void LaunchProgramm(ulong startAddress, Action<ulong, uint, OpCode>? act = null)
	{
		_snowOpcodeCallback = act;
		_sleeping = false;
		_registers[RegType.rIP.Int] = startAddress;
		_isRunning = true;

		_registers[RegType.rCL.Int] = 0;
		_registers[RegType.rCD.Int] = 0;
		_registers[RegType.rFL.Int] = 0;
		_registers[RegType.rTB.Int] = 0;

		ulong stackTop = _ram.RamSize;
		stackTop &= ~0x7UL;

		_registers[RegType.rSP.Int] = stackTop;
	}

	public void ExternalCommandExecute()
	{
		if (_hasExternalCommand)
		{        
			while (_externalCommands.TryDequeue(out uint cmd))
			{
				ExecuteExternalCommand(cmd);
				if (!_isRunning) return;   // если команда остановила процессор
			}
			_hasExternalCommand = false;
		}
	}
	public void Step()
	{
		if (_sleeping) return;

		ExternalCommandExecute();

		if (_hasSimulationStatus)
		{
			while (_statusFromSimulation.TryDequeue(out BiosStatus result))
			{
				if (result != BiosStatus.Success)
				{
					_isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(result, 1UL), ulong.MaxValue, _regLock);
					return;
				}
			}
			_hasSimulationStatus = false;
		}

		ulong ip = _registers[RegType.rIP.Int];

		RAMResultInt32 instResult = _ram.ReadInt32LE(ip);

		if (!instResult.IsSuccess)
		{
			_isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(instResult.Status, instResult.FaultAddress), ip, _regLock);
			return;
		}

		uint rawInst = instResult.Data;
		OpCode opCode = (OpCode)(rawInst & 0xFF);

		_snowOpcodeCallback?.Invoke(ip, rawInst, opCode);
		ip += 4;

		ulong data = 0;

		if (HasNeed64IntData(opCode))
		{
			ip = (ip + 7) & ~7UL;
			RAMResultInt64 dataResult = _ram.ReadInt64LE(ip);
			if (!dataResult.IsSuccess)
			{
				_isRunning = ProcessorHelpers.TryContinueAfterStatus(new ResultInstruction(dataResult.Status, dataResult.FaultAddress), ip, _regLock);
				return;
			}

			data = dataResult.Data;
			ip += 8;
		}

		_registers[RegType.rIP.Int] = ip;

		ResultInstruction dat = DecodeInstruction(rawInst, data);

		if (dat.BiosStatus != BiosStatus.Success)
		{
			_isRunning = ProcessorHelpers.TryContinueAfterStatus(dat, ip, _regLock);
		}
	}


	public ResultInstruction DecodeInstruction(uint rawInst, ulong data) => GetOpCode(rawInst) switch
	{
		// === 1. Системные команды ===
		OpCode.NOP => ResultInstruction.IsSucced,
		OpCode.END => EndProgramm(),
		OpCode.PRINT => InstructionPRINT(GetReg1(rawInst)),
		// === 2. Работа с памятью (Указатели и регистры) ===
		OpCode.MOV => InstructionMOV(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.LOAD => InstructionLOAD(GetDataSizeCode(rawInst), GetReg1(rawInst), data),
		OpCode.STORE => InstructionSTORE(GetDataSizeCode(rawInst), GetReg1(rawInst), data),
		OpCode.STORE_IND => InstructionSTORE_IND(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.LOAD_IND => InstructionLOAD_IND(GetDataSizeCode(rawInst), GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.LDI => InstructionLDI(GetReg1(rawInst), data),

		// === 3. Арифметика и Логика (Тьюринг-базис) ===
		OpCode.ADD => InstructionADD(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.SUB => InstructionSUB(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.INC => InstructionINC(GetReg1(rawInst)),
		OpCode.DEC => InstructionDEC(GetReg1(rawInst)),
		OpCode.MULT_INT => InstructionMULT_INT(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.SHR => InstructionSHR(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.DIV => InstructionDIV(GetReg1(rawInst), GetReg2(rawInst)),

		// === 4. Логика ===
		OpCode.AND => InstructionAND(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.OR => InstructionOR(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.XOR => InstructionXOR(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.NOT => InstructionNOT(GetReg1(rawInst)),

		// === 5. Управление потоком ===
		OpCode.JMP => InstructionJMP(data),
		OpCode.JZ => InstructionJZ(data),
		OpCode.JNZ => InstructionJNZ(data),
		OpCode.JG => InstructionJG(data),
		OpCode.JL => InstructionJL(data),

		// === 6. Работа со Стеком ===
		OpCode.PUSH => InstructionPUSH(GetReg1(rawInst)),
		OpCode.POP => InstructionPOP(GetReg1(rawInst)),
		OpCode.CALL => InstructionCALL(data),
		OpCode.RET => InstructionRET(),

		// === 7. Ввод-вывод ===
		OpCode.IN => InstructionIN(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.OUT => InstructionOUT(GetReg1(rawInst), GetReg2(rawInst)),
		OpCode.INT => InstructionINT(GetReg1(rawInst)),   // GetReg1(rawInst) содержит номер вектора
		OpCode.IRET => InstructionIRET(),

		OpCode.PRINT_INT => InstructionPRINT_INT(GetReg1(rawInst)),
		OpCode.ALLOC => InstructionALLOC(GetReg1(rawInst)),
		OpCode.WAKE_INT => InstructionWAKE_INT(GetReg1(rawInst)),
		OpCode.HALT => InstructionHALT(),
		_ => new ResultInstruction(BiosStatus.NotImplementedOpCode, (ulong)GetOpCode(rawInst))
	};

	private ResultInstruction InstructionHALT()
	{
		_sleeping = true;
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionWAKE_INT(RegType reg1)
	{
		ulong regV = _registers[reg1.Int];
		if (!_portBus.WakeProcessor(regV))
		{
			ulong ip = _registers[RegType.rIP.Int];
			return new(BiosStatus.NullDeviceOutput, ip);
		}

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionDIV(RegType reg1, RegType reg2)
	{
		ulong divisor = _registers[reg2.Int];
		if (divisor == 0)
		{
			ulong ip = _registers[RegType.rIP.Int];
			return new ResultInstruction(BiosStatus.DivOnZero, ip);
		}
		ulong dividend = _registers[reg1.Int];
		ulong res = dividend / divisor;
		SetRegValue(reg1, res);
		UpdateFlags(res);
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionSHR(RegType reg1, RegType reg2)
	{
		ulong regv1 = _registers[reg1.Int];
		byte regv2 = (byte)_registers[reg2.Int];
		ulong res = regv1 >> regv2;
		SetRegValue(reg1, res);
		UpdateFlags(res);
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionMULT_INT(RegType reg1, RegType reg2)
	{
		ulong regv1 = _registers[reg1.Int];
		ulong regv2 = _registers[reg2.Int];
		ulong res = regv1 * regv2;
		SetRegValue(reg1, res);
		UpdateFlags(res);
		return ResultInstruction.IsSucced;
	}

#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
	private ResultInstruction InstructionALLOC(RegType reg1)
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
	{
		ulong size = _registers[RegType.r0.Int];
		ulong hp = _registers[RegType.rHP.Int];
		// выравниваем размер вверх до кратности 8
		if (size % 8 != 0) size = (size + 7) & ~7UL;
		ulong result = hp;
		_registers[RegType.rHP.Int] = hp + size;
		_registers[RegType.r0.Int] = result;
		UpdateFlags(result);
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionINT(RegType reg)
	{
		uint vector = (uint)_registers[reg.Int] & 0x1F; // 32 вектора
		ulong tableBase = _registers[RegType.rTB.Int];
		ulong handlerAddr;
		RAMResultInt64 readResult = _ram.ReadInt64LE(tableBase + (ulong)vector * 8);
		if (!readResult.IsSuccess)
			return new ResultInstruction(readResult.Status, readResult.FaultAddress);
		handlerAddr = readResult.Data;

		// Сохраняем текущий IP в стек
		ulong ip = _registers[RegType.rIP.Int];
		ulong sp = _registers[RegType.rSP.Int];
		sp -= 8;
		RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, ip);

		if (!writeResult.IsSuccess)
			return new ResultInstruction(writeResult.Status, sp);

		_registers[RegType.rSP.Int] = sp;
		if (handlerAddr == 0)
		{
			ConsoleLock("handlerAddr == 0");
			return EndProgramm();
		}
		// Переходим на обработчик
		_registers[RegType.rIP.Int] = handlerAddr;
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionIRET()
	{
		// Восстанавливаем IP из стека
		ulong sp = _registers[RegType.rSP.Int];
		RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
		if (!readResult.IsSuccess)
			return new ResultInstruction(readResult.Status, sp);
		sp += 8;
		_registers[RegType.rIP.Int] = readResult.Data;
		_registers[RegType.rSP.Int] = sp;
		return ResultInstruction.IsSucced;
	}

	public ResultInstruction EndProgramm()
	{
		_isRunning = false;
		return new ResultInstruction(BiosStatus.EndProgramm, 0);
	}

	public ResultInstruction InstructionPRINT_INT(RegType reg)
	{
		ConsoleLock(_registers[reg.Int].ToString());
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionPRINT(RegType reg)
	{
		LoggerProvider.Info((char)_registers[reg.Int]);
		return ResultInstruction.IsSucced;
	}

	public readonly struct ResultInstruction(BiosStatus biosStatus, ulong adress)
	{
		public readonly BiosStatus BiosStatus = biosStatus;
		public readonly ulong Adress = adress;

		public static ResultInstruction IsSucced => new(BiosStatus.Success, 0);
	}

	private ResultInstruction InstructionMOV(RegType reg1, RegType reg2)
	{
		SetRegValue(reg1, _registers[reg2.Int]);
		return ResultInstruction.IsSucced;
	}
	private ResultInstruction InstructionLOAD(OpCodeSize sizeT, RegType reg, ulong adress)
	{

		switch (sizeT)
		{
			case OpCodeSize.S8:
				RAMResultInt8 data8 = _ram.ReadInt8LE(adress);
				if (data8.IsSuccess)
				{
					SetRegValue(reg, data8.Data);
					UpdateFlags(data8.Data);
					return ResultInstruction.IsSucced;
				}
				return new(data8.Status, data8.FaultAddress);


			case OpCodeSize.S16:
				RAMResultInt16 data16 = _ram.ReadInt16LE(adress);
				if (data16.IsSuccess)
				{
					SetRegValue(reg, data16.Data);
					UpdateFlags(data16.Data);

					return ResultInstruction.IsSucced;
				}
				return new(data16.Status, data16.FaultAddress);


			case OpCodeSize.S32:
				RAMResultInt32 data32 = _ram.ReadInt32LE(adress);
				if (data32.IsSuccess)
				{
					SetRegValue(reg, data32.Data);
					UpdateFlags(data32.Data);

					return ResultInstruction.IsSucced;
				}
				return new(data32.Status, data32.FaultAddress);


			case OpCodeSize.S64:
				RAMResultInt64 data64 = _ram.ReadInt64LE(adress);
				if (data64.IsSuccess)
				{
					SetRegValue(reg, data64.Data);
					UpdateFlags(data64.Data);

					return ResultInstruction.IsSucced;
				}
				return new(data64.Status, data64.FaultAddress);


			default:
				// Защита на случай передачи некорректного или нереализованного OpCodeSize
				return new(BiosStatus.SegmentationFault, adress);
		}


	}

	private ResultInstruction InstructionSTORE(OpCodeSize sizeT, RegType reg, ulong adress)
	{

		switch (sizeT)
		{
			case OpCodeSize.S8:
				RAMResultInt8 data8 = _ram.WriteInt8LE(adress, (byte)_registers[reg.Int]);
				if (data8.IsSuccess)
				{
					UpdateFlags(data8.Data);
					return ResultInstruction.IsSucced;
				}
				return new(data8.Status, data8.FaultAddress);

			case OpCodeSize.S16:
				RAMResultInt16 data16 = _ram.WriteInt16LE(adress, (ushort)_registers[reg.Int]);
				if (data16.IsSuccess)
				{
					UpdateFlags(data16.Data);
					return ResultInstruction.IsSucced;
				}
				return new(data16.Status, data16.FaultAddress);


			case OpCodeSize.S32:
				RAMResultInt32 data32 = _ram.WriteInt32LE(adress, (uint)_registers[reg.Int]);
				if (data32.IsSuccess)
				{
					UpdateFlags(data32.Data);
					return ResultInstruction.IsSucced;
				}
				return new(data32.Status, data32.FaultAddress);


			case OpCodeSize.S64:
				RAMResultInt64 data64 = _ram.WriteInt64LE(adress, _registers[reg.Int]);
				if (data64.IsSuccess)
				{
					UpdateFlags(data64.Data);
					return ResultInstruction.IsSucced;
				}
				return new(data64.Status, data64.FaultAddress);


			default:
				return new(BiosStatus.AlignmentFault, adress);
		}
	}

	private ResultInstruction InstructionLOAD_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
	{
		return InstructionLOAD(sizeT, reg1, _registers[reg2.Int]);
	}

	private ResultInstruction InstructionSTORE_IND(OpCodeSize sizeT, RegType reg1, RegType reg2)
	{
		return InstructionSTORE(sizeT, reg1, _registers[reg2.Int]);
	}

	private ResultInstruction InstructionLDI(RegType reg, ulong value)
	{
		SetRegValue(reg, value);
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionADD(RegType reg1, RegType reg2)
	{
		ulong value1 = _registers[reg1.Int];
		ulong value2 = _registers[reg2.Int];
		ulong res = value1 + value2;
		SetRegValue(reg1, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionSUB(RegType reg1, RegType reg2)
	{
		ulong value1 = _registers[reg1.Int];
		ulong value2 = _registers[reg2.Int];
		ulong res = value1 - value2;
		SetRegValue(reg1, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionINC(RegType reg)
	{
		ulong value1 = _registers[reg.Int];
		ulong res = value1 + 1;
		SetRegValue(reg, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionDEC(RegType reg)
	{
		ulong value1 = _registers[reg.Int];
		ulong res = value1 - 1;
		SetRegValue(reg, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionAND(RegType reg1, RegType reg2)
	{
		ulong value1 = _registers[reg1.Int];
		ulong value2 = _registers[reg2.Int];
		ulong res = value1 & value2;
		SetRegValue(reg1, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionOR(RegType reg1, RegType reg2)
	{
		ulong value1 = _registers[reg1.Int];
		ulong value2 = _registers[reg2.Int];
		ulong res = value1 | value2;
		SetRegValue(reg1, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionXOR(RegType reg1, RegType reg2)
	{
		ulong value1 = _registers[reg1.Int];
		ulong value2 = _registers[reg2.Int];
		ulong res = value1 ^ value2;
		SetRegValue(reg1, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionNOT(RegType reg)
	{
		ulong value = _registers[reg.Int];
		ulong res = ~value;
		SetRegValue(reg, res);
		UpdateFlags(res);

		return ResultInstruction.IsSucced;
	}
	private ResultInstruction InstructionJMP(ulong targetAddress)
	{
		_registers[RegType.rIP.Int] = targetAddress;
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionJZ(ulong targetAddress)
	{
		if (IsZero != 0) return InstructionJMP(targetAddress);
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionJNZ(ulong targetAddress)
	{
		if (IsZero == 0)
		{
			_registers[RegType.rIP.Int] = targetAddress;
		}
		return ResultInstruction.IsSucced;
	}


	private ResultInstruction InstructionJG(ulong targetAddress)
	{
		bool isZero = IsZero != 0;
		bool isNegative = IsNegative != 0;

		if (!isZero && !isNegative)
		{
			_registers[RegType.rIP.Int] = targetAddress;
		}
		return ResultInstruction.IsSucced;
	}


	private ResultInstruction InstructionJL(ulong targetAddress)
	{
		if (IsNegative != 0)
		{
			_registers[RegType.rIP.Int] = targetAddress;
		}
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionPUSH(RegType reg)
	{
		ulong value = _registers[reg.Int];
		ulong sp = _registers[RegType.rSP.Int];

		sp -= 8;

		RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, value);

		if (!writeResult.IsSuccess)
		{
			return new ResultInstruction(writeResult.Status, sp);
		}

		_registers[RegType.rSP.Int] = sp;
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionPOP(RegType reg)
	{
		ulong sp = _registers[RegType.rSP.Int];

		RAMResultInt64 readResult = _ram.ReadInt64LE(sp);

		if (!readResult.IsSuccess)
		{
			return new ResultInstruction(readResult.Status, sp);
		}

		sp += 8;

		SetRegValue(reg, readResult.Data);
		_registers[RegType.rSP.Int] = sp;

		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionCALL(ulong targetAddress)
	{
		ulong currentLink = _registers[RegType.rCL.Int];
		ulong callDepth = _registers[RegType.rCD.Int];
		ulong ip = _registers[RegType.rIP.Int];
		// Если глубина вложенности > 0, значит rCL уже занят предыдущим методом.
		// Спасаем его значение в стек.
		if (callDepth > 0)
		{
			ulong sp = _registers[RegType.rSP.Int];
			sp -= 8;

			RAMResultInt64 writeResult = _ram.WriteInt64LE(sp, currentLink);
			if (!writeResult.IsSuccess)
			{
				return new ResultInstruction(writeResult.Status, sp);
			}

			_registers[RegType.rSP.Int] = sp;
		}

		// Сохраняем адрес возврата в быстрый регистр rCL
		_registers[RegType.rCL.Int] = ip;

		// Увеличиваем счетчик вложенности
		callDepth++;

		// Переходим к коду вызванного метода
		ip = targetAddress;

		_registers[RegType.rCD.Int] = callDepth;
		_registers[RegType.rIP.Int] = ip;

		return ResultInstruction.IsSucced;
	}

	// Возврат из подпрограммы (RET)
	private ResultInstruction InstructionRET()
	{
		ulong callDepth = _registers[RegType.rCD.Int];
		ulong ip = _registers[RegType.rIP.Int];

		if (callDepth == 0)
		{
			ConsoleLock("Ошибка: RET вызван без предшествующего CALL. Программа остановлена.");
			return new ResultInstruction((BiosStatus)9, ip);
		}

		ulong returnAddress = _registers[RegType.rCL.Int];
		ip = returnAddress;


		callDepth--;

		if (callDepth > 0)
		{
			ulong sp = _registers[RegType.rSP.Int];

			RAMResultInt64 readResult = _ram.ReadInt64LE(sp);
			if (!readResult.IsSuccess)
			{
				return new ResultInstruction(readResult.Status, sp);
			}

			_registers[RegType.rCL.Int] = readResult.Data;
			sp += 8;
			_registers[RegType.rSP.Int] = sp;
		}
		else
		{
			_registers[RegType.rCL.Int] = 0;
		}

		_registers[RegType.rCD.Int] = callDepth;
		_registers[RegType.rIP.Int] = ip;
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionIN(RegType regDest, RegType regPort)
	{
		ulong portAddress = _registers[regPort.Int];
		RAMResultInt8 result = _portBus.ReadPort(portAddress);
		if (!result.IsSuccess)
			return new ResultInstruction(result.Status, result.FaultAddress);

		SetRegValue(regDest, result.Data);
		UpdateFlags(result.Data);
		return ResultInstruction.IsSucced;
	}

	private ResultInstruction InstructionOUT(RegType regSrc, RegType regPort)
	{
		ulong portAddress = _registers[regPort.Int];
		byte value = (byte)(_registers[regSrc.Int] & 0xFF);
		RAMResultInt8 result = _portBus.WritePort(portAddress, value);
		if (!result.IsSuccess)
			return new ResultInstruction(result.Status, result.FaultAddress);

		// Флаги обычно не меняются при выводе, но можно обновить по желанию
		return ResultInstruction.IsSucced;
	}

	public void Reset()
	{
		lock (_regLock)
		{
			_isRunning = false;
			_sleeping = false;
			ClearRegs();
			while (_externalCommands.TryDequeue(out _));
			while (_statusFromSimulation.TryDequeue(out _));
			_hasExternalCommand = false;
			_hasSimulationStatus = false;
		}
	}

	private void ClearRegs()
	{
		for (int i = 0; i < CountReg; i++) _registers[i] = 0UL;
	}

	public void InitRegHP(ulong hpInit) => _registers[RegType.rHP.Int] = hpInit;

	public void EnqueueExternalCommand(uint instruction)
	{
		_hasExternalCommand = true;
		_externalCommands.Enqueue(instruction);
	}

	public void ExecuteExternalCommand(uint rawInst)
	{
		OpCode opCode = GetOpCode(rawInst);
		switch (opCode)
		{
			case OpCode.END: _isRunning = false; return;
			case OpCode.HALT: _sleeping = true; return;
			case OpCode.WAKE: _sleeping = false; return;

			default: return;
		}
	}
}
````

## File: Controllers/RamSystem/MemoryBus.cs
````csharp
using Kernel.Common;
using System.Buffers.Binary;

namespace Kernel.RamSystem;

public class MemoryBus(RamSize size, NameDeviceToken nameDevice, ReadOnlySpan<char> name, byte[] biosRom = null!) : IDisposable
{
	private readonly NameDeviceToken _nameDevice = nameDevice.CreateChild(name);

	private NativeMemoryBuffer _memory = new (size);
	private byte[] _biosRom = biosRom;
	private ulong _biosRomSize = (ulong)biosRom.Length;
	private readonly ulong _ramSize = (ulong)size;
	private readonly ulong _biosRomStartCode = (ulong)size;

	public bool HaveBios => _biosRom != null && _biosRom.Length > 0;
	public ulong RamSize => _ramSize;

	public Span<byte> Span => _memory.AsSpan();
	public Span<byte> AsSpan(int start, int length) => _memory.AsSpan(start, length);
	public Memory<byte> Memory => _memory.AsMemory();
	public Memory<byte> AsMemory() => _memory.AsMemory();
	public Memory<byte> AsMemory(int start, int length) => _memory.AsMemory(start, length);

	public ReadOnlyMemory<byte> ReadOnlyMemory => _memory.AsMemory();

	/// <summary>
	/// Метод для чтения 8 битового целого числа (Int8)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
	public RAMResultInt8 ReadInt8LE(ulong address)
	{
		if (address < _ramSize)
		{
			return new RAMResultInt8(_memory[address]);
		}

		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress < _biosRomSize
			? new RAMResultInt8(_biosRom[biosAddress])
			: new RAMResultInt8(BiosStatus.SegmentationFault, address, _nameDevice);
	}

	/// <summary>
	/// Метод для чтения 16 битового целого числа (Int16)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

	public RAMResultInt16 ReadInt16LE(ulong address)
	{
		if ((address & 0x01) != 0) return new RAMResultInt16(BiosStatus.AlignmentFault, (uint)address, _nameDevice);

		if (address + 2 <= _ramSize)
		{
			return MemoryBusHelpers.GenerateInt16Le(_memory.AsSpan((int)address, 2));
		}

		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress + 2 <= _biosRomSize
			? MemoryBusHelpers.GenerateInt16Le(_biosRom.AsSpan((int)biosAddress, 2))
			: new RAMResultInt16(BiosStatus.SegmentationFault, (uint)address, _nameDevice);
	}

	/// <summary>
	/// Метод для чтения 32 битового целого числа (Int32)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
	public RAMResultInt32 ReadInt32LE(ulong address)
	{
		if ((address & 0x03) != 0) return new RAMResultInt32(BiosStatus.AlignmentFault, address, _nameDevice);

		if (address + 4 <= _ramSize)
			return MemoryBusHelpers.GenerateInt32Le(_memory.AsSpan((int)address, 4));


		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress + 4 <= _biosRomSize
			? MemoryBusHelpers.GenerateInt32Le(_biosRom.AsSpan((int)biosAddress, 4))
			: new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);
	}

	/// <summary>
	/// Метод для чтения 64 битового целого числа (Int64)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

	public RAMResultInt64 ReadInt64LE(ulong address)
	{
		if ((address & 0x07) != 0) return new RAMResultInt64(BiosStatus.AlignmentFault, address, _nameDevice);

		if (address + 8 <= _ramSize)
		{
			return MemoryBusHelpers.GenerateInt64Le(_memory.AsSpan((int)address, 8));
		}

		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress + 8 <= _biosRomSize
			? MemoryBusHelpers.GenerateInt64Le(_biosRom.AsSpan((int)biosAddress, 8))
			: new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);
	}

	/// <summary>
	/// Метод для записи в память 8 битового целого числа (Int8)
	/// </summary>
	/// <param name="address"> Адрес в виртуальной памяти </param>
	/// <param name="value"> 16 битовое целое число (Int8) </param>
	/// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
	public RAMResultInt8 WriteInt8LE(ulong address, byte value)
	{
		if (address >= _ramSize) return new RAMResultInt8(BiosStatus.SegmentationFault, address, _nameDevice);

		_memory[address] = value;
		return new RAMResultInt8(value);
	}

	/// <summary>
	/// Метод для записи в память 16 битового целого числа (Int16)
	/// </summary>
	/// <param name="address"> Адрес в виртуальной памяти </param>
	/// <param name="value"> 16 битовое целое число (Int16) </param>
	/// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
	public RAMResultInt16 WriteInt16LE(ulong address, ushort value)
	{
		if ((address & 0x01) != 0) return new RAMResultInt16(BiosStatus.AlignmentFault, address, _nameDevice);
		if (address + 2 > _ramSize) return new RAMResultInt16(BiosStatus.SegmentationFault, address, _nameDevice);

		BinaryPrimitives.WriteUInt16LittleEndian(_memory.AsSpan((int)address, 2), value);
		return new RAMResultInt16(value);
	}


	/// <summary>
	/// Метод для записи в память 32 битового целого числа (Int32)
	/// </summary>
	/// <param name="address"> Адрес в виртуальной памяти </param>
	/// <param name="value"> 32 битовое целое число (Int32) </param>
	/// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
	public RAMResultInt32 WriteInt32LE(ulong address, uint value)
	{
		if ((address & 0x03) != 0) return new RAMResultInt32(BiosStatus.AlignmentFault, address, _nameDevice);
		if (address + 4 > _ramSize) return new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);

		BinaryPrimitives.WriteUInt32LittleEndian(_memory.AsSpan((int)address, 4), value);
		return new RAMResultInt32(value);
	}

	/// <summary>
	/// Метод для записи в память 64 битового целого числа (Int64)
	/// </summary>
	/// <param name="address"> Адрес в виртуальной памяти </param>
	/// <param name="value"> 64 битовое целое число (Int64) </param>
	/// <returns> Успешность операции, при возврате False программа остановится и выведится ошибка </returns>
	public RAMResultInt64 WriteInt64LE(ulong address, ulong value)
	{
		if ((address & 0x07) != 0) return new RAMResultInt64(BiosStatus.AlignmentFault, address, _nameDevice);
		if (address + 8 > _ramSize) return new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);

		BinaryPrimitives.WriteUInt64LittleEndian(_memory.AsSpan((int)address, 8), value);
		return new RAMResultInt64(value);
	}

	/// <summary>
	/// Метод для чтения 16 битового целого числа (Int16)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

	public RAMResultInt16 ReadInt16LEUnSafe(ulong address)
	{
		if (address + 2 <= _ramSize)
		{
			return MemoryBusHelpers.GenerateInt16Le(_memory.AsSpan((int)address, 2));
		}

		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress + 2 <= _biosRomSize
			? MemoryBusHelpers.GenerateInt16Le(_biosRom.AsSpan((int)biosAddress, 2))
			: new RAMResultInt16(BiosStatus.SegmentationFault, (uint)address, _nameDevice);
	}

	/// <summary>
	/// Метод для чтения 32 битового целого числа (Int32)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>
	public RAMResultInt32 ReadInt32LEUnSafe(ulong address)
	{
		if (address + 4 <= _ramSize)
		{
			return MemoryBusHelpers.GenerateInt32Le(_memory.AsSpan((int)address, 4));
		}

		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress + 4 <= _biosRomSize
			? MemoryBusHelpers.GenerateInt32Le(_biosRom.AsSpan((int)biosAddress, 4))
			: new RAMResultInt32(BiosStatus.SegmentationFault, address, _nameDevice);
	}

	/// <summary>
	/// Метод для чтения 64 битового целого числа (Int64)
	/// </summary>
	/// <param name="address"> Адресс в виртуальной памяти </param>
	/// <returns> Успешность операции, при возврате не BiosStatus.Success происходит исключение и остановка работы биоса </returns>

	public RAMResultInt64 ReadInt64LEUnSafe(ulong address)
	{
		if (address + 8 <= _ramSize)
		{
			return MemoryBusHelpers.GenerateInt64Le(_memory.AsSpan((int)address, 8));
		}

		ulong biosAddress = address - _biosRomStartCode;
		return biosAddress + 8 <= _biosRomSize
			? MemoryBusHelpers.GenerateInt64Le(_biosRom.AsSpan((int)biosAddress, 8))
			: new RAMResultInt64(BiosStatus.SegmentationFault, address, _nameDevice);
	}

	// ==============================
	//              API 
	// ==============================

	/// <summary>
	/// API для очистки памяти
	/// </summary>
	public void ClearMemory() => _memory.Clear();

	public void SetBios(byte[] bios)
	{
		_biosRom = bios ?? [];
		_biosRomSize = (ulong)_biosRom.Length;
	}

	private int _disposed;

	public void Dispose()
	{
		if (Interlocked.Exchange(ref _disposed, 1) != 0)
			return;

		if (_memory != null)
		{
			ClearMemory();
			_memory.Dispose();
			_memory = null!;
		}

		GC.SuppressFinalize(this);
	}
}
````

## File: Controllers/Utilites/ManagerDevices.cs
````csharp
using Kernel.BiosSystem;
using Kernel.Common;
using Kernel.ControllersData;

namespace Kernel.Utilites;

public class ManagerDevices
{
	public const int MaxCountElements = 64;

	// Плотные массивы данных (Dense Arrays)
	private readonly RamSize[] _size = new RamSize[MaxCountElements];

	private readonly uint[] _sectorByDenseIndex = new uint[MaxCountElements];

	private readonly string?[] _name = new string[MaxCountElements];
	private readonly string?[] _nameProc = new string[MaxCountElements];
	private readonly string?[] _nameRam = new string[MaxCountElements];
	private readonly string?[] _namePortBus = new string[MaxCountElements];

	private readonly Device?[] _devices = new Device[MaxCountElements];

	private readonly int[] _denseToId = new int[MaxCountElements];  // Отображает внутренний индекс в ID
	private readonly int[] _sparse = new int[MaxCountElements];     // Отображает ID во внутренний индекс
	private readonly Queue<int> _freeIds = new();

	private int _count = 0;
	private int _nextId = 0;

	private readonly PortBus _portBus;

	public int Count => _count;
	public ManagerDevices(PortBus portBus)
	{
		_portBus = portBus;
		Array.Fill(_sparse, -1);
	}

	public DeviceInfo? GetDeviceInfo(int i)
	{
		var dev = _devices[i];
		if (dev != null) return null;

		return new(_denseToId[i], _devices[i]!, _size[i],
				   (SizePortOnDevice)_portBus.PortsOnDevice, _sectorByDenseIndex[i],
				   _name[i], _devices[i]!.CreatedAt);

	}
	public struct DeviceInfo(int id, Device device, RamSize ramSize, SizePortOnDevice portSize, uint sector, string? name, DateTime createdAt)
	{
		public int Id = id;
		public Device Device { get; } = device;
		public RamSize RamSize = ramSize;
		public SizePortOnDevice PortSize = portSize;
		public uint Sector = sector;
		public string? Name = name;
		public DateTime CreatedAt = createdAt;
	}

	public IEnumerable<DeviceInfo> GetAllDevices()
	{
		for (int i = 0; i < _count; i++)
		{
			var dev = _devices[i];
			if (dev != null)
			{
				yield return new(
					_denseToId[i],
					_devices[i]!,
					_size[i],
					(SizePortOnDevice)_portBus.PortsOnDevice,
					_sectorByDenseIndex[i],
					_name[i],
					_devices[i]!.CreatedAt);
			}
		}
	}

	public Device? GetDevice(int id)
	{
		if (id < 0 || id >= MaxCountElements) return null;
		int denseIndex = _sparse[id];
		if (denseIndex == -1 || denseIndex >= _count) return null;
		return _devices[denseIndex];
	}

	public int CreateNewDevice(
		RamSize size,
		string? name,
		string? nameProc,
		string? nameRam,
		string? namePortBus,
		uint sector,
		byte[] biosFirmware = null!)
	{
		if (_count >= MaxCountElements) return -1;
		if (!_portBus.IsFreeSector(sector)) return -2;

		int deviceId = _freeIds.Count > 0 ? _freeIds.Dequeue() : _nextId++;
		if (deviceId >= MaxCountElements)
			return -3;

		int denseIndex = _count;

		// Запись конфигурации
		_size[denseIndex] = size;
		_name[denseIndex] = name;
		_nameProc[denseIndex] = nameProc;
		_nameRam[denseIndex] = nameRam;
		_namePortBus[denseIndex] = namePortBus;
		_sectorByDenseIndex[denseIndex] = sector;

		// Создаём само устройство
		
		var device = new Device(biosFirmware, _portBus, size, name, nameProc, nameRam);
		// Регистрируем в PortBus
		if (!_portBus.RegisterDevice(device, sector))
		{
			device.Dispose();
			return -4;
		}


		_devices[denseIndex] = device;

		// Связываем Sparse и Dense
		_denseToId[denseIndex] = deviceId;
		_sparse[deviceId] = denseIndex;

		_count++;
		return deviceId;
	}
	/// <summary>
	/// Удаляет устройство по его стабильному ID.
	/// Использует алгоритм Swap-And-Pop для O(1) удаления без сдвигов.
	/// </summary>
	/// <param name="id">ID устройства, которое нужно удалить.</param>
	public bool RemoveDevice(int id)
	{
		// 1. Проверка валидности ID
		if (id < 0 || id >= MaxCountElements)
			return false;

		int denseIndex = _sparse[id];
		if (denseIndex == -1 || denseIndex >= _count)
			return false; // устройство уже удалено или не существует

		// 2. Отключаем устройство от шины портов
		uint sector = _sectorByDenseIndex[denseIndex];
		_portBus.UnregisterDevice(sector);

		// 3. Освобождаем ресурсы самого устройства
		Device? device = _devices[denseIndex];
		if (device != null)
		{
			device.Dispose();
			_devices[denseIndex] = null;
		}

		// 4. Swap-And-Pop (если удаляем не последний элемент)
		int lastDenseIndex = _count - 1;
		if (denseIndex != lastDenseIndex)
		{
			// Копируем данные из последней записи в удаляемую позицию
			_size[denseIndex] = _size[lastDenseIndex];
			_name[denseIndex] = _name[lastDenseIndex];
			_nameProc[denseIndex] = _nameProc[lastDenseIndex];
			_nameRam[denseIndex] = _nameRam[lastDenseIndex];
			_namePortBus[denseIndex] = _namePortBus[lastDenseIndex];
			_sectorByDenseIndex[denseIndex] = _sectorByDenseIndex[lastDenseIndex];
			_devices[denseIndex] = _devices[lastDenseIndex];

			// Обновляем разреженный массив для перемещённого элемента
			int movedId = _denseToId[lastDenseIndex];
			_denseToId[denseIndex] = movedId;
			_sparse[movedId] = denseIndex;
		}

		// 5. Очищаем последнюю запись (для GC и предотвращения утечек)
		_size[lastDenseIndex] = default;
		_name[lastDenseIndex] = null;
		_nameProc[lastDenseIndex] = null;
		_nameRam[lastDenseIndex] = null;
		_namePortBus[lastDenseIndex] = null;
		_sectorByDenseIndex[lastDenseIndex] = 0;
		_devices[lastDenseIndex] = null;

		// 6. Инвалидируем разреженный индекс для удалённого ID
		_sparse[id] = -1;
		_freeIds.Enqueue(id);
		// 7. Уменьшаем счётчик
		_count--;
		return true;
	}

	public void Clear()
	{
		for (int i = _count - 1; i >= 0; i--)
		{
			_name[i] = null;
			_nameProc[i] = null;
			_nameRam[i] = null;
			_namePortBus[i] = null;
			_devices[i]?.Dispose();
			_devices[i] = null;
		}

		_freeIds.Clear();
		_count = 0;

		// 3. Важно для генерации новых ID (опционально, но логично для полного сброса)
		_nextId = 0;
	}

	public bool ChangeSector(int id, uint newSector)
	{
		if (id < 0 || id >= MaxCountElements) return false;
		int denseIndex = _sparse[id];
		if (denseIndex == -1 || denseIndex >= _count) return false;

		// Проверим, не занят ли новый сектор
		if (newSector >= _portBus.SectorCount) return false;
		var existing = _devices[denseIndex];
		// Попробуем зарегистрировать в новом секторе
		if (existing != null && _portBus.RegisterDevice(existing, newSector))
		{
			// Открепим старый сектор
			uint oldSector = _sectorByDenseIndex[denseIndex];
			_portBus.UnregisterDevice(oldSector);
			_sectorByDenseIndex[denseIndex] = newSector;
			return true;
		}
		return false;
	}
}
````

## File: ASM gen/IDEPage.xaml.cs
````csharp
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
	private readonly string _binDir;
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
		var editorService = new WpfEditorService(tabEditor, fileService);
		_projectManager = new ProjectService(fileService, editorService);

		_hostLogger = new LoggerBuilder()
					.WithOutPut(_outputView)
					.Build();

		_hostProject = new VMHostProjectBuilder()
					.WithPaths(_projectPaths)
					.WithProjectSevice(_projectManager)
					.WithLogger(_hostLogger)
					.Build();
		_hostEmulator = new VMEmulatorBuilder()
					.WithLogger(_hostLogger)
					.WithPortBusSize(totalPorts)
					.WithPortsPerDevice(portsPerDevice)
					.Build();


		_analizator = new(editorService, _outputView);

		_projectManager.OpenProject();
		_binDir = Path.Combine(_projectPath, "bin");
	}

	private void UpdateDeviceInfo()
	{
		var dev = _hostEmulator.MainDevice();
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

	private void LogSystemData(in CompilationResult res, bool optim)
	{
		if (SnowAssemler)
		{
			var text = VMHostHelper.DisassemblCode(res.Program.AsSpan());
			_outputView.Append(text.TextAsm);
		}
		var resultOpt = res.OptimizationResultLog;
		if (optim && resultOpt != null)
		{
			_outputView.Append("--- Result Optimization ---\n");

			_outputView.Append("[Function inlining]");
			_outputView.Append(resultOpt.Value.InlinedFunc.ToString());
			_outputView.Append("[Control flow simplification]");
			_outputView.Append(resultOpt.Value.RemovedNodes.ToString());
		}
	}
	public void CompileAndSafeProgramFile(ulong baseAddress = ProjectBuilder.BaseAdressProgramm)
	{
		bool optimize = OptimizationCode;
		_projectManager.SaveAllFiles();
		CompilationResult result = _hostProject.Compile(baseAddress, optimize);


		if (result.Success)
		{
			LogSystemData(in result, optimize);
			_projectManager.FileService.SaveProgramFile(result.Program!);
		}
		else
		{

			foreach (var err in result.Errors!)
				_outputView.Append(err, LogLevel.Error);
		}
	}

	private void CompileAndRun(ulong baseAddress = ProjectBuilder.BaseAdressProgramm)
	{
		_projectManager.SaveAllFiles();
		_device = _hostEmulator.CreateDeviceContext();

		if (_device == null)
		{
			_outputView.Append("Устройство не подготовлено к запуску", LogLevel.Error);
			return;
		}

		if (BiosMode)
		{
			// Проверяем наличие BIOS
			if (!_device.HaveBios)
			{
				_outputView.Append("Устройство не имеет BIOS. Создайте устройство с BIOS через Device Manager.", LogLevel.Error);
				return;
			}

			bool optimize = OptimizationCode;
			CompilationResult result = _hostProject.Compile(baseAddress, optimize);

			if (!result.Success)
			{
				foreach (var err in result.Errors!)
					_outputView.Append(err, LogLevel.Error);
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
					_outputView.Append(err, LogLevel.Error);
				return;
			}

			CallBackOnLaunch callBackOnLaunch = new(onEnd: OnEndLaunch);
			LogSystemData(in result, optimize);
			ulong startAdress = (ulong)result.Program!.Length + result.StartAdress;
			_device.ResetMemoryRam();
			_launchModeDevice = _device.LoadProgram(result.Program!);
			_launchModeDevice.SetHeapAddress(startAdress);
			_launchModeDevice.LaunchDeviceOnDedicatedThread(ProjectBuilder.BaseAdressProgramm, IsDebugMode, _delayDeviceThred, SnowTimer, callBackOnLaunch);
		}
	}
	private string? PrepareBootDisk(byte[] program, out int diskSector)
	{
		diskSector = -1;
		string imagePath = Path.Combine(_projectPath, $"boot_{Guid.NewGuid():N}.vmg");
		try
		{
			_hostEmulator.WriteBootableProgram(imagePath, program); // пишет заголовок + программу
			int sectorCount = Math.Max(1, (program.Length + 8 + DiskData.SectorSize - 1) / DiskData.SectorSize);
			diskSector = _hostEmulator.CreateDisk(imagePath, sectorCount);
			return imagePath;
		}
		catch (Exception ex)
		{
			_outputView.Append($"Ошибка подготовки загрузочного диска: {ex.Message}", LogLevel.Error);
			return null;
		}
	}

	private void OnEndLaunch(Action<string, LogLevel> logger)
	{
		logger.Invoke(_hostEmulator.GetDumpRegisters(), LogLevel.Log);
	}
	private void NewFile_Click(object sender, RoutedEventArgs e) => this.NewFile(_projectPath, _projectManager);

	private void SaveAll_Click(object sender, RoutedEventArgs e) => _projectManager.SaveAllFiles();

	private void Exit_Click(object sender, RoutedEventArgs e)
	{
		_device?.Stop();
		_device?.Dispose();
		_hostEmulator.Reset();
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
````


d