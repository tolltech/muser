using System.Threading.Tasks;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class VkService : IVkService
    {
        public Task<VkTrack[]> GetVkTracksUnauthorizedAsync(string vkUserId)
        {
            throw new System.NotImplementedException();
        }
    }
}