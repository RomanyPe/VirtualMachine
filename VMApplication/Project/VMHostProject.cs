using Compiller.ASM.Optimizators;
using VMApplication.Logger;

namespace VMApplication.Project;

public readonly struct CompilationToILResult
{
    internal readonly IRAssembler? Assembler;
    internal OptimizationResultLog? OptimizationResultLog { get; }
    internal CompilationToILResult(IRAssembler? assembler, OptimizationResultLog? optimizationResultLog)
    {
        Assembler = assembler;
        OptimizationResultLog = optimizationResultLog;
    }
}

public sealed class VMHostProject(IProjectFilesConfig paths, VMHostLogger logger, IFileService fileService)
{
    private readonly IProjectFilesConfig _projectPaths = paths;
    private readonly IFileService _fileService = fileService;
    private readonly VMHostProjectCompiler _compiler = new(paths, logger, fileService);
    public VMHostProjectCompiler Compiler => _compiler;

    public string IncludePath => _projectPaths.IncludePath;

    public IFileService FileService => _fileService;
    public IProjectFilesConfig ProjectFilesConfig => _projectPaths;

    public CompilationToILResult CompileToIL(ulong baseAddress, bool optimize)
           => _compiler.CompileToIL(baseAddress, optimize);
    public CompilationResult Compile(ulong baseAddress, bool optimize)
        => _compiler.Compile(baseAddress, optimize);
    public CompilationResult Compile(CompilationToILResult res)
        => _compiler.Compile(res);
    public CompilationResult Compile(CompilationToILResult res, int peepholeOptimizationCount)
        => _compiler.Compile(res, peepholeOptimizationCount);
    public CompilationToILResult CompileToIL(IEnumerable<SourceFile> files, ulong baseAddress, bool optimize)
    => _compiler.CompileToIL(files, baseAddress, optimize);
    public CompilationToILResult CompileToIL(string source, ulong baseAddress, bool optimize, SourceLanguage language = SourceLanguage.C)
        => _compiler.CompileToIL(source, baseAddress, optimize, language);
    public CompilationToILResult CompileToIL(IEnumerable<string> sources, ulong baseAddress, bool optimize, SourceLanguage language = SourceLanguage.C)
        => _compiler.CompileToIL(sources, baseAddress, optimize, language);
    public CompilationResult Compile(IEnumerable<SourceFile> files, ulong baseAddress, bool optimize)
        => _compiler.Compile(files, baseAddress, optimize);
    public CompilationResult Compile(string source, ulong baseAddress, bool optimize, SourceLanguage language = SourceLanguage.C)
        => _compiler.Compile(source, baseAddress, optimize, language);
    public CompilationResult Compile(IEnumerable<string> sources, ulong baseAddress, bool optimize, SourceLanguage language = SourceLanguage.C)
        => _compiler.Compile(sources, baseAddress, optimize, language);
}
