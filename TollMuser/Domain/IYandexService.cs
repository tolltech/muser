using System;
using System.Threading.Tasks;
using Tolltech.YandexClient;

namespace Tolltech.Muser.Domain
{
    public interface IYandexService
    {
        Task<bool> CheckCredentialsAsync(string login, string password);
        Task<IYandexMusicClient> GetClientAsync(Guid? userId = null);
    }
}