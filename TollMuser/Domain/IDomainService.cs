using System;
using System.Threading.Tasks;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface IDomainService
    {
        Task<VkTrack[]> GetNewVkTracksUnauthorizedAsync(string yaPlaylistId, Guid? userId, string vkUserId);
        Task<ImportResult[]> ImportTracksAsync(VkTrack[] trackToImport, string playlistId, Guid? userId = null,
            Action<(int Processed, int Total)> percentsComplete = null);
    }
}