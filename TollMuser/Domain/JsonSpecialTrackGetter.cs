using System;
using System.Linq;
using Tolltech.Muser.Models;
using Tolltech.Serialization;

namespace Tolltech.Muser.Domain
{
    public class JsonSpecialTrackGetter : IJsonTrackGetter
    {
        private class TrackJson
        {
            public string Artist { get; set; }
            public string Song { get; set; }
        }

        private readonly IJsonSerializer jsonSerializer;

        public JsonSpecialTrackGetter(IJsonSerializer jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public int Order => 1;

        public bool TryParseText(string text, out SourceTrack[] tracks)
        {
            try
            {
                var parsedTracks = jsonSerializer.DeserializeFromString<TrackJson[]>(text);

                tracks = parsedTracks.Select(x => new SourceTrack
                    {
                        Title = x.Song,
                        Artist = x.Artist
                    })
                    .ToArray();

                return true;
            }
            catch (Exception e)
            {
                tracks = Array.Empty<SourceTrack>();
                return false;
            }
        }
    }
}