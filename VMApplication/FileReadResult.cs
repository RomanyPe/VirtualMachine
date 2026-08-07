namespace VMApplication;

public readonly struct FileReadResult(string fileName, string finalFilePath, ErrorFile errorFile = ErrorFile.None)
{
    public string FileName { get; } = fileName;
    public string FinalFilePath { get; } = finalFilePath;
    public ErrorFile Error { get; } = errorFile;
}

