using Kernel.Common;
using VMApplication;
using VMApplication.Emulator;
using VMApplication.Project;

namespace ExtensionsVMApplication.Host;

public sealed record RunOutcome(
    DeviceContext Device,
    int DeviceId,
    CompilationResult Compilation,
    long StepCount,
    bool Success,
    IReadOnlyList<string> Diagnostics);



public static class VMHostRunExtensions
{
    extension(VMHost host)
    {
        /// Запускает и оставляет device живым — можно читать память, регистры, порты.
        public RunOutcome RunAndKeep(
            string source,
            SourceLanguage lang,
            RamSize? ram = null,
            bool optimize = false,
            string deviceName = "run")
        {
            ram ??= RamSize.MB16;
            var il = host.Project.CompileToIL(source, 0, optimize, lang);
            var comp = host.Project.Compile(il);

            if (!comp.Success)
                return new(null!, -1, comp, 0, false, comp.Errors ?? []);
            // device пока не создавали — возвращать нечего

            int id = host.Emulator.CreateAndAddDevice(ram.Value, sector: 0, name: deviceName);
            var ctx = host.Emulator.GetDeviceContext(id)!;

            if (!ctx.TryFastLoadProgram(comp.Program!, 0, out var err))
            {
                host.Emulator.RemoveDevice(id);
                return new(null!, -1, comp, 0, false, [err!]);
            }

            ctx.Launch(new LaunchOptions(StartAddress: 0));
            return new(ctx, id, comp, ctx.StepCount, true, []);
        }

        /// Запускает и убирает за собой. Ничего не возвращает — только факт успеха.
        public bool RunAndCleanup(
            string source,
            SourceLanguage lang,
            RamSize? ram = null,
            bool optimize = false)
        {
            ram ??= RamSize.MB16;

            var res = host.RunAndKeep(source, lang, ram, optimize);
            if (res.Device is not null) host.Emulator.RemoveDevice(res.DeviceId);
            return res.Success;
        }
    }
}