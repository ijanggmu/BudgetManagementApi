using System.ComponentModel.DataAnnotations;

namespace Models.WebApi.Policy;
public class PrintPolicyCoreRequestModel
{
    public string draftNo { get; set; }

    public bool ishtml { get; set; }

    public bool isNepali { get; set; }

    public bool isCertificate { get; set; }

    public bool isReceipt { get; set; }

    public bool isEnglish { get; set; }
}
public class PrintPolicyRequestModel
{
    [Required]
    public string DraftNumber { get; set; }
    [Required]
    public bool isNepali { get; set; }
}
public class PrintReceiptRequestModel
{
    [Required]
    public string DraftNumber { get; set; }
}
public class PrintCertificateRequestModel
{
    [Required]
    public string DraftNumber { get; set; }
    [Required]
    public bool isNepali { get; set; }
}

