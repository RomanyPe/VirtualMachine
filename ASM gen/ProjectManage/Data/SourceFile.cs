using Compiller.Emulation;

namespace ASM_gen.ProjectManage.Data;

public readonly record struct SourceFile(string Name, string Content, SourceLanguage Language);
