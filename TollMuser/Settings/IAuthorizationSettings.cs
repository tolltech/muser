using System;
using JetBrains.Annotations;

namespace Tolltech.Muser.Settings
{
    public interface IAuthorizationSettings
    {
        [CanBeNull]
        MuserAuthorization SafeGetAndCacheMuserAuthorization(Guid? userId = null);
        void SetMuserAuthorization([NotNull] MuserAuthorization authorization, Guid? userId = null);
        [CanBeNull]
        MuserAuthorization GetCachedMuserAuthorization(Guid userId);
        bool UserAuthorized(Guid userId);
    }
}