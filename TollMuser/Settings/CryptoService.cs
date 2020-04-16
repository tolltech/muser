using System;
using System.Security.Cryptography;
using System.Text;

namespace Tolltech.Muser.Settings
{
    public class CryptoService : ICryptoService
    {
        public string Encrypt(string src, string cryptoKey)
        {
            var toEncryptArray = Encoding.UTF8.GetBytes(src);

            var hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(cryptoKey));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tdes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string src, string cryptoKey)
        {
            var toEncryptArray = Convert.FromBase64String(src);

            var hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(cryptoKey));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            try
            {
                var cTransform = tdes.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (CryptographicException e)
            {
                return null;
            }                        
        }
    }
}