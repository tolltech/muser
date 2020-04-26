using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using JetBrains.Annotations;
using log4net;
using Tolltech.Muser.Settings;
using Tolltech.Musync.Domain;
using Tolltech.Serialization;
using Tolltech.YandexClient;
using Tolltech.YandexClient.Authorizations;

namespace Tolltech.Muser.Domain
{
    public class YandexService : IYandexService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(YandexService));

        private readonly IAuthorizationSettings authorizationSettings;
        private readonly IJsonSerializer serializer;

        private static readonly ConcurrentDictionary<(string, string), Task<IYandexMusicClient>> yaClients =
            new ConcurrentDictionary<(string, string), Task<IYandexMusicClient>>();

        public YandexService(IAuthorizationSettings authorizationSettings, IJsonSerializer serializer)
        {
            this.authorizationSettings = authorizationSettings;
            this.serializer = serializer;
        }

        public async Task<bool> CheckCredentialsAsync(string login, string password)
        {
            try
            {
                await InnerGetClientAsync(login, password).ConfigureAwait(false);
            }
            catch (YaAuthorizeException)
            {
                return false;
            }

            return true;
        }

        public Task<IYandexMusicClient> GetClientAsync(Guid? userId = null)
        {
            return InnerGetClientAsync(userId ?? Guid.Empty);
        }

        [ItemNotNull]
        private async Task<IYandexMusicClient> InnerGetClientAsync(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                throw new YaAuthorizeException();
            }

            var client =  await yaClients.GetOrAdd((login, password), tuple => CreateClientAsync(tuple.Item1, tuple.Item2));

            if (client == null)
            {
                throw new VkAuthorizeException();
            }

            return client;
        }
        
        [ItemNotNull]
        private Task<IYandexMusicClient> InnerGetClientAsync(Guid userId)
        {
            var login = authorizationSettings.GetCachedMuserAuthorization(userId)?.YaLogin;
            var password = authorizationSettings.GetCachedMuserAuthorization(userId)?.YaPassword;

            log.Info($"Try to create yandex client for user {userId} and login {login}");

            return InnerGetClientAsync(login, password);
        }

        [ItemCanBeNull]
        private async Task<IYandexMusicClient> CreateClientAsync(string login, string password)
        {
            var yandexCredentials = new YandexCredentials(serializer, login, password);
            try
            {
                var authorizationInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }

            return new YandexMusicClient(yandexCredentials, serializer);
        }
    }
}