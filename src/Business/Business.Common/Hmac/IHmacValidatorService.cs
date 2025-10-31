using Microsoft.AspNetCore.Http;

namespace Business.Common.Hmac;

public interface IHmacValidatorService
{
    Task<bool> IsValidAsync(HttpRequest request);
}

