using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class Playlist
    {
        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("snapshot_id")]
        public string Revision { get; set; }
    }
}