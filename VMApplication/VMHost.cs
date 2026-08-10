using VMApplication.Emulator;
using VMApplication.Logger;
using VMApplication.Project;

namespace VMApplication;

public sealed record VMHost(VMHostLogger Logger, VMHostProject Project, VMEmulator Emulator);
