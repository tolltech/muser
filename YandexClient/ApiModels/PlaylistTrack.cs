using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class PlaylistTrack
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("track")]
        public Track Track { get; set; }
    }
}