using System;
using JetBrains.Annotations;

namespace Tolltech.Muser.Settings
{
    public interface IAuthorizationSettings
    {
        void SetMuserAuthorization([NotNull] MuserAuthorization authorization, Guid userId);
        [CanBeNull]
        MuserAuthorization GetCachedMuserAuthorization(Guid userId);
        bool UserAuthorized(Guid userId);
    }
}