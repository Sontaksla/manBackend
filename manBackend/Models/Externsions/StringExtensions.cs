using System.Security.Cryptography;

namespace manBackend.Models.Externsions
{
    public static class StringExtensions
    {
        public static string HashSha256(this string str) 
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(str));

            StringBuilder sb = new StringBuilder(hashBytes.Length);

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2")); // (hexadecimal)
            }

            return sb.ToString();
        }
    }
}
