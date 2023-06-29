using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tolltech.MuserUI.Spotify;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Sync
{
    public class SpotifyTokenHandler : SqlHandlerBase<SpotifyTokenDbo>
    {
        private readonly DataContextBase<SpotifyTokenDbo> dataContext;

        public SpotifyTokenHandler(DataContextBase<SpotifyTokenDbo> dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        public Task CreateAsync([ItemNotNull] [NotNull] params SpotifyTokenDbo[] sessions)
        {
            return dataContext.Table.AddRangeAsync(sessions);
        }

        [NotNull]
        public Task<SpotifyTokenDbo> FindAsync(Guid userId)
        {
            return dataContext.Table.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        
        [NotNull]
        public Task UpdateAsync()
        {
            return dataContext.SaveChangesAsync();
        }
    }
}