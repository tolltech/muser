using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tolltech.MuserUI.Sync
{
    public interface ITempSessionService
    {
        [NotNull]
        Task SaveTempSessionAsync(Guid sessionId, Guid? userId, [CanBeNull] string inputTracksText);

        [NotNull]
        [ItemCanBeNull]
        Task<string> FindSessionTextAsync(Guid sessionId, Guid userId);
    }
}