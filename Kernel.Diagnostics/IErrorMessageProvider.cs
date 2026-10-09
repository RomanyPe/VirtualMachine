namespace Kernel.Diagnostics;

public interface IErrorMessageProvider
{
    string GetMessage(ErrorCode code, params object[] args);
}
