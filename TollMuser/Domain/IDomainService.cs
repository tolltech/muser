using System;
using System.Threading.Tasks;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface IDomainService
    {
        Task<NormalizedTrack[]> GetNewTracksAsync(string yaPlaylistId, Guid? userId,
            SourceTrack[] inputTracks);
        Task<ImportResult[]> ImportTracksAsync(NormalizedTrack[] trackToImport, string playlistId, Guid? userId = null,
            Action<(int Processed, int Total, ImportResult importResult)> percentsComplete = null);
    }
}