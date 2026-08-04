using ASM_gen.ProjectManage.Data;
using Compiller.ASM;
using Compiller.C;
using Compiller.C.CodeGenerator;
using Compiller.Emulation;
using ICSharpCode.AvalonEdit;
using Kernel.BiosSystem;
using Kernel.ProcessorSystem;
using Kernel.RamSystem;
using System.IO;
using System.Windows.Controls;

namespace ASM_gen.ProjectManage.Managers.Static;

public static class ProjectBuilder
{
    private static readonly string SharedIncludeDir = AppPaths.SharedIncludePath;
    public static byte[] Build(IEnumerable<SourceFile> files, string path, ulong baseAddress = 0x0000)
    {
        var assembler = new Assembler(baseAddress);
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

        // 2. Объединяем все C‑функции и глобальные переменные в один AST
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

        // 3. Генерируем код всех C‑функций (без завершающего HALT)
        var funcGen = new FunctionGenerator(assembler);
        funcGen.Generate(combinedAst);

        // 4. Добавляем библиотеки из #include (после C‑кода)
        foreach (string inc in includes)
        {
            string localPath = Path.Combine(path, inc);
            string sharedPath = Path.Combine(SharedIncludeDir, inc);

            string? chosenPath = null;
            if (File.Exists(localPath))
                chosenPath = localPath;
            else if (File.Exists(sharedPath))
                chosenPath = sharedPath;

            if (chosenPath == null)
                throw new Exception($"Included file not found: '{inc}'. Searched in project folder and in '{SharedIncludeDir}'.");

            string asmCode = File.ReadAllText(chosenPath);
            asmParser.Assemble(asmCode, assembler);
        }

        // 5. Добавляем обычные ассемблерные файлы проекта (если есть)
        foreach (var file in files)
        {
            if (file.Language == SourceLanguage.Asm)
            {
                asmParser.Assemble(file.Content, assembler);
            }
        }

        // 6. Один завершающий HALT
        assembler.EmitInstruction(InstructionEncoder.EncodeHALT());

        return assembler.Build();
    }


    public static byte[] BuildProject(FileService _fileService, TabControl _tabEditor, string path)
    {
        var files = new List<SourceFile>();

        foreach (string fileName in _fileService.GetAllFileNames())
        {
            if (!_fileService.TryGetFile(fileName, out var dataTab))
                continue;

            // Ищем вкладку с таким заголовком
            string source = null!;
            foreach (TabItem item in _tabEditor.Items)
            {
                if (item.Header is string header && header == fileName && item.Content is TextEditor editor)
                {
                    source = editor.Text;
                    break;
                }
            }

            // Если вкладка не найдена (файл не открыт), читаем с диска
            source ??= _fileService.ReadFileSync(fileName);

            files.Add(new SourceFile(fileName, source, dataTab.Language == SourceLanguage.C
                    ? SourceLanguage.C
                    : SourceLanguage.Asm));
        }

        return Build(files, path);
    }

    public static Device? Compile(bool useConsole, bool snowAsm, Emulator emulator, ProjectService projectService, string path)
    {
        try
        {
            var sourceFiles = projectService.GetAllSourceFiles();
            int mainCount = 0;
            foreach (var file in sourceFiles)
            {
                if (file.Language == SourceLanguage.C)
                {
                    var lexer = new Lexer(file.Content);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens);
                    var ast = parser.Parse();
                    mainCount += ast.Functions.Count(f => f.Name == "main");
                }
            }
            if (mainCount > 1)
                throw new Exception("Обнаружено несколько функций main. Оставьте только одну.");


            IDEConsoleManager.InitConsole(useConsole);

            if (emulator.MainDevice == null) return null;

            var device = emulator.MainDevice;
            if (device == null) return null;

            byte[] program;
            try
            {
                program = projectService.BuildProject(path);
            }
            catch (Exception ex)
            {
                DeviceHelpers.LogFromSystem("Build", ex.Message, NotificationType.Error);
                return null;
            }
            if (snowAsm)
                MiniCCompiler.DisassembleCode(program);
            device.LoadProgram(program, 0x0000);
            device.InitHeap((ulong)(program.Length + 7) & ~7UL);
            return device;
        }
        catch (Exception ex)
        {
            DeviceHelpers.LogFromSystem("Build", ex.Message, NotificationType.Error);
            return null;
        }
    }

}
