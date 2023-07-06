using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.Muser.Models;
using Tolltech.SpotifyClient.ApiModels;

namespace Tolltech.Muser.Domain
{
    public interface IDomainService
    {
        Task<ImportResult[]> ImportTracksAsync(NormalizedTrack[] trackToImport, string playlistId, Guid userId,
            Action<(int Processed, int Total, ImportResult importResult, bool submitted)> percentsComplete = null);

        [NotNull]
        Task ImportTracksAsync(TrackToChange[] trackToImport, string playlistId, Guid userId);

        [ItemNotNull]
        [NotNull]
        Task<YandexTrack[]> GetExistentTracksAsync(Guid userId, string playlistId);
    }
}