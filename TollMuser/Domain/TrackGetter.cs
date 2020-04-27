using System.Linq;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class TrackGetter : ITrackGetter
    {
        private readonly ISpecialTrackGetter[] specialTrackGetters;

        public TrackGetter(IJsonTrackGetter jsonTrackGetter, ISimpleLinesTrackGetter simpleLinesTrackGetter)
        {
            this.specialTrackGetters = new ISpecialTrackGetter[] {jsonTrackGetter, simpleLinesTrackGetter}
                .OrderBy(x => x.Order).ToArray();
        }

        public SourceTrack[] GetTracks(string source)
        {
            foreach (var specialTrackGetter in specialTrackGetters)
            {
                if (specialTrackGetter.TryParseText(source, out var tracks))
                {
                    return tracks;
                }
            }

            throw new MuserDomainException($"Unable to parse tracksInput");
        }
    }
}