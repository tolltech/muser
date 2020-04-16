using Newtonsoft.Json;

namespace Tolltech.YandexClient.Authorizations
{
    public class FirstStepResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}