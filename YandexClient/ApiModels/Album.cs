using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class Album
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}