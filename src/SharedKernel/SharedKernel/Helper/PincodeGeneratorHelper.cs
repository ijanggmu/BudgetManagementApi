using System.Text;
namespace SharedKernel.Helper;

public static class PincodeGeneratorHelper
{
    private static readonly Random random = new Random();

    public static string GeneratePincode(int length)
    {
        StringBuilder pincodeBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            pincodeBuilder.Append(random.Next(10));
        }
        return pincodeBuilder.ToString();
    }

    public static string[] GenerateMultiplePincodes(int length, int count)
    {
        string[] pincodes = new string[count];
        for (int i = 0; i < count; i++)
        {
            pincodes[i] = GeneratePincode(length);
        }
        return pincodes;
    }
}
