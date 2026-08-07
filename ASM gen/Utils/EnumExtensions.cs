using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;

namespace ASM_gen.Utils;

public static class EnumExtensions
{
    extension(Enum value)
    {
        public string GetDescription()
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }

    public static IEnumerable<EnumItem> GetEnumItems<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new EnumItem(e, e.GetDescription()));
    }

    // Необобщённый метод для использования в конвертере
    public static IEnumerable<EnumItem> GetEnumItems(Type enumType)
    {
        if (!enumType.IsEnum) throw new ArgumentException("Type must be an enum");
        return Enum.GetValues(enumType)
            .Cast<Enum>()
            .Select(e => new EnumItem(e, e.GetDescription()));
    }
}

public class EnumItem(Enum value, string displayName)
{
    public Enum Value { get; } = value;
    public string DisplayName { get; } = displayName;
    public override string ToString() => DisplayName;
}

// Конвертер для ComboBox, который показывает список элементов перечисления
public class EnumToItemsSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value == null) return Array.Empty<EnumItem>();
        // Получаем тип перечисления из значения (значение — это выбранный элемент)
        Type enumType = value.GetType();
        return EnumExtensions.GetEnumItems(enumType);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is EnumItem item) return item.Value;
        return Binding.DoNothing;
    }
}