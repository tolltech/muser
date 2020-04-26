using JetBrains.Annotations;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface ITrackGetter
    {
        [NotNull] [ItemNotNull] SourceTrack[] GetTracks([CanBeNull] string source);
    }
}