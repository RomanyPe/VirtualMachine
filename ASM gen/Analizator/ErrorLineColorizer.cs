using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace ASM_gen.Analizator;

public class ErrorLineColorizer : DocumentColorizingTransformer
{
    // Храним номера строк с ошибками (1-based индексация, как в AvalonEdit)
    public HashSet<int> ErrorLines { get; } = [];

    protected override void ColorizeLine(DocumentLine line)
    {
        // Проверяем, есть ли текущая строка в списке ошибок
        if (ErrorLines.Contains(line.LineNumber))
        {
            // Изменяем свойства отображения для всей строки целиком
            ChangeLinePart(
                line.Offset,
                line.EndOffset,
                visualLineElement =>
                {
                    // Устанавливаем светло-красный фон для строки
                    visualLineElement.TextRunProperties.SetBackgroundBrush(
                        new SolidColorBrush(Color.FromArgb(50, 255, 0, 0))
                    );

                    // Опционально: можно изменить цвет самого текста на темно-красный
                    // visualLineElement.TextRunProperties.SetForegroundBrush(Brushes.DarkRed);
                });
        }
    }
}
