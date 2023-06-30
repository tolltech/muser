using System;
using System.Linq;
using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class Track
    {
        public Track()
        {
            Artists = Array.Empty<Artist>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("artists")]
        public Artist[] Artists { get; set; }

        [JsonProperty("album")]
        public Album Album { get; set; }

        public string ArtistsStr
        {
            get { return string.Join(", ", Artists?.Select(x => x.Name).ToArray() ?? Array.Empty<string>()); }
        }

        public Album[] Albums => new[] { Album };
    }
}