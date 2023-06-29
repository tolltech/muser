using System;

namespace TolltechCore
{
    public static class Extensions
    {
        public static string ToBase64(this string plainText) 
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}