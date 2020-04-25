using System.Threading.Tasks;
using Tolltech.MuserUI.Models.Sync;

namespace Tolltech.MuserUI.Sync
{
    public interface ITrackGetter
    {
        Task<TracksModel> GetTracksAsync(string source);
        string GetAudioUrl(string source);
    }
}