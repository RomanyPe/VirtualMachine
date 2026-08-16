using VMApplication.Project;

namespace ASM_gen.Utils;

public static class ErrorFileHelper
{
    extension(ErrorFile error)
    {
        public bool IsSucced()
        {
            return error == ErrorFile.None;
        }
    }
}
