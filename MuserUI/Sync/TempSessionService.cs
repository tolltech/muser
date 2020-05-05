using System;
using System.Threading.Tasks;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Sync
{
    public class TempSessionService : ITempSessionService
    {
        private readonly IQueryExecutorFactory queryExecutorFactory;

        public TempSessionService(IQueryExecutorFactory queryExecutorFactory)
        {
            this.queryExecutorFactory = queryExecutorFactory;
        }

        public async Task SaveTempSessionAsync(Guid sessionId, Guid? userId, string inputTracksText)
        {
            var sessionDbo = new TempSessionDbo
            {
                UserId = userId,
                Text = inputTracksText,
                Date = DateTime.UtcNow,
                Id = sessionId
            };

            using var queryExecutor = queryExecutorFactory.Create<TempSessionHandler, TempSessionDbo>();
            await queryExecutor.ExecuteAsync(x => x.CreateAsync(sessionDbo)).ConfigureAwait(false);
        }

        public async Task<string> FindSessionTextAsync(Guid sessionId, Guid userId)
        {
            using var queryExecutor = queryExecutorFactory.Create<TempSessionHandler, TempSessionDbo>();
            var savedTracks = await queryExecutor.ExecuteAsync(x => x.FindAsync(sessionId, userId)).ConfigureAwait(false);
            return savedTracks?.Text;
        }
    }
}