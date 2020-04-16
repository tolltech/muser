using System;
using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class Track
    {
        public Track()
        {
            Artists = Array.Empty<Artist>();
            Albums = Array.Empty<Album>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("storageDir")]
        public string StorageDir { get; set; }

        [JsonProperty("artists")]
        public Artist[] Artists { get; set; }

        [JsonProperty("albums")]
        public Album[] Albums { get; set; }
    }
}