using System;
using System.Threading.Tasks;
using Business.Common.File;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.Common.File;

namespace BeemaEdgeApi.Controllers.V1.Common.FileUpload;

public class FileUploadController(IFileService fileService) : BaseCommonApiController
{
    [HttpPost("UploadFile")]
    [RequestSizeLimit(10 * 1024 * 1024)] // Limit to 10MB max
    public async Task<IActionResult> UploadFile([FromForm] FileUploadRequestModel model)
    {
        if (!Request.HasFormContentType ||
     !Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType, "Content-Type must be multipart/form-data");
        }
        var result = await fileService.UploadFileAsync(model);

        return HandleResult(result);
    }

}
