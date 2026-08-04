using ASM_gen.ProjectManage.Data;

namespace ASM_gen.Utils;

public static class ErrorFileHelper
{
    public static bool IsSucced(this ErrorFile error)
    {
        return error == ErrorFile.None;
    }
}
