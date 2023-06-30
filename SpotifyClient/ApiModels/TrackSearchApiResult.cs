using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class TrackSearchApiResult
    {
        [JsonProperty("tracks")]
        public TrackSearchResult Tracks { get; set; }
    }
}