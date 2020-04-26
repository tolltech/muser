using System.Linq;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class NormalizedTrackService : INormalizedTrackService
    {
        public NormalizedTrack[] GetNormalizedTracks(SourceTrack[] tracks)
        {
            return tracks
                .Select(x => new NormalizedTrack
                {
                    Title = x.Title,
                    Artist = x.Artist
                })
                .ToArray();
        }
    }
}