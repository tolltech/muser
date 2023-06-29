using System.Net;
using System.Threading.Tasks;
using Tolltech.Serialization;

namespace Tolltech.SpotifyClient
{
    public class SpotifyApiClient : ISpotifyApiClient
    {
        private readonly TokenInfo tokenInfo;
        private readonly IJsonSerializer serializer;

        public SpotifyApiClient(TokenInfo tokenInfo, ISpotifyTokenClient spotifyTokenClient, IJsonSerializer serializer)
        {
            this.tokenInfo = tokenInfo;
            this.serializer = serializer;
        }
        
        public async Task<ArtistInfo> GetArtist(string artistId)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Set("Authorization", $@"Bearer  {tokenInfo.AccessToken}");
                var response = await webClient
                    .DownloadDataTaskAsync($@"https://api.spotify.com/v1/artists/{artistId}")
                    .ConfigureAwait(false);

                return serializer.Deserialize<ArtistInfo>(response);
            }
        }
    }
}