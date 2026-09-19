using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Compiller.ASM.Optimizators.Rules;
using Compiller.C;
using Kernel.Common;
using System.Text.RegularExpressions;
using VMApplication.Logger;

namespace VMApplication.Project;

public sealed class VMHostProjectCompiler(
    IProjectFilesConfig paths,
    VMHostLogger logger,
    IFileService fileService)
{
    private static readonly ReplaceSafeMemAccessWithUnsafeRule _rule;
    private readonly VMHostLogger _logger = logger;
    private readonly ProjectBuilder _projectBuilder = new(fileService, paths);

    static VMHostProjectCompiler()
    {
        _rule = new();
        PeepholeOptimizer.AddRule(_rule);

    }

    public static Regex AsmMnemonics => AsmLanguageDefinition.GetMnemonicRegex();
    public static Regex AsmRegisters => AsmLanguageDefinition.GetRegisterRegex();
    public static Regex MiniCMnemonics => MiniCLanguageDefinition.GetKeywordRegex();

    public CompilationToILResult CompileToIL(ulong baseAddress, bool optimize)
    {
        try
        {
            IRAssembler assembler = new(baseAddress);
            var resLog = _projectBuilder.BuildProject(optimize, assembler);

            OptimizationResultLog? res;
            if (optimize)
            {
                res = new();
                res.AddLog(resLog);
                
            }
            else
            {
                res = null;
            }
            return new CompilationToILResult(assembler, res);
        }
        catch (Exception ex)
        {
            _logger.AppendLine(ex.Message, LogLevel.Error);
            return new CompilationToILResult(null, null);
        }
    }

    public CompilationResult Compile(ulong baseAddress, bool optimize)
    {
        try
        {
            AssemblerBase assembler = new Assembler(baseAddress);
            var resLog = _projectBuilder.BuildProject(optimize,assembler);

            OptimizationResultLog? res;
            if (optimize)
            {
                res = new();
                res.AddLog(resLog);
            }
            else
            {
                res = null;
            }

            return new CompilationResult(assembler.Build(), baseAddress, null, res);
        }
        catch (Exception ex)
        {
            _logger.AppendLine(ex.Message, LogLevel.Error);
            return new CompilationResult(null, 0, [ex.Message], null);
        }
    }

    public CompilationResult Compile(CompilationToILResult res)
    {
        try
        {
            var asm = res.Assembler;
            return asm != null
                ? new CompilationResult(asm.Build(), asm.BaseAddress, null, res.OptimizationResultLog)
                : new CompilationResult(null, 0, ["Unkown error, assembler is null"], null);
        }
        catch (Exception ex)
        {
            _logger.AppendLine(ex.Message, LogLevel.Error);
            return new CompilationResult(null, 0, [ex.Message], null);
        }
    }

    public CompilationResult Compile(CompilationToILResult res, int peepholeOptimizationCount, RamSize size = RamSize.Size128KB)
    {
        try
        {
            var asm = res.Assembler;
            if (asm != null)
            {
                var log = new PeepholeLog();
                var peep = asm.GetItems();
                _rule.RamSize = size;
                PeepholeOptimizer.Optimize(peep, log, peepholeOptimizationCount);
                var peepholeLog = new PeepholeOptimizationLogs(log);
                var logger = res.OptimizationResultLog;
                logger ??= new();
                logger.AddLog(peepholeLog);

                return new CompilationResult(asm.Build(), asm.BaseAddress, null, logger);
            }
            return new CompilationResult(null, 0, ["Unkown error, assembler is null"], null);

        }
        catch (Exception ex)
        {
            _logger.AppendLine(ex.Message, LogLevel.Error);
            return new CompilationResult(null, 0, [ex.Message], null);
        }
    }
}