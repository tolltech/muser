using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Tolltech.Muser.Settings;
using Tolltech.Musync.Domain;
using Tolltech.Serialization;
using Tolltech.SpotifyClient;
using Vostok.Logging.Abstractions;

namespace Tolltech.Muser.Domain
{
    public class YandexService : IYandexService
    {
        private static readonly ILog log = LogProvider.Get();

        private readonly IAuthorizationSettings authorizationSettings;
        private readonly IJsonSerializer serializer;

        private static readonly ConcurrentDictionary<string, ISpotifyApiClient> yaClients =
            new ConcurrentDictionary<string, ISpotifyApiClient>();

        public YandexService(IAuthorizationSettings authorizationSettings, IJsonSerializer serializer)
        {
            this.authorizationSettings = authorizationSettings;
            this.serializer = serializer;
        }

        public ISpotifyApiClient GetClientAsync(Guid userId)
        {
            return InnerGetClientAsync(userId);
        }

        [ItemNotNull]
        private ISpotifyApiClient InnerGetClientAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new YaAuthorizeException();
            }

            var client = yaClients.GetOrAdd(accessToken, CreateClientAsync);

            if (client == null)
            {
                throw new YaAuthorizeException();
            }

            return client;
        }
        
        [ItemNotNull]
        private ISpotifyApiClient InnerGetClientAsync(Guid userId)
        {
            var accessToken = authorizationSettings.GetCachedMuserAuthorization(userId)?.SpotifyAccessToken;

            log.Info($"Try to create yandex client for user {userId}");

            return InnerGetClientAsync(accessToken);
        }

        private ISpotifyApiClient CreateClientAsync(string accessToken)
        {
            return new SpotifyApiClient(accessToken, serializer);
        }
    }
}