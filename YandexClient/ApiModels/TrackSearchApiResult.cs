using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class TrackSearchApiResult
    {
        [JsonProperty("tracks")]
        public TrackSearchResult Tracks { get; set; }
    }
}