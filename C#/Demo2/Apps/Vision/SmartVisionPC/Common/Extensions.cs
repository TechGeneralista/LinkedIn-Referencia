using System;
using System.Security.Cryptography;
using System.Text;

namespace SmartVisionClientApp.Common
{
    public static class Extensions
    {
        public static bool IsNotNull(this object o) => o != null;
        public static bool IsNull(this object o) => o == null;
        public static string NewGuid(int charCount) => Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0,charCount).ToUpper();

        public static string CreateMD5(this string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string CreateMD5(this byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
