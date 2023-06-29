using Microsoft.Extensions.Configuration;
using Tolltech.SpotifyClient.Integration;

namespace Tolltech.MuserUI.Spotify
{
    public class SpotifyClientConfiguration : ISpotifyClientConfiguration
    {
        private readonly IConfiguration configuration;

        public SpotifyClientConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string ClientId => configuration["spotifyClientId"];
        public string ClientSecret => configuration["spotifyClientSecret"];
    }
}