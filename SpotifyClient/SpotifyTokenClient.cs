using System.Net;
using System.Threading.Tasks;
using MusicClientCore;
using Tolltech.Serialization;

namespace Tolltech.SpotifyClient
{
    public class SpotifyTokenClient : ISpotifyTokenClient
    {
        private readonly IJsonSerializer serializer;

        public SpotifyTokenClient(IJsonSerializer serializer)
        {
            this.serializer = serializer;
        }
        
        public async Task<TokenResponse> GetAppToken(string clientId, string clientSecret)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Set("Content-Type", @"application/x-www-form-urlencoded");
                var body = new
                {
                    grant_type = "client_credentials",
                    client_id = clientId,
                    client_secret = clientSecret
                };
                var response = await webClient
                    .UploadDataTaskAsync(@"https://accounts.spotify.com/api/token", body.ToFormData())
                    .ConfigureAwait(false);

                return serializer.Deserialize<TokenResponse>(response);
            }
        }
    }
}