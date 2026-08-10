namespace Kernel.Common;

public static class CacheCharsInt8
{
    // Массив ровно под размер byte (от 0 до 255)
    private static readonly string[] ByteToTextCache = new string[256];

    static CacheCharsInt8()
    {
        for (int i = 0; i < 256; i++)
        {
            char character = (char)i;
            ByteToTextCache[i] = character.AsText;
        }
    }

    extension(char memoryValue)
    {
        // Супербыстрый метод вывода
        public string CharToString() => ByteToTextCache[memoryValue];

        public string AsText => ByteToTextCache[memoryValue];
    }
}
