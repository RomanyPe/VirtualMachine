namespace VMApplication.Project;

// Результат компиляции
public readonly struct CompilationResult(byte[]? program,ulong startAdress, IReadOnlyList<string>? errors)
{
    public byte[]? Program { get; } = program;
    public ulong StartAdress { get; } = startAdress;
    public IReadOnlyList<string>? Errors { get; } = errors;
    public bool Success => Errors == null || Errors.Count == 0;
}

