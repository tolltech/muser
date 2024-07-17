using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using MusicClientCore;
using Tolltech.Serialization;
using Tolltech.SpotifyClient.Integration;
using TolltechCore;

namespace Tolltech.SpotifyClient
{
    public class SpotifyTokenClient : ISpotifyTokenClient
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly IJsonSerializer serializer;
        private readonly ISpotifyClientConfiguration spotifyClientConfiguration;

        private string ClientId => spotifyClientConfiguration.ClientId;
        private string ClientSecret => spotifyClientConfiguration.ClientSecret;

        public SpotifyTokenClient(IJsonSerializer serializer, ISpotifyClientConfiguration spotifyClientConfiguration)
        {
            this.serializer = serializer;
            this.spotifyClientConfiguration = spotifyClientConfiguration;
        }

        //private static readonly string domain = "https://accounts.spotify.com";
        private static readonly string domain = "http://10.7.0.1:5000";

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
                    .UploadDataTaskAsync($@"{domain}/api/token", body.ToFormData())
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
                webClient.Headers.SetAllowedHeader("Authorization");
                webClient.Headers.SetDomainHeader("accounts.spotify.com");
                var body = new
                {
                    code = authCode,
                    redirect_uri = @"https://tolltech.ru/spotify/callback",
                    grant_type = "authorization_code"
                };
                var response = await webClient
                    .UploadDataTaskAsync($@"{domain}/api/token", body.ToFormData())
                    .ConfigureAwait(false);

                return serializer.Deserialize<TokenResponse>(response);
            }
        }

        public async Task<TokenResponse> RefreshToken(string refreshToken)
        {
            var body = new
            {
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };
            
            var stringContent = new StringContent(body.ToFormDataStr());
            
            stringContent.Headers.ContentType = new MediaTypeHeaderValue(@"application/x-www-form-urlencoded");
            stringContent.Headers.AddAllowedHeader("Authorization");
            stringContent.Headers.AddDomainHeader("accounts.spotify.com");
            stringContent.Headers.Add("Toll-Full-Log", "true");
            var response = await client
                .PostAsync($@"{domain}/api/token", stringContent)
                .ConfigureAwait(false);

            var responseStr = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var msg = $"{response.StatusCode} {responseStr} {response}";
                Console.WriteLine(msg);
                throw new WebException(msg);
            }

            return serializer.Deserialize<TokenResponse>(responseStr);
        }
    }
}