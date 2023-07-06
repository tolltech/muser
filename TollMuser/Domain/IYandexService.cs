using System;
using Tolltech.SpotifyClient;

namespace Tolltech.Muser.Domain
{
    public interface IYandexService
    {
        ISpotifyApiClient GetClientAsync(Guid userId);
    }
}