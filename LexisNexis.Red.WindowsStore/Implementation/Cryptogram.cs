using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Interface;

namespace LexisNexis.Red.WindowsStore.Implementation
{
    public class Cryptogram : ICryptogram
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

         


            IBuffer keyBuffer = key.AsBuffer(0, key.Length);
            IBuffer ivBuffer = iv.AsBuffer(0, iv.Length);
            IBuffer encrypted = cipherText.AsBuffer(0, cipherText.Length);

            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            CryptographicKey cryptoKey2 = symmetricAlgorithm.CreateSymmetricKey(keyBuffer);
            IBuffer decrypted = CryptographicEngine.Decrypt(cryptoKey2, encrypted, ivBuffer);
            byte[] decryptedBytes = decrypted.ToArray(0, (int)decrypted.Length);

            using(MemoryStream ms = new MemoryStream(decryptedBytes))
            using (StreamReader srDecrypt = new StreamReader(ms, Encoding.GetEncoding(Constants.SISOEncoding)))
            {
                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                return await srDecrypt.ReadToEndAsync();
            }          
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

            IBuffer keyBuffer = key.AsBuffer(0, key.Length);
            IBuffer ivBuffer = iv.AsBuffer(0, iv.Length);
            IBuffer encrypted = cipherText.AsBuffer(0, cipherText.Length);

            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm("AES_CBC_PKCS7");
            CryptographicKey cryptoKey2 = symmetricAlgorithm.CreateSymmetricKey(keyBuffer);
            IBuffer decrypted = CryptographicEngine.Decrypt(cryptoKey2, encrypted, ivBuffer);
            byte[] decryptedBytes = decrypted.ToArray(0, (int)decrypted.Length);
            return await Task.Run(() => decryptedBytes);
        }

        public byte[] GenerateHMAC(byte[] symmetricKey, byte[] contentEncryptedKey)
        {
            MacAlgorithmProvider hmacAlgorithm = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");

            IBuffer key = symmetricKey.AsBuffer(0, symmetricKey.Length);
            IBuffer content = contentEncryptedKey.AsBuffer(0, contentEncryptedKey.Length);

            CryptographicKey hmacKey = hmacAlgorithm.CreateKey(key);
            IBuffer signature = CryptographicEngine.Sign(
                   hmacKey, 
                   content);
            byte[] encryptedKey = signature.ToArray(0, (int)signature.Length);         
            return encryptedKey;
        }

        public string GetDefaultEncodingString(byte[] bytes)
        {
            string content = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return content;
        }

        public string GetMD5(string message)
        {
            return string.Empty;
        }
    }
}
