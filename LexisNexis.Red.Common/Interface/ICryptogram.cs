using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Interface
{
    public interface ICryptogram
    {
        /// <summary>
        /// Decrypt content to string
        /// </summary>
        /// <param name="cipherText">content's bytes</param>
        /// <param name="key">book key</param>
        /// <param name="iv">iv</param>
        /// <returns>decrypted content</returns>
        Task<string> DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv);
        /// <summary>
        /// Decrypt content to bytes
        /// </summary>
        /// <param name="cipherText">content's bytes</param>
        /// <param name="key">book key</param>
        /// <param name="iv">iv</param>
        /// <returns>decrypted content</returns>
        Task<byte[]> DecryptBytes(byte[] cipherText, byte[] key, byte[] iv);
        /// <summary>
        /// get HMAC 
        /// </summary>
        /// <param name="symmetricKey">symmetric Key</param>
        /// <param name="contentEncryptedKey">content's bytes</param>
        /// <returns>HMAC</returns>
        byte[] GenerateHMAC(byte[] symmetricKey, byte[] contentEncryptedKey);
        string GetDefaultEncodingString(byte[] bytes);
        string GetMD5(string message);
    }
}
