using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Implementation
{
    public class AndroidCryptogram : ICryptogram
    {
        public async Task<string> DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(Constants.SCipherText);
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(Constants.Key);
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(Constants.IV);

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            AesCryptoServiceProvider aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;


            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (aesAlg = new AesCryptoServiceProvider())
            {
                //aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                using(ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.GetEncoding(Constants.SISOEncoding)))
                {
                    // Read the decrypted bytes from the decrypting stream
                    // and place them in a string.
                    plaintext = await srDecrypt.ReadToEndAsync();
                }
            }

            // Clear the RijndaelManaged object.
            if (aesAlg != null)
                aesAlg.Clear();

            return plaintext;
        }

        public async Task<byte[]> DecryptBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(Constants.SCipherText);

            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(Constants.Key);

            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(Constants.IV);

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            AesCryptoServiceProvider aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            byte[] buffer;

            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (aesAlg = new AesCryptoServiceProvider())
            {
                //aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    byte[] tempbuffer = new byte[cipherText.Length];
                    int totalBytesRead = await csDecrypt.ReadAsync(tempbuffer, 0, tempbuffer.Length);
                    await csDecrypt.FlushAsync();
                    buffer = tempbuffer.Take(totalBytesRead).ToArray();
                }
            }

            // Clear the RijndaelManaged object.
            if (aesAlg != null)
                aesAlg.Clear();

            return buffer;
        }

        public byte[] GenerateHMAC(byte[] symmetricKey, byte[] contentEncryptedKey)
        {
            byte[] key;
            using (HMACSHA1 hMACSHA1 = new HMACSHA1(symmetricKey))
            {
                key = hMACSHA1.ComputeHash(contentEncryptedKey);
            }
            return key;
        }

        public string GetDefaultEncodingString(byte[] bytes)
        {
            string content = Encoding.Default.GetString(bytes);
            return content;
        }

        public string GetMD5(string message)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] m=Encoding.Default.GetBytes(message);
            byte[] res = md5.ComputeHash(m, 0, m.Length); 
            char[] temp = new char[res.Length];
            System.Array.Copy(res, temp, res.Length);
            return new String(temp); 
        }
    }
}
