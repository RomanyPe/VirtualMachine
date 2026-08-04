using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows;
using System.Windows.Media;

namespace ASM_gen.Highlight;

public enum LanguageType
{
    ASM,
    C,
}
public static partial class CastomHighlightingManager
{
    private class CustomHighlightingDefinition(HighlightingRuleSet mainRuleSet) : IHighlightingDefinition
    {
        private readonly HighlightingRuleSet _mainRuleSet = mainRuleSet;

        public string Name => "MyAsm";
        public HighlightingRuleSet MainRuleSet => _mainRuleSet;
        public HighlightingRuleSet GetNamedRuleSet(string name) => null!;
        public HighlightingColor GetNamedColor(string name) => null!;
        public static IEnumerable<HighlightingColor> NamedColors => null!;
        public IDictionary<string, string> Properties => null!;

        public IEnumerable<HighlightingColor> NamedHighlightingColors => null!;
    }

    public static void ChoseLang(LanguageType lang, TextEditor textEditor)
    {
        if (textEditor == null) return;
        textEditor.SyntaxHighlighting = lang switch
        {
            LanguageType.ASM => ApplyASMHighlighting(),
            LanguageType.C => ApplyCHighlighting(),
            _ => ApplyCHighlighting()
        };
    }
    private static CustomHighlightingDefinition ApplyASMHighlighting()
    {
        // 1. Создаем пустую разметку правил синтаксиса
        HighlightingRuleSet ruleSet = new();

        // 2. Правило для комментариев (; комментарий или // комментарий)
        HighlightingColor commentColor = new()
        {
            Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#064a1b"))
        };
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.RegexASMCommentColor(),
            Color = commentColor
        });

        // 3. Правило для команд (MOV, CLR, ADD, SUB, LW, SW, LOAD, STR, RET)
        HighlightingColor keywordColor = new()
        {
            Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#a5e6e3")),
        };
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.RegexASMKeywordColor(),
            Color = keywordColor
        });

        // 4. Правило для ВАШИХ регистров (r0..r22, sp, ra, zero)
        HighlightingColor registerColor = new()
        {
            Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#729687")),
        };
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.RegexASMRegisterColor(),
            Color = registerColor
        });

        // 5. Правило для чисел и смещений (10, 0x1A)
        HighlightingColor numberColor = new()
        {
            Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#7fe069")),
            FontWeight = FontWeights.Bold
        };
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.RegexASMNumberColor(),
            Color = numberColor,
        });

        // 6. Упаковываем это в определение синтаксиса
        CustomHighlightingDefinition asmDefinition = new(ruleSet);

        // 7. Применяем к редактору
        return asmDefinition;
    }

    private static CustomHighlightingDefinition ApplyCHighlighting()
    {
        var ruleSet = new HighlightingRuleSet();

        // 1. Многострочные комментарии /* ... */ (должны быть обработаны первыми,
        //    чтобы не перекрываться однострочными или ключевыми словами)
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.CommentMultiLine(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#6A9955"))
            }
        });

        // 2. Однострочные комментарии //
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.CommentSingleLine(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#6A9955"))
            }
        });

        // 3. Строковые литералы "..."
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.StringLiteral(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#CE9178"))
            }
        });

        // 4. Символьные литералы 'x'
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.CharLiteral(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#CE9178"))
            }
        });

        // 5. Ключевые слова (int, char, void, if, while, for, return и т.д.)
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.Keyword(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#569CD6"))
            }
        });

        // 6. Числа (десятичные и 0x...)
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.Number(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#B5CEA8")),
                FontWeight = FontWeights.Bold
            }
        });

        // 7. Операторы (+, -, *, /, ==, !=, <, >, &&, || и т.д.)
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.Operator(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"))
            }
        });

        // 8. Пунктуация (скобки, запятые, точка с запятой)
        //    (можно закомментировать, если не нужна отдельная подсветка)
        ruleSet.Rules.Add(new HighlightingRule
        {
            Regex = SyntaxHighlighter.Punctuation(),
            Color = new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush((Color)ColorConverter.ConvertFromString("#D4D4D4"))
            }
        });

        return new CustomHighlightingDefinition(ruleSet);
    }
}