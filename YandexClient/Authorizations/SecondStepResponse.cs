using Newtonsoft.Json;

namespace Tolltech.YandexClient.Authorizations
{
    public class SecondStepResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("expires_in")]
        public long Expires { get; set; }
    }
}