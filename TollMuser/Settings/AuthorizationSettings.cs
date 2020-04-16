using System;
using System.Collections.Concurrent;
using Tolltech.Serialization;

namespace Tolltech.Muser.Settings
{
    public class AuthorizationSettings : IAuthorizationSettings
    {
        private readonly IJsonSerializer serializer;
        private readonly ICryptoService cryptoService;

        private static readonly ConcurrentDictionary<Guid, MuserAuthorization> authorizations =
            new ConcurrentDictionary<Guid, MuserAuthorization>();

        public AuthorizationSettings(IJsonSerializer serializer, ICryptoService cryptoService)
        {
            this.serializer = serializer;
            this.cryptoService = cryptoService;
        }

        public MuserAuthorization SafeGetAndCacheMuserAuthorization(string cryptoKey, Guid? userId = null)
        {
            return authorizations.TryGetValue(userId ?? Guid.Empty, out var authorization) 
                ? authorization 
                : null;
        }

        public void SetMuserAuthorization(string cryptoKey, MuserAuthorization authorization, Guid? userId = null)
        {
            authorizations.AddOrUpdate(userId ?? Guid.Empty, authorization, (guid, muserAuthorization) => authorization);
        }

        public MuserAuthorization GetCachedMuserAuthorization(Guid userId)
        {
            return authorizations.TryGetValue(userId, out var auth) ? auth : null;
        }

        public bool UserAuthorized(Guid userId)
        {
            return authorizations.ContainsKey(userId);
        }
    }
}