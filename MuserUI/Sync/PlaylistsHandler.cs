using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Sync
{
    public class PlaylistsHandler : SqlHandlerBase<PlaylistDbo>
    {
        private readonly DataContextBase<PlaylistDbo> dataContext;

        public PlaylistsHandler(DataContextBase<PlaylistDbo> dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        public Task CreateAsync([NotNull] [ItemNotNull] params PlaylistDbo[] playlists)
        {
            return dataContext.Table.AddRangeAsync(playlists);
        }
    }
}