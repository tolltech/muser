using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface ISpecialTrackGetter
    {
        int Order { get; }
        bool TryParseText(string text, out SourceTrack[] tracks);
    }
}