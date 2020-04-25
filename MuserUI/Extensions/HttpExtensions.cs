using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Tolltech.MuserUI.Extensions
{
    public static class HttpExtensions
    {
        public static void SetCookies(this HttpResponse response, [NotNull] string name, string val)
        {
            response.Cookies.Append(name, val, new CookieOptions
            {
                Domain = ".tolltech.ru",
                HttpOnly = true
            });
        }

        [CanBeNull]
        public static string FindCookies(this HttpRequest request, string name)
        {
            return request.Cookies[name];
        }
    }
}