namespace Models.BeemaEdgeApi.UserSignature;

public class UserSignatureResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string SignatureUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
