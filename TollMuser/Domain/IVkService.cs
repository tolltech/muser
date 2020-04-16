using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public interface IVkService
    {
        [ItemNotNull]
        [NotNull]
        Task<VkTrack[]> GetVkTracksUnauthorizedAsync(string vkUserId);
    }
}