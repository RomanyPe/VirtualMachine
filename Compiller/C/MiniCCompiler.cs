using Compiller.ASM;
using Kernel.BiosSystem;

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
    public const string DefaultCode = @"
int a;
int b;
int result;

int sum(int x, int y) {
    int s;
    int i;
    s = 0;
    i = 0;
    while (i < x) {
        s = s + y;
        i = i + 1;
    }
    return s;
}

void m(){
    a = 90;
}

int main() {
    a = 3;
    b = 4;
    a = a + 1;
    m();
    result = sum(a, b);
    return result;
}
";

    private const string _nameSystem = "Mini-C Compiler";
    /// <summary>Компилирует исходный код в байт-код для виртуальной машины.</summary>
    public static byte[] Compile(string source, ulong baseAddress = 0)
    {
        var lexer = new Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        var parser = new Parser(tokens);
        ProgramNode ast = parser.Parse();

        // Перемещаем main в начало, чтобы выполнение начиналось с неё
        FunctionNode? mainFunc = ast.Functions.FirstOrDefault(f => f.Name == "main");
        if (mainFunc != null)
        {
            ast.Functions.Remove(mainFunc);
            ast.Functions.Insert(0, mainFunc);
        }

        var assembler = new Assembler(baseAddress);
        var generator = new CodeGenerator(assembler);
        generator.Generate(ast);

        return assembler.Build();
    }

    /// <summary>Выводит дизассемблированный код программы в консоль.</summary>
    public static void DisassembleCode(byte[] program)
    {
        DeviceHelpers.LogFromSystem(_nameSystem, Disassembler.Disassemble( program, out int lines, out int size, 0));
        DeviceHelpers.LogFromSystem(_nameSystem, $"Количество строк кода: {lines}");
        DeviceHelpers.LogFromSystem(_nameSystem, $"Размер файла программы: {size} байт");
    }
}