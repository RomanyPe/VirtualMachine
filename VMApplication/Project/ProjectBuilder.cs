using Compiller.ASM;
using Compiller.C;
using Compiller.C.CodeGenerator;
using Compiller.C.Optimizators;
using Kernel.Common;

namespace VMApplication.Project;

public static class ProjectBuilder
{
    private static readonly string[] DefaultAsmExtensions = [".soe", ".asm"];
    private static readonly string[] DefaultCExtensions = [".mic", ".c"];

    public const int BaseAdressProgramm = 0x0;

    internal static (byte[] ByteCode, OutPutOptimizeText ResLog) Build(List<SourceFile> files, IProjectFilesConfig path, ulong baseAddress, bool optimize)
    {
        var assembler = new Assembler(baseAddress: baseAddress);
        var asmParser = new AssemblerParser();

        var cAsts = new List<ProgramNode>();
        var includes = new List<string>();

        // 1. Разбираем все C‑файлы, собираем их AST и списки include
        foreach (var file in files)
        {
            if (file.Language == SourceLanguage.C)
            {
                var lexer = new Lexer(file.Content);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens);
                var ast = parser.Parse();
                cAsts.Add(ast);
                includes.AddRange(ast.Includes);
            }
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
        OutPutOptimizeText resLog = default;

        if (optimize)
            resLog = AstOptimizer.Optimize(combinedAst);

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

        // 3. Сначала добавляем библиотеки из #include (они могут содержать init_vectors)
        foreach (string inc in includes)
        {
            string localPath = Path.Combine(path.ProjectPath, inc);
            string sharedPath = Path.Combine(path.IncludePath, inc);

            string? chosenPath = null;
            if (File.Exists(localPath))
                chosenPath = localPath;
            else if (File.Exists(sharedPath))
                chosenPath = sharedPath;

            if (chosenPath == null)
                throw new Exception($"Included file not found: '{inc}'. Searched in project folder and in '{path.IncludePath}'.");

            string asmCode = File.ReadAllText(chosenPath);
            asmParser.Assemble(asmCode, assembler);
        }

        // 4. Добавляем обычные ассемблерные файлы проекта (если есть)
        foreach (var file in files)
        {
            if (file.Language == SourceLanguage.Asm)
            {
                asmParser.Assemble(file.Content, assembler);
            }
        }

        // 6. Генерируем код всех C‑функций (определит метку func_main)
        var funcGen = new FunctionGenerator(assembler, structLayouts);
        funcGen.Generate(combinedAst);

        // 7. Один завершающий HALT
        assembler.EmitInstruction(InstructionEncoder.EncodeHALT());

        return (assembler.Build(), resLog);
    }

    internal static(byte[] ByteCode, OutPutOptimizeText ResLog) BuildProject(IFileService fileService, IEditorService editorService,IProjectFilesConfig paths, ulong baseAddress, bool optimize)
    {
        var files = new List<SourceFile>();

        foreach (string fileName in fileService.GetSourceFiles())
        {
            // Пытаемся получить текст из открытой вкладки
            string source = editorService.GetText(fileName) ?? fileService.ReadFile(fileName);

            SourceLanguage lang = FindLang(paths, fileName);
            files.Add(new SourceFile(fileName, source, lang));
        }

        // Вызываем существующий метод Build с базовым адресом (можно параметризовать)
        return Build(files, paths, baseAddress: baseAddress, optimize);
    }

    private static bool Has(string[] extens, string name) 
    {
        foreach (var ext in extens)
            if (name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    private static bool IsAsm(IProjectFilesConfig config, string name)
    {
        var extens = config.ExtensionsAsm;
        extens ??= DefaultAsmExtensions;
        return Has(extens, name);
    }

    private static bool IsMiniC(IProjectFilesConfig config, string name)
    {
        var extens = config.ExtensionsMiniC;
        extens ??= DefaultCExtensions;
        return Has(extens, name);
    }

    private static SourceLanguage FindLang(IProjectFilesConfig config, string name)
    {
        if (IsAsm(config, name)) return SourceLanguage.Asm;
        else if (IsMiniC(config, name)) return SourceLanguage.C;
        else return SourceLanguage.None;
    }
}

