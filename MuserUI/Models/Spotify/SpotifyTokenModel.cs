using System;

namespace Tolltech.MuserUI.Models.Spotify
{
    public class SpotifyTokenModel
    {
        public bool LoggedIn { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public DateTime ExpiresUtc { get; set; }
        
    }
}