namespace Models.Common;
public class MessageResponseModel
{
    public string Message { get; set; }

    public MessageResponseModel()
    {

    }
    public MessageResponseModel(string message)
    {
        Message = message;
    }
}
public class VerifyOtpResponseModel(string message, string token)
{
    public string Message { get; set; } = message;
    public string Token { get; set; } = token;
}
