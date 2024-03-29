using System.Net;
using System.Threading.Tasks;
using MusicClientCore;
using Tolltech.Serialization;
using Tolltech.SpotifyClient.Integration;
using TolltechCore;

namespace Tolltech.SpotifyClient
{
    public class SpotifyTokenClient : ISpotifyTokenClient
    {
        private readonly IJsonSerializer serializer;
        private readonly ISpotifyClientConfiguration spotifyClientConfiguration;

        private string ClientId => spotifyClientConfiguration.ClientId;
        private string ClientSecret => spotifyClientConfiguration.ClientSecret;

        public SpotifyTokenClient(IJsonSerializer serializer, ISpotifyClientConfiguration spotifyClientConfiguration)
        {
            this.serializer = serializer;
            this.spotifyClientConfiguration = spotifyClientConfiguration;
        }
        
        public async Task<TokenResponse> GetAppToken()
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Set("Content-Type", @"application/x-www-form-urlencoded");
                var body = new
                {
                    grant_type = "client_credentials",
                    client_id = ClientId,
                    client_secret = ClientSecret
                };
                var response = await webClient
                    .UploadDataTaskAsync(@"https://accounts.spotify.com/api/token", body.ToFormData())
                    .ConfigureAwait(false);

                return serializer.Deserialize<TokenResponse>(response);
            }
        }

        public async Task<TokenResponse> ExchangeToken(string authCode)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Set("Content-Type", @"application/x-www-form-urlencoded");
                webClient.Headers.Set("Authorization", $@"Basic {$"{ClientId}:{ClientSecret}".ToBase64()}");
                var body = new
                {
                    code = authCode,
                    redirect_uri = @"https://tolltech.ru/spotify/callback",
                    grant_type = "authorization_code"
                };
                var response = await webClient
                    .UploadDataTaskAsync(@"https://accounts.spotify.com/api/token", body.ToFormData())
                    .ConfigureAwait(false);

                return serializer.Deserialize<TokenResponse>(response);
            }
        }

        public async Task<TokenResponse> RefreshToken(string refreshToken)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Set("Content-Type", @"application/x-www-form-urlencoded");
                webClient.Headers.Set("Authorization", $@"Basic {$"{ClientId}:{ClientSecret}".ToBase64()}");
                var body = new
                {
                    grant_type = "refresh_token",
                    refresh_token = refreshToken
                };
                var response = await webClient
                    .UploadDataTaskAsync(@"https://accounts.spotify.com/api/token", body.ToFormData())
                    .ConfigureAwait(false);

                return serializer.Deserialize<TokenResponse>(response);
            }
        }
    }
}