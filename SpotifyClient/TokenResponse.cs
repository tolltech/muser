using Newtonsoft.Json;

namespace Tolltech.SpotifyClient
{
    public class TokenResponse
    {
        // {
        //     "access_token": "NgCXRK...MzYjw",
        //     "token_type": "Bearer",
        //     "scope": "user-read-private user-read-email",
        //     "expires_in": 3600,
        //     "refresh_token": "NgAagA...Um_SHo"
        // }

        [JsonProperty("access_token")] public string AccessToken { get; set; }
        [JsonProperty("token_type")] public string TokenType { get; set; }
        [JsonProperty("scope")] public string Scope { get; set; }
        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
    }
}