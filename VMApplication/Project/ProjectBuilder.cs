using Compiller.ASM;
using Compiller.ASM.Optimizators;
using Compiller.C;
using Compiller.C.CodeGenerator;
using Compiller.C.Optimizators;
using Kernel.Common;

namespace VMApplication.Project;

public class ProjectBuilder(IFileService fileService, IProjectFilesConfig path)
{
    public const int BaseAdressProgram = 0x0;

    private static readonly string[] _defaultAsmExtensions = [".soe", ".asm"];
    private static readonly string[] _defaultMiniCExtensions = [".mic", ".c"];

    private readonly IFileService fileService = fileService;
    private readonly IProjectFilesConfig path = path;

    private string[]? _cachedAsmExtensionArray;
    private string[]? _cachedMiniCExtensionArray;
    private HashSet<string>? _cachedAsmExtensionHashSet;
    private HashSet<string>? _cachedMiniCExtensionHashSet;

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
            includes.AddRange(ast.Includes);
        }

        // 2. Объединяем C-функции и глобальные переменные в один AST
        var combinedAst = new ProgramNode();
        foreach (var ast in cAsts)
        {
            combinedAst.Functions.AddRange(ast.Functions);
            combinedAst.Globals.AddRange(ast.Globals);
            combinedAst.Structs.AddRange(ast.Structs);
        }
        // Оптимизация AST перед кодогенерацией
        IEnumerable<IReadOnlyLogOptimization> resLog = 
            optimize ? AstOptimizer.Optimize(combinedAst) 
            : [];


        var allStructDecls = new List<StructDeclNode>();
        foreach (var ast in cAsts)
            allStructDecls.AddRange(ast.Structs);

        var structLayouts = StructLayout.Resolve(allStructDecls);

        // Перемещаем main в начало
        var mainFunc = combinedAst.Functions.FirstOrDefault(f => f.Name == "main");
        if (mainFunc != null)
        {
            combinedAst.Functions.Remove(mainFunc);
            combinedAst.Functions.Insert(0, mainFunc);
        }

        // 5. Теперь, когда все asm-метки известны, вставляем стартовый код
        if (combinedAst.Functions.Any(f => f.Name == "main"))
        {
            assembler.EmitJump(InstructionEncoder.EncodeJ(OpCode.JMP.Uint), "func_main");
        }

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

        foreach (var file in files.Where(file => file.Language == SourceLanguage.Asm))
        {
            asmParser.Assemble(file.Content, assembler);
        }

        var funcGen = new FunctionGenerator(assembler, structLayouts);
        funcGen.Generate(combinedAst);

        assembler.EmitInstruction(InstructionEncoder.EncodeEND());

        return resLog;
    }

    public IEnumerable<IReadOnlyLogOptimization> BuildProject(bool optimize, AssemblerBase assembler)
    {
        var files = new List<SourceFile>();

        foreach (string fileName in fileService.GetSourceFiles())
        {
            // Пытаемся получить текст из открытой вкладки
            string source = fileService.ReadFile(fileName);

            SourceLanguage lang = FindLang(path, fileName);
            files.Add(new SourceFile(fileName, source, lang));
        }

        // Вызываем существующий метод Build с базовым адресом (можно параметризовать)
        return Build(files, optimize, assembler);
    }

    private void CacheAsmExtensions(string[] ext)
    {
        if (!ReferenceEquals(ext, _cachedAsmExtensionArray))
        {
            _cachedAsmExtensionArray = ext;
            _cachedAsmExtensionHashSet = new HashSet<string>(ext, StringComparer.OrdinalIgnoreCase);
        }
    }

    private void CacheMiniCExtensions(string[] ext)
    {
        if (!ReferenceEquals(ext, _cachedMiniCExtensionArray))
        {
            _cachedMiniCExtensionArray = ext;
            _cachedMiniCExtensionHashSet = new HashSet<string>(ext, StringComparer.OrdinalIgnoreCase);
        }
    }

    private bool IsAsm(IProjectFilesConfig config, string name)
    {
        string[] extensions = config.ExtensionsAsm;
        if (extensions.Length == 0)
            extensions = _defaultAsmExtensions; // если нужно преобразовать ImmutableArray в массив

        CacheAsmExtensions(extensions);
        string fileExt = Path.GetExtension(name);
        return _cachedAsmExtensionHashSet!.Contains(fileExt);
    }

    private bool IsMiniC(IProjectFilesConfig config, string name)
    {
        string[] extensions = config.ExtensionsMiniC;
        if (extensions.Length == 0)
            extensions = _defaultMiniCExtensions;

        CacheMiniCExtensions(extensions);
        string fileExt = Path.GetExtension(name);
        return _cachedMiniCExtensionHashSet!.Contains(fileExt);
    }
    private SourceLanguage FindLang(IProjectFilesConfig config, string name)
    {
        if (IsAsm(config, name)) return SourceLanguage.Asm;
        else if (IsMiniC(config, name)) return SourceLanguage.C;
        else return SourceLanguage.None;
    }
}

