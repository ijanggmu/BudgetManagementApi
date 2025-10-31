namespace Business.Common.File;
public static class MinioFileFolderConstant
{
    private const string FrontPhoto = "frontPhoto";
    private const string BackPhoto = "backPhoto";
    private const string UserPhoto = "userPhoto";
    private const string PanCertificate = "panCertificate";
    private const string OtherDocuments = "otherDocuments";
    private const string AppLogo = "appLogo";
    private const string CmsLogo = "cmsLogo";

    private const string BankLogo = "bankLogo";
    private const string PassportPhoto = "passportPhoto";
    private const string AdditionalDocumentImage = "additionalDocumentImage";
    private const string UserDigitalSignature = "userDigitalSignature";
    private const string BlueBook = "blueBook";

    public static string[] GetAllFileFolderConstant()
    {
        return [
                FrontPhoto,
                BackPhoto,
                UserPhoto,
                PanCertificate,
                OtherDocuments,
                AppLogo,
                CmsLogo,
                BankLogo,
                PassportPhoto,
                AdditionalDocumentImage,
                UserDigitalSignature,
                BlueBook
            ];
    }
}

