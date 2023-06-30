using System;
using System.Threading.Tasks;
using Tolltech.SpotifyClient;
using Tolltech.YandexClient;

namespace Tolltech.Muser.Domain
{
    public interface IYandexService
    {
        ISpotifyApiClient GetClientAsync(Guid userId);
    }
}