using System;
using Newtonsoft.Json;

namespace Tolltech.SpotifyClient
{
    public class ArtistInfo
    {
        [JsonProperty("id")] public string Id { get; set; }
    }
}