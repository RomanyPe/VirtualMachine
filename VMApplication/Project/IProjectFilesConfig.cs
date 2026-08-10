namespace VMApplication.Project;

public interface IProjectFilesConfig
{
    string ProjectPath { get; }
    string IncludePath { get; }
    string[] ExtensionsAsm { get; }
    string[] ExtensionsMiniC { get; }
}

