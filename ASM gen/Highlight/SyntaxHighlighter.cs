using System.Text.RegularExpressions;

namespace ASM_gen.Highlight
{
    public static partial class SyntaxHighlighter
    {
        [GeneratedRegex(@"^\s*#include\s+([<""][^>""]+[>""])", RegexOptions.Multiline)]
        public static partial Regex IncludeDirective();

        // Однострочный комментарий // ...
        [GeneratedRegex(@"//.*")]
        public static partial Regex CommentSingleLine();

        // Многострочный комментарий /* ... */ (поддерживает переводы строк)
        [GeneratedRegex(@"/\*[\s\S]*?\*/")]
        public static partial Regex CommentMultiLine();

        // Ключевые слова языка
        [GeneratedRegex(@"\b(byte|ushort|ulong|int|char|void|if|else|while|for|return|break|continue|extern|include|struct)\b")]
        public static partial Regex Keyword();

        // Числовые литералы: десятичные и шестнадцатеричные (0x...)
        [GeneratedRegex(@"\b0x[0-9a-fA-F]+|\b\d+\b")]
        public static partial Regex Number();

        // Строковый литерал в двойных кавычках с поддержкой escape-последовательностей
        [GeneratedRegex(@"""(?:\\.|[^""\\])*""")]
        public static partial Regex StringLiteral();

        // Символьный литерал в одинарных кавычках
        [GeneratedRegex(@"'(?:\\.|[^'\\])'")]
        public static partial Regex CharLiteral();

        // Операторы (составные и одиночные)
        [GeneratedRegex(@"[+\-*/%<>=!&|^~]+")]
        public static partial Regex Operator();

        // Знаки пунктуации (скобки, запятые, точка с запятой и пр.)
        [GeneratedRegex(@"[{}()\[\];,.:]")]
        public static partial Regex Punctuation();


        [GeneratedRegex(@";.*|//.*")]
        public static partial Regex RegexASMCommentColor();

        [GeneratedRegex(@"rZ|\b(r[0-9]|r1[0-9]|r2[0-2]|rCD|rFL|rLP|rCL|rRT|rSP|rHP|rIP)\b", RegexOptions.IgnoreCase, "ru-RU")]
        public static partial Regex RegexASMRegisterColor();

        [GeneratedRegex(@"\b(NOP|HALT|MOV|LOAD|STORE|LDI|ADD|SUB|INC|DEC|AND|OR|XOR|NOT|JMP|JZ|JNZ|JG|JL|PRINT|PUSH|POP|CALL|RET)\b", RegexOptions.IgnoreCase, "ru-RU")]
        public static partial Regex RegexASMKeywordColor();

        [GeneratedRegex(@"\b0x[0-9a-fA-F]+\b|\b\d+\b")]
        public static partial Regex RegexASMNumberColor();
    }
}
