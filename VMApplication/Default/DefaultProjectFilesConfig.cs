using VMApplication.Project;

namespace VMApplication.Default;

public sealed class DefaultProjectFilesConfig : IProjectFilesConfig
{
    public string ProjectPath { get; init; } = AppDomain.CurrentDomain.BaseDirectory;
    public string IncludePath { get; init; } = "include";
    public string[] ExtensionsAsm { get; init; } = [".asm", ".vma"];
    public string[] ExtensionsMiniC { get; init; } = [".c", ".mic"];
}
