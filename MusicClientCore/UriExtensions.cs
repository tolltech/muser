using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MusicClientCore
{
    public static class UriExtensions
    {
        public static string ToUriParams(this Dictionary<string,string> parameters)
        {
            return string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
        }

        public static string ToFormDataStr(this object obj)
        {
            //todo: reflection cache
            var props = obj.GetType().GetProperties();
            return props.ToDictionary(x => x.Name, x => x.GetValue(obj)?.ToString()).ToUriParams();
        }

        public static byte[] ToFormData(this object obj)
        {
            return obj == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(obj.ToFormDataStr());
        }

        public static string Decode(this string src)
        {
            return WebUtility.HtmlDecode(src);
        }
    }
}