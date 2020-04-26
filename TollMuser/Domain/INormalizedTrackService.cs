using JetBrains.Annotations;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface INormalizedTrackService
    {
        [ItemNotNull]
        [NotNull]
        NormalizedTrack[] GetNormalizedTracks(SourceTrack[] tracks);
    }
}