using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BackBone
{
    public static class AES
    {
        public static string Encrypt(string toEncrypt, string shareKey)
        {
            if (toEncrypt == null)
            {
                throw new ArgumentNullException("toEncrypt");
            }
            if (shareKey == null)
            {
                throw new ArgumentNullException("shareKey");
            }
            shareKey = shareKey.Length > 32 ? shareKey.Substring(0, 32) : shareKey.PadLeft(32, shareKey[0]);

            byte[] keyArray = Encoding.UTF8.GetBytes(shareKey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            var rDel = new RijndaelManaged { Key = keyArray, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="shareKey"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt, string shareKey)
        {
            if (toDecrypt == null)
            {
                throw new ArgumentNullException("toDecrypt");
            }
            if (shareKey == null)
            {
                throw new ArgumentNullException("shareKey");
            }
            shareKey = shareKey.Length > 32 ? shareKey.Substring(0, 32) : shareKey.PadLeft(32, shareKey[0]);

            byte[] keyArray = Encoding.UTF8.GetBytes(shareKey);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

      
    }
}
