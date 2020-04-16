using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class TrackToChange
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("albumId")]
        public string AlbumId { get; set; }
    }
}