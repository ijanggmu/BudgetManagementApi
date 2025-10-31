using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Models.Common.File;
public class FileUploadRequestModel
{
    [Required]
    public IFormFile File { get; set; }

    [Required]
    public string Type { get; set; }

    public string ForType { get; set; }

    public string Order { get; set; }
}
