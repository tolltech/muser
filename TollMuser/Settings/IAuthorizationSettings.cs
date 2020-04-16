using System;
using JetBrains.Annotations;

namespace Tolltech.Muser.Settings
{
    public interface IAuthorizationSettings
    {
        [CanBeNull]
        MuserAuthorization SafeGetAndCacheMuserAuthorization([NotNull] string cryptoKey, Guid? userId = null);
        void SetMuserAuthorization([CanBeNull] string cryptoKey, [NotNull] MuserAuthorization authorization, Guid? userId = null);
        [CanBeNull]
        MuserAuthorization GetCachedMuserAuthorization(Guid userId);
        bool UserAuthorized(Guid userId);
    }
}