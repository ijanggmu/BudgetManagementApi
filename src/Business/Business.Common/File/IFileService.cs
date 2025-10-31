using Models.Common.File;
using SharedKernel.Operation;

namespace Business.Common.File;

public interface IFileService
{
    Task<Result<FileUploadSummaryResponseModel>> UploadFileAsync(FileUploadRequestModel model);
    Task<string> GetFilePresignedUrlAsync(string objectName);
}



