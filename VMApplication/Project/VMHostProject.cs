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
