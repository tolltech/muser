using System;

namespace Tolltech.YandexClient
{
    public class YandexApiException : Exception
    {
        public YandexApiException(string msg) : base(msg)
        {
        }
    }
}