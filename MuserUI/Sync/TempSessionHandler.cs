using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Sync
{
    public class TempSessionHandler : SqlHandlerBase<TempSessionDbo>
    {
        private readonly DataContextBase<TempSessionDbo> dataContext;

        public TempSessionHandler(DataContextBase<TempSessionDbo> dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        public Task CreateAsync([ItemNotNull] [NotNull] params TempSessionDbo[] sessions)
        {
            return dataContext.Table.AddRangeAsync(sessions);
        }

        [NotNull]
        public Task<TempSessionDbo> FindAsync(Guid id, Guid userId)
        {
            return dataContext.Table.FirstOrDefaultAsync(x => x.Id == id && (!x.UserId.HasValue || x.UserId == userId));
        }
    }
}