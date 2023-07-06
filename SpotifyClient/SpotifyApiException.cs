using System;

namespace Tolltech.SpotifyClient
{
    public class SpotifyApiException : Exception
    {
        public SpotifyApiException(string msg) : base(msg)
        {
        }
    }
}