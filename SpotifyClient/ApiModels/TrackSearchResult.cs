using System;
using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class TrackSearchResult
    {
        public TrackSearchResult()
        {
            Tracks = Array.Empty<Track>();
        }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("results")]
        public Track[] Tracks { get; set; }
    }
}