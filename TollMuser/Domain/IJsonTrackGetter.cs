using JetBrains.Annotations;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface IJsonTrackGetter : ISpecialTrackGetter
    {
        [NotNull] string SerializeTracks([NotNull] [ItemNotNull] SourceTrack[] tracks);
    }
}