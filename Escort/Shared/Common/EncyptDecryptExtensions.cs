using System.Security.Cryptography;
using System.Text;

namespace Shared.Common
{
    public static class EncryptDecryptExtensions
    {
        public static string ToEncrypt(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            using (var tripleDES = TripleDES.Create())
            {
                if (tripleDES != null)
                {
                    var keyBytes = UTF8Encoding.UTF8.GetBytes(SiteKeys.EncryptDecryptKey ?? string.Empty);
                    Array.Resize(ref keyBytes, 24); // Ensure key is exactly 24 bytes
                    tripleDES.Key = keyBytes;
                    tripleDES.Mode = CipherMode.ECB;
                    tripleDES.Padding = PaddingMode.PKCS7;

                    ICryptoTransform cTransform = tripleDES.CreateEncryptor();

                    byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
                    byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);

                    string base64 = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                    // Replace '/' with a custom separator to avoid URL encoding issues
                    return base64.Replace("/", "-$$-$$-");
                }
                else
                {
                    throw new InvalidOperationException("TripleDES.Create() returned null. Check if TripleDES is supported.");
                }
            }
        }

        public static string ToDecrypt(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            byte[] inputArray = Convert.FromBase64String(input.Replace("-$$-$$-", "/"));
            var tripleDES = TripleDES.Create();
            var keyBytes = UTF8Encoding.UTF8.GetBytes(SiteKeys.EncryptDecryptKey ?? string.Empty);
            Array.Resize(ref keyBytes, 24); 
             tripleDES.Key = keyBytes;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);

        }

    }
}
