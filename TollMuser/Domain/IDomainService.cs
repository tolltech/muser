using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.Muser.Models;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.Muser.Domain
{
    public interface IDomainService
    {
        Task<NormalizedTrack[]> GetNewTracksAsync(string yaPlaylistId, Guid? userId,
            SourceTrack[] inputTracks);

        Task<ImportResult[]> ImportTracksAsync(NormalizedTrack[] trackToImport, string playlistId, Guid? userId = null,
            Action<(int Processed, int Total, ImportResult importResult)> percentsComplete = null);

        [NotNull] Task ImportTracksAsync(TrackToChange[] trackToImport, string playlistId, Guid? userId);
    }
}