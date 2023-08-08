using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.SpotifyClient;

namespace Tolltech.MuserUI.Spotify
{
    public interface ISpotifyTokenService
    {
        Task CreateOrUpdate([NotNull] TokenInfo token, Guid userId);
        [ItemCanBeNull] [NotNull] Task<TokenInfo> Find(Guid userId);
        [NotNull] Task Delete(Guid userId);
    }
}