using VMApplication.Emulator;
using VMApplication.Logger;
using VMApplication.Project;

namespace VMApplication;

public sealed record VMHost(VMHostProject Project, VMEmulator Emulator, VMHostLogger Logger) : IDisposable
{
    public void Dispose() => Emulator.Dispose();
}