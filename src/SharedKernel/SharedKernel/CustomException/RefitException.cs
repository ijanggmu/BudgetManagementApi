namespace SharedKernel.CustomException;

public class RefitException(string message, int errorCode) : Exception(message)
{
    public int ErrorCode { get; } = errorCode;
}
