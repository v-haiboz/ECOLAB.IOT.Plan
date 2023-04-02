namespace ECOLAB.IOT.Plan.Provider.Certification
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public interface IECOLABIOTSecurityProvider
    {
        public string AESEncrypt(string str);
        public string AESDecrypt(string str);
    }

    public class ECOLABIOTSecurityProvider : IECOLABIOTSecurityProvider
    {
        private static string _KEY = "1234567896011121";
        private static string _IV = "eyy7c;4@#43454..-+234$#fds";
        public string AESEncrypt(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(plainText);
                aesAlg.Key = Encoding.UTF8.GetBytes(_KEY);
                aesAlg.IV = Encoding.Unicode.GetBytes(_IV).Take(16).ToArray();
                byte[] resultArray = aesAlg.EncryptEcb(toEncryptArray, PaddingMode.PKCS7);
                var data = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                return data;
            }
        }

        public string AESDecrypt(string cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            using (Aes aesAlg = Aes.Create())
            {
                byte[] toEncryptArray = Convert.FromBase64String(cipherText);
                aesAlg.Key = Encoding.UTF8.GetBytes(_KEY);
                aesAlg.IV = Encoding.Unicode.GetBytes(_IV).Take(16).ToArray();
                byte[] resultArray = aesAlg.DecryptEcb(toEncryptArray, PaddingMode.PKCS7);
                var data = Encoding.UTF8.GetString(resultArray);
                return data;
            }
        }
    }
}
