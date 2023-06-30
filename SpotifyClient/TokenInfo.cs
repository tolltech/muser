using System;

namespace Tolltech.SpotifyClient
{
    public class TokenInfo
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string RefreshToken { get; set; }
    }
}