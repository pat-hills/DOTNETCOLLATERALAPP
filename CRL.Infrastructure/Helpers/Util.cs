using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CRL.Infrastructure.Helpers
{
    public static class Util
    {
        //128 bit encyption: DO NOT CHANGE    
        private static readonly byte[] Key = { 18, 19, 8, 24, 36, 22, 4, 22, 17, 5, 11, 9, 13, 15, 06, 23 };
        private static readonly byte[] IV = { 14, 2, 16, 7, 5, 9, 17, 8, 4, 47, 16, 12, 1, 32, 25, 18 };
        private static RijndaelManaged _cryptoProvider = new RijndaelManaged()
        {
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7
        };
        public static string GetNewValidationCode()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public static  string GetUniqueKey()
        {
            int maxSize = 8;
            int minSize = 5;
            char[] chars = new char[62];
            string a;
            a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);;
            }
            return result.ToString();
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

        public static string GetUrlEncode(string url)
        {


            return HttpContext.Current.Server.UrlEncode(url);
        }

        public static string GetUrlDecode(string url)
        {

            return HttpContext.Current.Server.UrlDecode(url); ;

        }
        public static string GetFlexibleBorrowerNameForSearch(string BorrowerName)
        {
            string[] exclude_word = { "Ltd", "Ltd.", "Inc", "Inc.", "Incorp", "Incorp.", "Corp", "Corp.", "Co", "Co.", "Limited", "Limited.", "Incorporated", "Incorporated.", "Corporation", "Corporation.", "Company", "Company.", "Ent.", "Enterprise","Enterprise." };
            string[] mBName_word = BorrowerName.Trim().Split(' ');
            int indx = mBName_word.Length;

            bool isExists = exclude_word.Any(s =>
            s.Equals(mBName_word[indx - 1], StringComparison.InvariantCultureIgnoreCase));

            if (isExists)
            {
                BorrowerName = string.Empty;
                for (int i = 0; i < indx - 1; i++)
                {
                    BorrowerName += " " + mBName_word[i];
                }
            }
            return BorrowerName.ToLower().Trim();
        }

    }
}
