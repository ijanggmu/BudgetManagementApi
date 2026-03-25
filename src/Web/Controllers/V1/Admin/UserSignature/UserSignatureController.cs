using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using Microsoft.AspNetCore.Http;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.Common.File;
using Business.Common.TenantDomain;
using Microsoft.AspNetCore.Mvc;
using Models.Common.File;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.UserSignature;

[AdminOrSuperAdmin]
public class UserSignatureController(IUserSignatureService signatureService, IFileService fileService) : BaseAdminApiController
{
    [HttpGet]
    [Permission(MenuPermissionConstant.SignatureView)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        => HandleResult(await signatureService.GetCurrentUserSignatureAsync(cancellationToken));

    [HttpPost("upload")]
    [Permission(MenuPermissionConstant.SignatureUpload)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");
        var uploadResult = await fileService.UploadFileAsync(new FileUploadRequestModel
        {
            File = file,
            Type = "userDigitalSignature"
        });
        if (!uploadResult.IsSuccess)
            return BadRequest(SharedKernel.Operation.ErrorApiResponse.WrapError(uploadResult.Error, uploadResult.ErrorCode));
        return HandleResult(await signatureService.SetSignatureUrlAsync(uploadResult.Data.FilePath, cancellationToken));
    }

    public record SetSignatureUrlRequest(string SignatureUrl);

    [HttpPost("set")]
    [Permission(MenuPermissionConstant.SignatureUpload)]
    [Consumes("application/json")]
    public async Task<IActionResult> SetAsync([FromBody] SetSignatureUrlRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.SignatureUrl))
            return BadRequest("No signatureUrl provided.");

        return HandleResult(await signatureService.SetSignatureUrlAsync(request.SignatureUrl, cancellationToken));
    }
}
