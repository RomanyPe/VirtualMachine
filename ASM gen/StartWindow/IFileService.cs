namespace ASM_gen.StartWindow;

public interface IFileService
{
    void AddFilePath(string? path);
    Task<string> GetAllText();
    ValueTask<string> GetTextFile(string fileName);
    void RemoveFile(string fileName);
    bool TryGetFile(string fileName, out TabDataEditor dataTab);
    Task<ErrorFile> TrySaveTextFile(string fileName, string text);
}