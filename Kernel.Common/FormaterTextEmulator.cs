namespace Kernel.Common;

public static class FormaterTextEmulator
{
    // Массив ровно под размер byte (от 0 до 255)
    private static readonly string[] _charToStringCache = new string[char.MaxValue + 1];
    private static readonly string[] _suffixes = ["B", "KB", "MB", "GB", "TB"];

    static FormaterTextEmulator()
    {
        for (int i = 0; i < _charToStringCache.Length; i++)
        {
            char character = (char)i;
            _charToStringCache[i] = character.ToString();
        }
    }

    public static string FormatBytes(ulong bytes)
    {
        int counter = 0;

        if (bytes < 0) bytes = 0;

        while (bytes >= 1024 && counter < _suffixes.Length - 1)
        {
            bytes >>= 10;
            counter++;
        }

        return $"{bytes:F1} {_suffixes[counter]}";
    }


    extension(char memoryValue)
    {
        // Супербыстрый метод вывода
        public string CharToString() => _charToStringCache[memoryValue];

        public string AsText => _charToStringCache[memoryValue];
    }
}
