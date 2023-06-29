using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class Album
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}