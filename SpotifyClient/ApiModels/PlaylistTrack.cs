using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class PlaylistTrack
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("track")]
        public Track Track { get; set; }
    }
}