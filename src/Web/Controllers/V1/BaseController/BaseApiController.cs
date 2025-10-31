using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Operation;


namespace BeemaEdgeApi.Controllers.V1.BaseController
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return NoContent();
                }
                else if (result.Pagination != null)
                {
                    return Ok(SuccessPaginateApiResponse<object, object>.WrapSuccess(result.Data, result.Pagination));
                }
                else
                {
                    return Ok(SuccessApiResponse<object>.WrapSuccess(result.Data));
                }
            }

            return BadRequest(ErrorApiResponse.WrapError(result.Error, result.ErrorCode));
        }
    }
}
