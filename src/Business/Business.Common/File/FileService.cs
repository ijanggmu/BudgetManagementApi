using System.Text.RegularExpressions;
using Infrastructure.Common.UserProfile;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Models.Common.File;
using SharedKernel.Config;
using SharedKernel.Operation;

namespace Business.Common.File;

public class FileService : IFileService, IDisposable
{
    private readonly ILogger<FileService> _logger;
    private readonly IOptions<MinioSettings> _minioConfig;
    private readonly IMinioClient _minioClient;
    private readonly IUserProfileService _userProfileService;
    private bool _disposed = false;

    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".heif", ".heic", ".jfif" };
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "application/pdf", "image/heic", "image/heif" };
    private readonly int _signedUrlExpiryInSeconds = 120;

    public FileService(ILogger<FileService> logger, IUserProfileService userProfileService, IOptions<MinioSettings> minioConfig)
    {
        _logger = logger;
        _minioConfig = minioConfig;

        _minioClient = new MinioClient()
            .WithEndpoint(_minioConfig.Value.Endpoint)
            .WithCredentials(_minioConfig.Value.AccessKey, _minioConfig.Value.SecretKey)
            .WithSSL()
            .Build();
        _userProfileService = userProfileService;
    }

    public async Task<Result<FileUploadSummaryResponseModel>> UploadFileAsync(FileUploadRequestModel model)
    {
        var userId = _userProfileService.GetUserId();
        // Validate input early
        if (model.File == null || model.File.Length == 0)
            return Result<FileUploadSummaryResponseModel>.Failed("No file was uploaded.");

        if (!MinioFileFolderConstant.GetAllFileFolderConstant().Contains(model.Type))
            return Result<FileUploadSummaryResponseModel>.Failed("Invalid file type.");

        var extension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            return Result<FileUploadSummaryResponseModel>.Failed("Unsupported file extension.");

        using var memoryStream = new MemoryStream();
        await model.File.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        try
        {
            //ValidateMimeType(memoryStream, extension);

            var safeType = SanitizeSegment(model.Type);
            var safeForType = SanitizeSegment(model.ForType);
            var safeOrder = SanitizeSegment(model.Order);

            var basePath = string.IsNullOrWhiteSpace(safeForType)
                ? $"{userId}/{safeType}"
                : $"{userId}/{safeForType}/{safeType}";

            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = string.IsNullOrWhiteSpace(safeOrder)
                ? $"{basePath}/{uniqueName}"
                : $"{basePath}_{safeOrder}/{uniqueName}";

            if (extension == ".heic" || extension == ".heif")
            {
                var jpegFilePath = $"{filePath}.jpg";
                //await ConvertHEICtoJPEGAndUpload(memoryStream, jpegFilePath);
                return Result<FileUploadSummaryResponseModel>.Success(new FileUploadSummaryResponseModel { FilePath = jpegFilePath });
            }

            var putArgs = new PutObjectArgs()
                .WithBucket(_minioConfig.Value.Bucket)
                .WithObject(filePath)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithContentType("application/octet-stream");

            await _minioClient.PutObjectAsync(putArgs);
            return Result<FileUploadSummaryResponseModel>.Success(new FileUploadSummaryResponseModel { FilePath = filePath });
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex, "MinIO error while uploading file");
            return Result<FileUploadSummaryResponseModel>.Failed("File storage error.");
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "File validation failed");
            return Result<FileUploadSummaryResponseModel>.Failed("File validation failed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during file upload");
            return Result<FileUploadSummaryResponseModel>.Failed("Unexpected server error.");
        }
    }

    public async Task<string> GetFilePresignedUrlAsync(string objectName)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_minioConfig.Value.Bucket)
                .WithObject(objectName)
                .WithExpiry(_signedUrlExpiryInSeconds);

            return await _minioClient.PresignedGetObjectAsync(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL");
            return null;
        }
    }

    //private void ValidateMimeType(Stream stream, string extension)
    //{
    //    stream.Position = 0;

    //    var inspector = new ContentInspectorBuilder()
    //        .AddDefaults()
    //        .Build();

    //    var result = inspector.Inspect(stream);
    //    stream.Position = 0;

    //    if (result == null || !_allowedMimeTypes.Contains(result.MimeType))
    //    {
    //        throw new InvalidDataException("Invalid MIME type detected.");
    //    }
    //}

    private string SanitizeSegment(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Only allow alphanumeric, dash, underscore to keep paths safe
        return Regex.Replace(input, @"[^a-zA-Z0-9-_]", string.Empty);
    }

    //private async Task ConvertHEICtoJPEGAndUpload(Stream heicStream, string jpegFilePath)
    //{
    //    using var jpegStream = new MemoryStream();
    //    using (var image = new MagickImage(heicStream))
    //    {
    //        image.Format = MagickFormat.Jpeg;
    //        await image.WriteAsync(jpegStream);
    //    }
    //    jpegStream.Position = 0;

    //    var putArgs = new PutObjectArgs()
    //        .WithBucket(_minioConfig.Value.Bucket)
    //        .WithObject(jpegFilePath)
    //        .WithStreamData(jpegStream)
    //        .WithObjectSize(jpegStream.Length)
    //        .WithContentType("image/jpeg");

    //    await _minioClient.PutObjectAsync(putArgs);
    //}

    public void Dispose()
    {
        if (!_disposed)
        {
            _minioClient?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }


}



