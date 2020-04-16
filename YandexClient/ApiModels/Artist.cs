using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class Artist
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}