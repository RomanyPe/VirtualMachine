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
