using System;
using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class Playlist
    {
        public Playlist()
        {
            Tracks = Array.Empty<PlaylistTrack>();
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("kind")]
        public string Id { get; set; }

        [JsonProperty("revision")]
        public string Revision { get; set; }

        [JsonProperty("modified")]
        public DateTime ModifyDate { get; set; }

        [JsonProperty("created")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("tracks")]
        public PlaylistTrack[] Tracks { get; set; }
    }
}