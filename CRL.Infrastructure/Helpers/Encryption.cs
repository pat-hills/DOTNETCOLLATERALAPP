using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CRL.Infrastructure.Helpers
{
    public static class Encryption
    {
        private static RijndaelManaged _cryptoProvider;
        //128 bit encyption: DO NOT CHANGE    
        private static readonly byte[] Key = { 18, 19, 8, 24, 36, 22, 4, 22, 17, 5, 11, 9, 13, 15, 06, 23 };
        private static readonly byte[] IV = { 14, 2, 16, 7, 5, 9, 17, 8, 4, 47, 16, 12, 1, 32, 25, 18 };

        static Encryption()
        {
            _cryptoProvider = new RijndaelManaged();
            _cryptoProvider.Mode = CipherMode.CBC;
            _cryptoProvider.Padding = PaddingMode.PKCS7;
        }

        /// <summary>
        /// Encrypts a given string.
        /// </summary>
        /// <param name="unencryptedString">Unencrypted string</param>
        /// <returns>Returns an encrypted string</returns>
        public static string Encrypt(string unencryptedString)
        {
            byte[] bytIn = ASCIIEncoding.ASCII.GetBytes(unencryptedString);

            // Create a MemoryStream
            MemoryStream ms = new MemoryStream();

            // Create Crypto Stream that encrypts a stream
            CryptoStream cs = new CryptoStream(ms,
                _cryptoProvider.CreateEncryptor(Key, IV),
                CryptoStreamMode.Write);

            // Write content into MemoryStream
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();

            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }

        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="encryptedString">Encrypted string</param>
        /// <returns>Returns a decrypted string</returns>
        public static string Decrypt(string encryptedString)
        {
            encryptedString = encryptedString.Replace(" ", "+");
            if (encryptedString.Trim().Length != 0)
            {
                // Convert from Base64 to binary
                byte[] bytIn = Convert.FromBase64String(encryptedString);

                // Create a MemoryStream
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);

                // Create a CryptoStream that decrypts the data
                CryptoStream cs = new CryptoStream(ms,
                    _cryptoProvider.CreateDecryptor(Key, IV),
                    CryptoStreamMode.Read);

                // Read the Crypto Stream
                StreamReader sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            else
            {
                return "";
            }
        }

    }
}
