using System;
using System.Threading.Tasks;
using Tolltech.SpotifyClient;
using Tolltech.YandexClient;

namespace Tolltech.Muser.Domain
{
    public interface IYandexService
    {
        Task<ISpotifyApiClient> GetClientAsync(Guid userId);
    }
}