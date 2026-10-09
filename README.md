# VMA — Virtual Machine & Emulator Platform

!ВАЖНО! Статус: платформа/тулкит, не фреймворк. API не гарантирует semver.

Платформа для сборки, компиляции и исполнения виртуальных машин с собственным байт-кодом,
компилятором C-подобного языка (MiniC) и ассемблером (VMA), плюс секторно-адресуемая шина
портов для IO и меж-устройственного взаимодействия.

Название «Kernel» в проектах `Kernel.*` — историческое. По сути это **общие (публичные)
контракты и примитивы** платформы, а не ядро. Настоящее ядро-исполнитель живёт в проекте
`Kernel` и наружу не выставляется.

---

# Слои:
```
┌────────────────────────────────────────────────────────────────────┐
│  Хосты и тесты                                                     │
│  ConsoleEmulatorForTests, TestVMSpeed                              │
└────────────────────────────────────────────────────────────────────┘
                              │
┌────────────────────────────────────────────────────────────────────┐
│  ExtensionsVMApplication                                           │
│  Готовые peephole-правила, AST-правила, IO-устройства, host-helpers│
└────────────────────────────────────────────────────────────────────┘
                              │
┌────────────────────────────────────────────────────────────────────┐
│  VMApplication                                                     │
│  VMHost, VMHostFactory, VMEmulator, DeviceContext, ProjectBuilder, │
│  компилятор-фасад, оптимизатор-фасады, IFileService, IOutputView   │
└────────────────────────────────────────────────────────────────────┘
                              │
┌────────────────────────────────────────────────────────────────────┐
│  Kernel                       │  Compiller                         │
│  Processor, Device,           │  Lexer, Parser, CodeGen,           │
│  MemoryBus, PortBus,          │  Assembler, Disassembler,          │
│  ManagerDevices, Emulator     │  AstOptimizer, PeepholeOptimizer   │
└────────────────────────────────────────────────────────────────────┘
                              │
┌────────────────────────────────────────────────────────────────────┐
│  Kernel.Common                │  Kernel.Contracts                  │
│  RegType, OpCode, RamSize,    │  IPortController, IProcessorReader,│
│  PortSize, BiosStatus,        │  IProcessorFaultPolicy,            │
│  LogLevel, InstructionDecoder │  IDeviceLoggerContext,             │
│                               │  ISimulationResult, OptimizationId │
└────────────────────────────────────────────────────────────────────┘
```

`Kernel.Common` и `Kernel.Contracts` — публичные. Они специально выделены в отдельные проекты, чтобы истинное ядро (`Kernel`) не утекало наружу.
Ссылаться на них — нормально.

---

# Бытсрый старт

```csharp
using VMApplication;
using VMApplication.Project;

const string source =
"""
main:
    LDI  r1, 0x41          // 'A'
    LDI  r2, 0x10          // адрес порта Data
    OUT  r1, r2

    LDI  r1, 1
    LDI  r2, 0x12          // Flush
    OUT  r1, r2

    END
""";

using var host = VMHostFactory.CreateDefault();

var il   = host.Project.Compiler.CompileToIL(source, 0, optimize: true, SourceLanguage.Asm);
var comp = host.Project.Compiler.Compile(il);

if (!comp.Success)
{
    foreach (var err in comp.Errors!) Console.WriteLine(err);
    return;
}

int id  = host.Emulator.CreateAndAddDevice(RamSize.MB64, sector: 0, name: "vm");
var ctx = host.Emulator.GetDeviceContext(id)!;

ctx.TryFastLoadProgram(comp.Program!, 0, out var error);
ctx.Run();
// Dispose у host сам уберёт устройство.
```

# Работа платформы

## Что такое `VMHost`
`VMHost` — это фасад всей ВМ. Он объединяет три подсистемы:

 - `VMHostProject` — компилятор (`CompileToIL`, `Compile`) и файловый сервис.
 - `VMEmulator` — реестр устройств, `PortBus`, создание/удаление `Device`.
 - `VMHostLogger` — единая точка логов.

## Управление жизненным циклом

- VMHost реализует IDisposable. При Dispose он сам убирает все устройства и освобождает их память.
- Если хост и устройство живут одинаково долго — не нужно вызывать RemoveDevice вручную.
- Не рекомендуется вызывать Dispose у DeviceContext вручную, если устройство принадлежит хосту. 
Это приведёт либо к двойному Dispose (если ваш класс это не защищает), либо к попытке хоста убрать 
уже освобождённое устройство.

### ВАЖНО!

Метод `VMHostFactory.CreateDefault()` создает хоста с стандартными сервисами (они подходят для в основном только для простых задач), если нужно более точная настройка
используйте билдеры.

Этот метод показан как пример проверки работоспособности с минимальным возможным кодом. Но лучше использовать свои сервисы.

```csharp
using var host = VMHostFactory.CreateDefault();
int id = host.Emulator.CreateAndAddDevice(RamSize.MB64, sector: 0);
// ... работа ...
// Dispose у host сделает всё сам.
```
Если устройство нужно убрать раньше хоста — используйте `host.Emulator.RemoveDevice(id)`.

## Компилятор

Компилятор встроен в `VMApplication` и вызывается через `host.Project.Compiler`:

```csharp
CompilationToILResult il = host.Project.Compiler.CompileToIL(source, 0, optimize: true, SourceLanguage.C);
CompilationResult res = host.Project.Compiler.Compile(il);
```

### Поддерживается:

- MiniC - C-подобный язык (`int`, `char`, `void`, `byte`, `ushort`, `ulong`, `struct`, `if`, `while`, `for`, `return`, `asm`, `extern`
, указатели, массивы).
- VMA — ассемблер (`LDI`, `MOV`, `ADD`, `LOAD`, `STORE`, `JMP`, `CALL`, `IN`, `OUT`, `HALT` и т.д.).
- **Inline asm** через `asm { ... }`.

### Важно про оптимизаторы

Оптимизаторы не заложены «**в коробку**». Встроенный компилятор работает без них. Чтобы включить:

- подключите `ExtensionsVMApplication` и вызовите `AddStdRules()` для AST- и peephole-оптимизаторов, либо
- напишите свои правила (`IAstOptimizationRule`, `IPeepholeRule`).

```csharp
using ExtensionsVMApplication.Optimizators;

host.Project.Compiler.Optimizator
    .VMAstOptimizer.AddStdRules();

host.Project.Compiler.Optimizator
    .VMPeepholeOptimizer.AddStdRules();
```

### Ограничения кодогенерации

Кодогенератор сознательно простой и не претендует на качество промышленных компиляторов: 
слабое распределение регистров, 
консервативный inline, 
ограниченная поддержка указателей и структур. 
Если критична скорость готового байт-кода — либо пишите критичные участки на VMA напрямую, 
либо используйте `optimize: true` если вы подключили расширения, в ином случае без расширений оптимизаций никаких не будет.

## IO и шина портов

Ограничений на IO нет: реализуйте `IPortController` (или унаследуйтесь от `PortControlBase`) — и всё.

### Рекомендация: `PortControlBase`, а не `IPortController`

- Наследуйтесь от PortControlBase, если вам нужна защита от двойного Dispose — она уже реализована.
- Реализуйте `IPortController` напрямую только если обязаны наследоваться от другого класса
(C# не даёт множественного наследования).

```
using VMApplication.Emulator.Abstraction;

public sealed class MyDevice : PortControlBase
{
    public override byte ReadPort(ulong offset) => 0;
    public override void WritePort(ulong offset, byte value) { /* ... */ }
    public override void WakeProcessor() { /* ... */ }

    // При необходимости:
    protected override void OnDispose() { /* освободить ресурсы */ }
}
```

### Как работает `PortBus`

`PortBus` — линейно-блочный (секторный) маршрутизатор портов. Адрес разбивается 
на сектор (`address >> deviceShift`) и смещение внутри устройства (`address & offsetMask`). 
Реализовано через битовые сдвиги и маски — размеры блоков всегда кратны степеням двойки, 
это даёт предсказуемую и быструю адресацию.

- Помимо связи CPU ↔ устройство, `PortBus` позволяет устройствам общаться друг с другом — регистрируйте 
контроллеры в разных секторах и читайте/пишите в чужие адреса.
- `PortSize` — размер всего адресного пространства портов (степень 2, от 64 B до 512 KiB).
- `DevicePortSize` — размер блока одного устройства (степень 2, от 8 B до 128 B).
Оба типа — публичные value-типы, `default(T)` невалиден.

## `Kernel` — прямой доступ к интерпретатору

Проект `Kernel` — это минимальный возможный уровень работы с интерпретатором, памятью, IO и контрактами. 
Он не предназначен для обычного использования. 
Чтобы воспользоваться им напрямую, нужно явно указать:
```
using Kernel;
```

### `Device` — интерпретатор

Если быть точным то интерпритатор это именно `Processor`, а не сам `Device`, но вы не сможете использовать его без 
`Device`, так что относительно правильно считать именно `Device` интерпритатором.

- Хорошая производительность; инструкции работы с памятью сравнимы по скорости с арифметикой.
- Точные цифры на вашем железе — в проекте `TestVMSpeed` 
(12 кейсов: `Memory`, `Math`, `ALU`, `Call`, `PushPop`, `BitOps`, `Mul`, `Branch`).
- `Device` всегда выделяет нативную память под ОЗУ. Это позволяет создавать ВМ практически любого размера. 
Сейчас разумный предел — ~2 GiB; расширение до 16 GiB технически возможно, 
но редко осмысленно (не на каждой машине столько ОЗУ можно выделить только под эмулятор).
- ОЗУ полностью инкапсулировано внутри `Device`.
Даже `INativeReadOnlyBuffer` — это отдельный фасад-класс; 
никакие приведения не дадут записать в ОЗУ в обход API.

### Безопасность

Использование `Kernel` напрямую — это небезопасный сценарий. 
Вы работаете с низкоуровневой памятью, регистрами и портами без страховок, 
которые даёт `VMApplication`. Для обычных задач используйте `VMApplication` + `ExtensionsVMApplication`.

---

# Рекомендации

1. Не вызывайте `Dispose` у `DeviceContext` вручную, если устройство принадлежит `VMHost`. Пусть хост сделает это сам.
2. Наследуйтесь от `PortControlBase`, а не реализуйте `IPortController` вручную — иначе сами отвечаете за идемпотентность Dispose.
3. Не забывайте подключать оптимизаторы — иначе соберёте медленный байт-код, хотя AST/peephole-пассы готовы в `ExtensionsVMApplication`.
4. Критичные по скорости участки пишите сразу на VMA или инлайните через `asm { }` — кодогенератор слабоват.
5. Размеры портов (`PortSize`, `DevicePortSize`) — только из статических фабрик (`PortSize.KB64`, `DevicePortSize.B16`); `default` невалиден.
6. Файловый сервис (`IFileService`) и конфиг путей (`IProjectFilesConfig`) — подменяемые; удобно для тестов и `in-memory` сборок (`RamFileService` в комплекте).
7. Логи идут через `VMHostLogger` → `IOutputView`. Для «тихой» работы используйте `NullOutputView.Instance`.

## Проекты решения

| Проект | Назначение |
|-------------|-------------|
| `Kernel.Common` | Публичные примитивы: `RegType`, `OpCode`, `OpCodeSize`, `RamSize`, `PortSize`, `DevicePortSize`, `BiosStatus`, `LogLevel`, `InstructionDecoder`, `AlignmentExtensions`.|
| `Kernel.Contracts` | Публичные контракты: `IPortController`/`IPortUse`, `IPortController`, `IProcessorReader`, `IProcessorFaultPolicy`, `IDeviceLoggerContext`, `INativeReadOnlyBuffer`, `ISimulationResult`, `OptimizationId`, `IReadOnlyLogOptimization`.|
| `Kernel` | Реализация: `Processor`, `Device`, `MemoryBus`, `NativeMemoryBuffer`, `PortBus`, `ManagerDevices`, `Emulator`. Небезопасный, прямой доступ. |
| `Kernel.Diagnostics` | `ErrorCode`, `IErrorMessageProvider`, `DefaultErrorMessageProvider`, `ThrowHelper`, `CodeExpection`. |
| `Compiller` | Лексер, парсер, AST, кодогенератор, ассемблер/дизассемблер, платформенных оптимизаторов. |
| `VMApplication` | Фасад: `VMHost`, `VMHostFactory`, `VMEmulator`, `DeviceContext`, `ProjectBuilder`, `VMHostProjectCompiler`, `IFileService`, `IOutputView`, билдеры. |
| `ExtensionsVMApplication` | Готовые AST- и peephole-правила, `PortControlBase`-совместимые IO-устройства (`QueuedIOStream`, `SynchronousIOStream`), host-helpers (`RunAndKeep`, `RunOnce`, `RunAndCleanup`). |
| `ConsoleEmulatorForTests` | Консольный пример запуска. |
| `TestVMSpeed` | Бенчмарк интерпретатора (warmup + прогоны, статистика, дизассемблер). |

---

## Известные ограничения

- Кодогенератор MiniC: базовое распределение регистров, ограниченная поддержка указателей/структур в параметрах функций.
- Оптимизаторы нужно подключать вручную.
- `TestVMSpeed` и `ExtensionsVMApplication` таргетят net10.0; `Compiller` и `Kernel.`* — net8.0; `VMApplication` — net8.0;net10.0.
- Прямое использование `Kernel` небезопасно и не рекомендуется без веской причины.