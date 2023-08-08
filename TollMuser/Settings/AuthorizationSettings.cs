using System;
using System.Collections.Concurrent;

namespace Tolltech.Muser.Settings
{
    public class AuthorizationSettings : IAuthorizationSettings
    {
        private static readonly ConcurrentDictionary<Guid, MuserAuthorization> authorizations =
            new ConcurrentDictionary<Guid, MuserAuthorization>();

        public void SetMuserAuthorization(MuserAuthorization authorization, Guid userId)
        {
            authorizations.AddOrUpdate(userId, authorization, (guid, muserAuthorization) => authorization);
        }

        public void DeleteMuserAuthorization(Guid userId)
        {
            authorizations.TryRemove(userId, out _);
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