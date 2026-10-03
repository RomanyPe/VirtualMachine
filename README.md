# VM64 — Виртуальная машина, компилятор MiniC и ассемблер

Учебно-исследовательский проект: собственная **64-битная виртуальная машина** (CPU + RAM + порты ввода-вывода), **ассемблер**, **компилятор языка MiniC** (упрощённый C) и набор **оптимизаторов** (AST и peephole).

Проект демонстрирует полный цикл компиляции: исходный код → лексер → парсер → AST → оптимизации → генерация машинного кода → исполнение на эмуляторе.

---

## Содержание

- [Возможности](#возможности)
- [Архитектура](#архитектура)
- [Требования](#требования)
- [Быстрый старт](#быстрый-старт)
  - [Пример 1: ассемблерная программа](#пример-1-ассемблерная-программа)
  - [Пример 2: программа на MiniC](#пример-2-программа-на-minic)
  - [Пример 3: устройство вывода (Console)](#пример-3-устройство-вывода-console)
- [Система команд (ISA)](#система-команд-isa)
- [Регистры](#регистры)
- [Модель памяти](#модель-памяти)
- [Порты ввода-вывода](#порты-ввода-вывода)
- [Язык MiniC](#язык-minic)
- [Оптимизации](#оптимизации)
- [Структура решения](#структура-решения)
- [Сборка и запуск](#сборка-и-запуск)
- [Лицензия](#лицензия)

---

## Возможности

### Виртуальная машина
- 64-битная архитектура с 32 регистрами (`r0`–`r23`, `rZ`, `rSP`, `rHP`, `rIP`, `rFL`, `rCL`, `rCD`, `rTB`).
- 4-байтные инструкции фиксированной длины + 64-битные операнды (с выравниванием по 8 байт).
- Поддержка стековых операций, вызовов функций (`CALL`/`RET`) с сохранением адреса возврата через `rCL`/`rCD`.
- Программные прерывания (`INT`/`IRET`) с таблицей векторов.
- Порты ввода-вывода (Port-mapped I/O), устройства (Device, Console, QueuedIOStream).
- Безопасный (`LOAD`/`STORE`) и небезопасный (`LOAD_UNSAFE`/`STORE_UNSAFE`) доступ к памяти.
- Опциональная политика обработки процессорных сбоев (`IProcessorFaultPolicy`).

### Компилятор MiniC
- Лексер с поддержкой Unicode-идентификаторов, hex-чисел, char-литералов и escape-последовательностей.
- Парсер рекурсивного спуска (выражения с правильным приоритетом операторов).
- AST с типами-узлами (`VariableNode`, `IfNode`, `WhileNode`, `ForNode`, `FunctionCallNode`, `MemberAccessNode` и др.).
- Поддержка структур (`struct`), указателей, массивов, `new[]`, `&`, `*`.
- Генератор кода с распределением регистров (граф интерференции + жадная раскраска).
- Inline-asm (`asm { ... }`).

### Оптимизаторы
**AST-уровень** (`IAstOptimizationRule`):
- Распространение констант (`PropagateConstantsRule`).
- Свёртка констант (`FoldConstantsRule`).
- Удаление неиспользуемых переменных (`RemoveUnusedVariablesRule`).
- Удаление недостижимого кода до и после инлайна (`RemoveUnreachableCodeRule*`).
- Инлайн маленьких `void`-функций (`InlineSmallVoidFunctionsRule`).

**Peephole-уровень** (`IPeepholeRule`):
- Удаление `MOV r, r`.
- Удаление пар `PUSH`/`POP` одного регистра.
- Удаление `JMP` на следующую метку.
- Свёртка `LDI, LDI, ADD` → `LDI`.
- Замена безопасных инструкций на небезопасные, если адрес известен и корректен.

---

## Архитектура

```
┌──────────────────────────────────────────────────────────────────┐
│                          VMApplication                           │
│   (VMHost, VMHostProject, VMEmulator, DeviceContext, логгеры)    │
├──────────────────────────────────────────────────────────────────┤
│                            Compiller                             │
│  ┌───────────┐  ┌───────────┐  ┌──────────┐  ┌──────────────┐    │
│  │  Lexer    │→ │  Parser   │→ │   AST    │→ │ Оптимизаторы │    │
│  └───────────┘  └───────────┘  └──────────┘  └──────────────┘    │
│                                       │                          │
│                                       ▼                          │
│                              ┌──────────────────┐                │
│                              │ CodeGenerator    │                │
│                              │ (Function/Stmt/  │                │
│                              │  Expression)     │                │
│                              └──────────────────┘                │
│                                       │                          │
│                                       ▼                          │
│                              ┌──────────────────┐                │
│                              │   Assembler      │                │
│                              └──────────────────┘                │
├──────────────────────────────────────────────────────────────────┤
│                              Kernel                              │
│  ┌─────────────┐  ┌──────────────┐  ┌────────────┐  ┌────────┐   │
│  │  Processor  │←→│  MemoryBus   │  │  PortBus   │→ │ Device │   │
│  │  (CPU)      │  │  (RAM+BIOS)  │  │  (Ports)   │  │        │   │
│  └─────────────┘  └──────────────┘  └────────────┘  └────────┘   │
├──────────────────────────────────────────────────────────────────┤
│                        Kernel.Common                             │
│   (OpCode, RegType, RamSize, SizePort, BiosStatus, ошибки...)    │
└──────────────────────────────────────────────────────────────────┘
```

---

## Требования

- **.NET 10.0** (проект использует `net10.0`, `ImplicitUsings`, `Nullable`, C# 13 с `extension`-блоками).
- ОС: Windows / Linux / macOS (x64 или arm64).
- Опционально: `sudo` для повышения приоритета процесса в бенчмарках на Linux/macOS.

---

## Быстрый старт

### Пример 1: ассемблерная программа

```csharp
using Kernel.Common;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Project.IO;

// 1. Создаём хост (компилятор + эмулятор + логгер)
using var host = VMHostFactory.CreateDefault();

// 2. Создаём устройство с RAM 128 МБ и BIOS 128 байт
var device = host.Emulator.CreateDevice(
    ramSize: RamSize.Size128MB,
    biosSize: RamSize.Size128B,
    name: "ConsoleVM"
);

// 3. Подключаем консольное устройство вывода в сектор 1
var iostream = new QueuedIOStream(Console.Out, PortCharEncoding.Utf8);
host.Emulator.AddDevice(device, 0);
host.Emulator.AddDevice(iostream, "console", 1);

// 4. Компилируем ассемблерный код
const string asmSource = """
main:
    LDI  r1, 0x41          // 'A'
    LDI  r2, 0x10          // адрес порта Data
    OUT  r1, r2

    LDI  r1, 1             // команда Flush
    LDI  r2, 0x12
    OUT  r1, r2
    END
""";

var il = host.Project.Compiler.CompileToIL(asmSource, 0, optimize: true, SourceLanguage.Asm);
var compiled = host.Project.Compiler.Compile(il);

// 5. Загружаем в устройство и запускаем
if (compiled.Success)
{
    var launchMode = device.TryFastLoadProgram(compiled.Program!, 0, out var error);
    launchMode?.Launch(showTimer: true);
}
```

### Пример 2: программа на MiniC

```csharp
const string cSource = """
int add(int a, int b) {
    return a + b;
}

int main() {
    int x = 10;
    int y = 32;
    int z = add(x, y);
    return 0;
}
""";

var il = host.Project.Compiler.CompileToIL(cSource, 0, optimize: true, SourceLanguage.C);
var compiled = host.Project.Compiler.Compile(il);
```

### Пример 3: устройство вывода (Console)

Устройство консоли занимает 3 порта (по смещениям):

| Смещение | Имя      | Описание                                                     |
|----------|----------|--------------------------------------------------------------|
| `0x00`   | `Data`   | Запись одного байта в поток вывода                           |
| `0x01`   | `Status` | `0` — очередь пуста, `1` — есть необработанные данные         |
| `0x02`   | `Flush`  | Запись любого значения — дождаться опустошения очереди и flush|

**Кодировки:** `BytePerChar` (Latin-1), `Utf8`, `Utf16`.

Доступны две реализации:
- `SynchronousIOStream` — блокирующая запись (просто, но медленно).
- `QueuedIOStream` — фоновая очередь (не блокирует CPU).

### Пример 4: бенчмарк (TestVMSpeed)

```csharp
using var host = VMHostFactory.CreateDefault(fileService: fileService, outputView: console);
int deviceId = host.Emulator.CreateAndAddDevice(
    bios: [], ramSize: RamSize.Size128MB, biosSize: RamSize.Size128B,
    sector: 0, name: "ConsoleVM"
);

var case_ = new BenchCase("Math", programText, Runs: 30, WarmupRuns: 10);
var runner = new BenchRunner(host, deviceId, case_, fileService);

runner.TryPrepare();
runner.Warmup();
for (int i = 0; i < case_.Runs; i++) runner.RunOnce();

Console.WriteLine($"Peak MIPS: {runner.PeakMips:F2}");
Console.WriteLine($"Steps/run: {runner.StepsOnce:N0}");
```

---

## Система команд (ISA)

Все инструкции — 4 байта. Операнды (64-бит) идут с выравниванием до 8 байт.

### Формат инструкции (32 бита)

| Биты      | Поле       | Описание                          |
|-----------|------------|-----------------------------------|
| 0–7       | `OpCode`   | Код операции (см. ниже)           |
| 8–12      | `Reg1`     | Первый регистр (dst)              |
| 13–17     | `Reg2`     | Второй регистр (src)              |
| 18–19     | `Size`     | Размер данных (`S8`/`S16`/`S32`/`S64`) |
| 20–31     | —          | Зарезервировано                   |

### Основные группы

| Группа           | Инструкции                                                                                  |
|------------------|---------------------------------------------------------------------------------------------|
| Системные        | `NOP`, `END`, `HALT`, `WAKE`, `WAKE_INT`, `RET`, `IRET`, `INT`                               |
| Память           | `MOV`, `LDI`, `LOAD[.S8/16/32/64]`, `STORE[.S8/16/32/64]`, `LOAD_IND`, `STORE_IND`           |
| Память (unsafe)  | `LOAD_UNSAFE`, `STORE_UNSAFE`, `LOAD_IND_UNSAFE`, `STORE_IND_UNSAFE`                        |
| Арифметика       | `ADD`, `SUB`, `MULT_INT`, `DIV`, `SHR`, `INC`, `DEC`                                        |
| Логика           | `AND`, `OR`, `XOR`, `NOT`, `CMP`, `TEST`                                                    |
| Поток управления | `JMP`, `JZ`, `JNZ`, `JG`, `JL`                                                              |
| Стек / вызовы    | `PUSH`, `POP`, `CALL`, `RET`                                                                |
| Ввод-вывод       | `IN`, `OUT`, `PRINT_INT`                                                                    |
| Память (heap)    | `ALLOC`                                                                                     |

### Флаги (`rFL`)

- бит 0 — `ZeroFlag` (ZF)
- бит 1 — `NegativeFlag` (NF)

Обновляются инструкциями `CMP`, `TEST`.

### Условные переходы

| Инструкция | Условие                    |
|------------|----------------------------|
| `JZ`       | `ZF == 1`                  |
| `JNZ`      | `ZF == 0`                  |
| `JL`       | `NF == 1`                  |
| `JG`       | `NF == 0 && ZF == 0`       |

---

## Регистры

| Регистр  | Назначение                                        |
|----------|---------------------------------------------------|
| `r0`–`r3`| Временные (аргументы функций 1–4, возврат в `r0`) |
| `r4`–`r23`| Регистры общего назначения (caller-saved)         |
| `rZ`     | Нулевой регистр (всегда 0)                        |
| `rSP`    | Stack Pointer                                     |
| `rHP`    | Heap Pointer                                      |
| `rIP`    | Instruction Pointer                               |
| `rFL`    | Flags (ZF, NF)                                    |
| `rCL`    | Call Link (адрес возврата текущего вызова)         |
| `rCD`    | Call Depth (глубина вложенности вызовов)           |
| `rTB`    | Table Base (база таблицы векторов прерываний)      |

---

## Модель памяти

- **RAM** + **BIOS** располагаются в едином непрерывном буфере.
- BIOS находится в конце (`_ptrRam + _sizeRam`).
- Начальный `rSP` = верх RAM (выровнен по 8).
- Начальный `rHP` = конец загруженной программы (выровнен по 8).
- **Стек растёт вниз**, **куча — вверх**. При коллизии — `SegmentationFault`.

### Выравнивание

Инструкции `LOAD`/`STORE` для `.S16`, `.S32`, `.S64` требуют выравнивания адреса соответственно на 2, 4, 8 байт. При невыровненном адресе — `AlignmentFault`.

### Безопасный vs небезопасный доступ

- Безопасный доступ проверяет границы памяти и выравнивание.
- `*_UNSAFE` не проверяет ничего — быстрее, но можно выйти за пределы RAM и повредить BIOS.

---

## Порты ввода-вывода

Порты адресуются 64-битным адресом. Разбиение:

```
[ address >> log2(portsPerDevice) ] — сектор (индекс устройства)
[ address &  (portsPerDevice - 1) ] — смещение внутри устройства
```

- `IN Rdest, Rport`  — читает байт из порта по адресу в `Rport`.
- `OUT Rsrc, Rport`  — записывает младший байт `Rsrc` в порт.

`PortBus` регистрирует устройства по секторам. Если устройство не найдено — `NullDeviceInput` / `NullDeviceOutput`.

---

## Язык MiniC

Поддерживаются:

- **Типы:** `int` (S32), `char` (S16), `void`, `byte` (S8), `ushort` (S16), `ulong` (S64), пользовательские `struct`.
- **Указатели:** `int* p`, `&x`, `*p`, `p->field`.
- **Массивы:** `int a[10]`, доступ `a[i]`.
- **Структуры:** `struct Point { int x; int y; };`, доступ `p.x`, `ptr->x`.
- **Управление:** `if/else`, `while`, `for`.
- **Функции:** до 4 аргументов через регистры `r0..r3`, остальные — через псевдоглобалы.
- **Встроенный asm:** `asm { ... }`.
- **Динамическая память:** `new int[n]`, `new Point()`.
- **Препроцессор:** `#include "file"` (asм-файлы).

### Пример

```c
struct Point {
    int x;
    int y;
};

int main() {
    struct Point p;
    p.x = 10;
    p.y = 20;

    int sum = 0;
    for (int i = 0; i < 10; i++) {
        sum = sum + i;
    }
    return 0;
}
```

---

## Оптимизации

### AST-оптимизации

Правила применяются последовательно к `ProgramNode`:

1. `PropagateConstantsRule` — подставляет значения константных переменных в выражения.
2. `FoldConstantsRule` — сворачивает `2 + 3` в `5`.
3. `RemoveUnusedVariablesRule` — удаляет объявления, чьи значения не используются.
4. `RemoveUnreachableCodeRuleBeforeInline` — удаляет код после `return`, схлопывает `if (false)` / `if (true)`.
5. `InlineSmallVoidFunctionsRule` — инлайнит `void`-функции размером ≤ 10 операторов (с переименованием параметров).
6. `RemoveUnreachableCodeRuleAfterInline` — повторно удаляет недостижимый код.

### Peephole-оптимизации

Применяются к списку `AsmItem` (`IRAssembler`) в несколько проходов:

- `RemoveNoopMovRule` — `MOV rX, rX`.
- `RemovePushPopPairRule` — `PUSH rX; POP rX`.
- `RemoveSwapMovPairRule` — `MOV rX, rY; MOV rY, rX`.
- `RemoveJumpToNextLabelRule` — `JMP label` непосредственно перед `label:`.
- `ConstantFoldingRule` — `LDI rX, A; LDI rY, B; ADD rX, rY` → `LDI rX, A+B`.
- `RemoveMultiEndPairRule` — удаляет дублирующиеся `END`.
- `ReplaceSafeMemAccessWithUnsafeRule` — заменяет `LOAD`/`STORE` на `*_UNSAFE` при известном корректном адресе.

---

## Структура решения

```
├── Compiller/                    # Компилятор MiniC
│   ├── ASTNode.cs                # Узлы AST
│   ├── Lexer.cs                  # Лексер
│   ├── Parser.cs                 # Парсер
│   ├── MiniCLanguageDefinition.cs
│   ├── StructLayout.cs
│   ├── CodeGenerator/
│   │   ├── CodeGenUtils.cs
│   │   ├── ExpressionGenerator.cs
│   │   ├── FunctionGenerator.cs
│   │   ├── StatementGenerator.cs
│   │   ├── FunctionContext.cs
│   │   ├── VariableAccessor.cs
│   │   ├── RegisterAllocator.cs
│   │   ├── VarLocation.cs
│   │   ├── GlobalInfo.cs
│   │   └── GlobalMemoryManager.cs
│   └── Optimizators/
│       ├── AstOptimizer.cs
│       └── Rules/
│           ├── PropagateConstantsRule.cs
│           ├── FoldConstantsRule.cs
│           ├── RemoveUnusedVariablesRule.cs
│           ├── RemoveUnreachableCodeRule.cs
│           └── InlineSmallVoidFunctionsRule.cs
│
├── Controllers/ (Kernel)         # Эмулятор
│   ├── BiosSystem/Device.cs
│   ├── ProcessorSystem/Processor.cs
│   ├── RamSystem/ (MemoryBus, NativeMemoryBuffer, RAMResultInt*)
│   ├── ControllersData/PortBus.cs
│   └── Utilites/ (Emulator, ManagerDevices)
│
├── Kernel.Common/                # Общие типы
│   ├── OpCode.cs, RegType.cs, OpCodeSize.cs
│   ├── RamSize.cs, SizePort.cs, BiosStatus.cs
│   ├── InstructionDecoder.cs
│   └── IReadOnlyLogOptimization.cs
│
├── Kernel.Diagnostics/           # Система ошибок
│   ├── DefaultErrorMessageProvider.cs
│   └── ThrowHelper.cs
│
├── Kernel/                       # BIOS-логика (низкоуровневая)
│
├── VMApplication/                # Высокоуровневый API
│   ├── Project/ (VMHostProject, ProjectBuilder, IFileService)
│   ├── Emulator/ (VMEmulator, DeviceContext, LaunchModeDevice)
│   ├── Project/IO/ (SynchronousIOStream, QueuedIOStream)
│   └── Logger/
│
├── TestVMSpeed/                  # Бенчмарки и профилирование
├── ConsoleEmulatorForTests/      # Примеры использования API
└── VM64.sln
```

---

## Сборка и запуск

```bash
# Клонировать репозиторий
git clone https://github.com/<user>/<repo>.git
cd <repo>

# Собрать всё решение
dotnet build VM64.sln -c Release

# Запустить примеры использования API
dotnet run --project ConsoleEmulatorForTests -c Release

# Запустить бенчмарки
dotnet run --project TestVMSpeed -c Release
```

### Результаты бенчмарка

`TestVMSpeed` измеряет MIPS (миллионы инструкций в секунду), время, дисперсию и сохраняет отчёт в `logs/result_Benchmark_Logs_<timestamp>.txt`.

Пример вывода:

```
Test 'Math':
  Performance summary:
      Instructions / run:  30 000 003
      Peak MIPS:                 45.12   (TotalInstr / MinElapsed)
      Stable MIPS (trim10):      43.88   (устойчиво к JIT/GC)
      Median MIPS:               43.95
      ns / instruction:           22.13
      CV (MIPS):                  1.84%
```

---
