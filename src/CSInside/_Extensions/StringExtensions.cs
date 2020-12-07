using System;
using System.Text;

namespace CSInside.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64String(this string str, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(str));
        }

        public static byte[] ToByteArray(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }
    }
}
