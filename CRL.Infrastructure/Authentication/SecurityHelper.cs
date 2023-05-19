using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CRL.Infrastructure.Authentication
{
    static public class SecurityHelper
    {
        private static int SaltValueSize = 8;

        public static string GenerateSaltValue()
        {
            string newCode = String.Empty;
            int seed = unchecked(DateTime.Now.Ticks.GetHashCode());
            Random random = new Random(seed);

            // keep going until we find a unique code       
            
                newCode = random.Next(0, 9999).ToString("0000")
                        + random.Next(0, 9999).ToString("0000");
       
            return newCode;
        }

        //public static string GenerateSaltValue()
        //{
        //    UnicodeEncoding utf16 = new UnicodeEncoding();

        //    if (utf16 != null)
        //    {
        //        // Create a random number object seeded from the value
        //        // of the last random seed value. This is done
        //        // interlocked because it is a static value and we want
        //        // it to roll forward safely.

        //        Random random = new Random(unchecked((int)DateTime.Now.Ticks));

        //        if (random != null)
        //        {
        //            // Create an array of random values.

        //            byte[] saltValue = new byte[SaltValueSize];

        //            random.NextBytes(saltValue);

        //            // Convert the salt value to a string. Note that the resulting string
        //            // will still be an array of binary values and not a printable string. 
        //            // Also it does not convert each byte to a double byte.

        //            string saltValueString = utf16.GetString(saltValue);

        //            // Return the salt value as a string.

        //            return saltValueString;
        //        }
        //    }

          
        //}

        public static string HashPassword(string clearData, string saltValue, HashAlgorithm hash)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
          
            if (clearData != null && hash != null && encoding != null)
            {
                // If the salt string is null or the length is invalid then
                // create a new valid salt value.
                //saltValue = "12345678";
                if (saltValue == null)
                {
                    // Generate a salt string.
                    saltValue = GenerateSaltValue();
                }

                // Convert the salt string and the password string to a single
                // array of bytes. Note that the password string is Unicode and
                // therefore may or may not have a zero in every other byte.

                byte[] binarySaltValue = new byte[SaltValueSize];

                binarySaltValue[0] = byte.Parse(saltValue.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                binarySaltValue[1] = byte.Parse(saltValue.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                binarySaltValue[2] = byte.Parse(saltValue.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
                binarySaltValue[3] = byte.Parse(saltValue.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);

                byte[] valueToHash = new byte[SaltValueSize + encoding.GetByteCount(clearData)];
                byte[] binaryPassword = encoding.GetBytes(clearData);

                // Copy the salt value and the password to the hash buffer.

                binarySaltValue.CopyTo(valueToHash, 0);
                binaryPassword.CopyTo(valueToHash, SaltValueSize);

                byte[] hashValue = hash.ComputeHash(valueToHash);

                // The hashed password is the salt plus the hash value (as a string).

                string hashedPassword = saltValue;

                foreach (byte hexdigit in hashValue)
                {
                    hashedPassword += hexdigit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
                }

                // Return the hashed password as a string.

                return hashedPassword;
            }

            return null;
        }

        private static bool VerifyHashedPassword(string password, string profilePassword, HashAlgorithm hash)
        {
            int saltLength = SaltValueSize * UnicodeEncoding.CharSize;

            if (string.IsNullOrEmpty(profilePassword) ||
                string.IsNullOrEmpty(password) ||
                profilePassword.Length < saltLength)
            {
                return false;
            }

            // Strip the salt value off the front of the stored password.
            string saltValue = profilePassword.Substring(0, saltLength);


            string hashedPassword = HashPassword(password, saltValue, hash);
                if (profilePassword.Equals(hashedPassword, StringComparison.Ordinal))
                    return true;
        
            // None of the hashing algorithms could verify the password.
            return false;
        }
    }

    
}
