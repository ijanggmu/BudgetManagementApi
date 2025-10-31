using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using SharedKernel.Config;

namespace Business.Common.StringCipher;
public class StringCipherService
{
    private readonly byte[] myAesKey;
    private readonly byte[] myAesIV;

    public StringCipherService(IOptions<AesConfig> aesSettings)
    {
        myAesKey = Convert.FromBase64String(aesSettings.Value.Key);
        myAesIV = Convert.FromBase64String(aesSettings.Value.IV);
    }

    public async Task<string> EncryptAsync(string plainText)
    {
        using (Aes myAes = Aes.Create())
        {
            myAes.Key = myAesKey;
            myAes.IV = myAesIV;

            ICryptoTransform encryptor = myAes.CreateEncryptor(myAes.Key, myAes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        await swEncrypt.WriteAsync(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public async Task<string> DecryptAsync(string cipherText)
    {
        using (Aes myAes = Aes.Create())
        {
            myAes.Key = myAesKey;
            myAes.IV = myAesIV;

            ICryptoTransform decryptor = myAes.CreateDecryptor(myAes.Key, myAes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return await srDecrypt.ReadToEndAsync();
                    }
                }
            }
        }
    }

}
