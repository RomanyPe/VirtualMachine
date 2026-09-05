namespace Kernel.Common;

public interface IErrorMessageProvider
{
    string GetMessage(ErrorCode code, params ReadOnlySpan<object> args);
}
