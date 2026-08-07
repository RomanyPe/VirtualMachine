namespace VMApplication;

public class TabDataEditor(string path)
{
    public string Path { get; } = path;
    public bool IsOpened { get; set; } = false;
    public string Content { get; set; } = string.Empty;
    public SourceLanguage Language { get; init; } = !path.EndsWith(".asm", StringComparison.OrdinalIgnoreCase) ? SourceLanguage.C : SourceLanguage.Asm;
}

