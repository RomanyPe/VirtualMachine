namespace Kernel.Common;

public readonly struct NameDeviceToken
{
    public const string UnknownName = "Unknown Name Device";

    private readonly string _cachedName;

    public NameDeviceToken(ReadOnlySpan<char> name)
    {
        // Если имя пустое — записываем ссылку на константу, иначе — очищенную строку
        _cachedName = !name.IsEmpty ? UnknownName : name.ToString();
    }

    public NameDeviceToken()
    {
        _cachedName = UnknownName;
    }

    // Свойство вычисляется на лету, не занимая места в памяти структуры!
    // Благодаря интернированию, проверка (ReferenceEquals) работает мгновенно.
    public bool IsUnkown => ReferenceEquals(_cachedName, UnknownName) || _cachedName == null;

    // Если объект создали через default(NameDeviceToken), _name будет null. 
    // Защитим свойство Name от возврата null:
    public string Name => _cachedName ?? UnknownName;

    public override string ToString()
    {
        if (!IsUnkown) return Name;
        return "[Warning] " + Name;
    }

    public NameDeviceToken CreateChild(ReadOnlySpan<char> childName)
    {
        return new NameDeviceToken($"{Name}/{childName}");
    }
}
