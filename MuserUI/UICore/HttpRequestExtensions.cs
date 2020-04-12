using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Tolltech.MuserUI.UICore
{
    public static class HttpRequestExtensions
    {
        [NotNull]
        public static string[] GetAcceptHeaders([CanBeNull] this HttpRequest request)
        {
            return request?.Headers.TryGetValue("Accept", out var val) ?? false ? val.ToArray() : Array.Empty<string>();
        }
    }
}