using Compiller.ASM;
using Compiller.C.CodeGenerator;
using Kernel.BiosSystem;
using Kernel.Common;

namespace Compiller.C;

// ============================================================
// ОБЁРТКА КОМПИЛЯТОРА
// ============================================================
/// <summary>
/// Статический класс, отвечающий только за компиляцию исходного кода на C-подобном языке
/// и вспомогательные утилиты (дизассемблирование).
/// </summary>
public static class MiniCCompiler
{
    private const string _nameSystem = "Mini-C Compiler";
    /// <summary>Компилирует исходный код в байт-код для виртуальной машины.</summary>
    public static byte[] Compile(string source, ulong baseAddress = 0)
    {
        var asm = new Assembler(baseAddress);
        Compile(source, asm);
        return asm.Build();
    }

    public static void Compile(ProgramNode ast, Assembler asm)
    {
        var generator = new FunctionGenerator(asm);
        generator.Generate(ast);
    }

    public static void Compile(string source, Assembler asm)
    {
        var lexer = new Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        var parser = new Parser(tokens);
        ProgramNode ast = parser.Parse();

        // main в начало
        FunctionNode? mainFunc = ast.Functions.FirstOrDefault(f => f.Name == "main");
        if (mainFunc != null)
        {
            ast.Functions.Remove(mainFunc);
            ast.Functions.Insert(0, mainFunc);
        }

        var generator = new FunctionGenerator(asm);
        generator.Generate(ast);
    }
}