using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Compiller.C;
using Compiller.C.CodeGenerator;
using Compiller.C.Optimizators;
using Kernel.Common;
using Kernel.Contracts;

namespace VMApplication.Project;

public class ProjectBuilder(IFileService? fileService, IProjectFilesConfig? path)
{
    public const int BaseAdressProgram = 0x0;

    private static readonly string[] _defaultAsmExtensions = [".soe", ".asm"];
    private static readonly string[] _defaultMiniCExtensions = [".mic", ".c"];

    private readonly IFileService? fileService = fileService;
    private readonly IProjectFilesConfig? path = path;

    internal IEnumerable<IReadOnlyLogOptimization> Build(IEnumerable<SourceFile> files, bool optimize, AssemblerBase assembler)
    {
        var asmParser = new AssemblerParser();

        var cAsts = new List<ProgramNode>();
        var includes = new List<string>();
        foreach (var file in files.Where(file => file.Language == SourceLanguage.C))
        {
            var lexer = new Lexer(file.Content);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            cAsts.Add(ast);
            includes.AddRange(ast.IncludesList);
        }

        var combinedAst = new ProgramNode();
        foreach (var ast in cAsts)
        {
            combinedAst.FunctionNodes.AddRange(ast.FunctionNodes);
            combinedAst.GlobalVarNodes.AddRange(ast.GlobalVarNodes);
            combinedAst.StructNodes.AddRange(ast.StructNodes);
        }

        IEnumerable<IReadOnlyLogOptimization> resLog = 
            optimize ? AstOptimizer.Optimize(combinedAst) 
            : [];


        var allStructDecls = new List<StructDeclNode>();
        foreach (var ast in cAsts)
            allStructDecls.AddRange(ast.StructNodes);

        var structLayouts = StructLayout.Resolve(allStructDecls);

        var mainFunc = combinedAst.FunctionNodes.FirstOrDefault(f => f.Name == "main");
        if (mainFunc != null)
        {
            combinedAst.FunctionNodes.Remove(mainFunc);
            combinedAst.FunctionNodes.Insert(0, mainFunc);
        }

        if (combinedAst.FunctionNodes.Any(f => f.Name == "main"))
        {
            assembler.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), "func_main");
        }

        AddInclude(includes, asmParser, assembler);

        foreach (var file in files.Where(file => file.Language == SourceLanguage.Asm))
        {
            asmParser.Assemble(file.Content, assembler);
        }

        var funcGen = new FunctionGenerator(assembler, structLayouts);
        funcGen.Generate(combinedAst);

        assembler.EmitInstruction(InstructionEncoder.EncodeEND());

        return resLog;
    }

    private void AddInclude(IEnumerable<string> includes, AssemblerParser asmParser, AssemblerBase assembler)
    {
        if (fileService == null || path == null) return;
        foreach (string inc in includes)
        {
            string localPath = fileService.CombinePath(path.ProjectPath, inc);
            string sharedPath = fileService.CombinePath(path.IncludePath, inc);

            string? chosenPath = null;
            if (fileService.Exist(localPath))
                chosenPath = localPath;
            else if (fileService.Exist(sharedPath))
                chosenPath = sharedPath;

            if (chosenPath == null)
                throw new Exception($"Included file not found: '{inc}'. Searched in project folder and in '{path.IncludePath}'.");

            string asmCode = fileService.ReadFile(chosenPath);
            asmParser.Assemble(asmCode, assembler);
        }
    }

    public IEnumerable<IReadOnlyLogOptimization> BuildProject(bool optimize, AssemblerBase assembler)
    {
        if (fileService == null || path == null) return [];

        var files = new List<SourceFile>();

        foreach (string fileName in fileService.GetSourceFiles())
        {
            SourceLanguage lang = FindLang(path, fileName);
            if (lang == SourceLanguage.None) continue;

            string source = fileService.ReadFile(fileName);

            files.Add(new SourceFile(fileName, source, lang));
        }

        return Build(files, optimize, assembler);
    }

    private static bool IsAsm(IProjectFilesConfig config, string name)
    {
        var extensions = config.ExtensionsAsm;
        if (extensions.Length == 0)
            extensions = _defaultAsmExtensions;

        return HasExtension(extensions, Path.GetExtension(name));
    }

    private static bool IsMiniC(IProjectFilesConfig config, string name)
    {
        var extensions = config.ExtensionsMiniC;
        if (extensions.Length == 0)
            extensions = _defaultMiniCExtensions;

        return HasExtension(extensions, Path.GetExtension(name));
    }

    private static bool HasExtension(string[] extensions, string fileExt)
    {
        foreach (var ext in extensions)
        {
            if (string.Equals(ext, fileExt, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    private static SourceLanguage FindLang(IProjectFilesConfig config, string name)
    {
        if (IsAsm(config, name)) return SourceLanguage.Asm;
        else if (IsMiniC(config, name)) return SourceLanguage.C;
        else return SourceLanguage.None;
    }
}

