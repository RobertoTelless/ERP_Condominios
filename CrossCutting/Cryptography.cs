using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    /// <summary>
    /// 
    /// </summary>
    public static class Cryptography
    {
        /// <summary>
        /// Encodes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static String Encode(String value)
        {
            var hash = System.Security.Cryptography.SHA256.Create();
            var encoder = new ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }

        /// <summary>
        /// Generates the random password.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static String GenerateRandomPassword(Int32 size)
        {
            String allowedCharacters = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            Char[] chars = new Char[size];
            Random rd = new Random();
            for (Int32 i = 0; i < size; i++)
            {
                chars[i] = allowedCharacters[rd.Next(0, allowedCharacters.Length)];
            }
           return new String(chars);
        }
    }
}
