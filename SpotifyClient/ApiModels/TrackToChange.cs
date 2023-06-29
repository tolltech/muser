using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class TrackToChange
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("albumId")]
        public string AlbumId { get; set; }
    }
}