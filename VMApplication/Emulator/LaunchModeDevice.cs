using Kernel.BiosSystem;
using Kernel.Contracts;
using VMApplication.Project;

namespace VMApplication.Emulator;

public record class LaunchOptions(ulong StartAddress = ProjectBuilder.ZeroAdressProgram,
                             bool Debug = false,
                             int DelayMs = 0,
                             bool ShowTimer = false,
                             Action<IDeviceLoggerContext>? OnLaunch = null,
                             Action<IDeviceLoggerContext>? OnStart = null,
                             Action<IDeviceLoggerContext, ISimulationResult?>? OnEnd = null)
{ 
    public static LaunchOptions Default { get; } = new();
}