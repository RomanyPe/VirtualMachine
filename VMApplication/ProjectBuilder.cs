using Compiller.ASM;
using Compiller.C;
using Compiller.C.CodeGenerator;
using static Kernel.ProcessorSystem.Processor;

namespace VMApplication;

public static class ProjectBuilder
{
    public const int BaseAdressProgramm = 0x0;

    public static byte[] Build(IEnumerable<SourceFile> files, IProjectPaths path, ulong baseAddress)
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
        }

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
            assembler.EmitJump(InstructionEncoder.EncodeJ((uint)OpCode.JMP), "func_main");
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
        var funcGen = new FunctionGenerator(assembler);
        funcGen.Generate(combinedAst);

        // 7. Один завершающий HALT
        assembler.EmitInstruction(InstructionEncoder.EncodeHALT());

        return assembler.Build();
    }

    public static byte[] BuildProject(IFileService fileService, IEditorService editorService,IProjectPaths paths, ulong baseAddress)
    {
        var files = new List<SourceFile>();

        foreach (string fileName in fileService.GetSourceFiles())
        {
            // Пытаемся получить текст из открытой вкладки
            string source = editorService.GetText(fileName) ?? fileService.ReadFile(fileName);

            // Определяем язык по расширению
            SourceLanguage lang = fileName.EndsWith(".asm", StringComparison.OrdinalIgnoreCase)
                ? SourceLanguage.Asm
                : SourceLanguage.C;

            files.Add(new SourceFile(fileName, source, lang));
        }

        // Вызываем существующий метод Build с базовым адресом (можно параметризовать)
        return Build(files, paths, baseAddress: baseAddress);
    }
}

