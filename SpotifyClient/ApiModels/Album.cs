using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class Album
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }
    }
}