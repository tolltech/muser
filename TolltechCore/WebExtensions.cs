﻿using System.Net;
using System.Net.Http.Headers;

namespace TolltechCore
{
    public static class WebExtensions
    {
        public static void SetAllowedHeader(this WebHeaderCollection collection, string headerValue)
        {
            collection.Set("Toll-Allowed-Headers", headerValue);
        }
        
        public static void AddAllowedHeader(this HttpRequestHeaders collection, string headerValue)
        {
            collection.Add("Toll-Allowed-Headers", headerValue);
        }
    }
}