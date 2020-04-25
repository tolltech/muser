using System;
using System.Linq;
using System.Threading.Tasks;
using Tolltech.Muser.Models;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Sync
{
    public class ImportResultLogger : IImportResultLogger
    {
        private readonly IQueryExecutorFactory<ImportResultHandler, ImportResultDbo> queryExecutorFactory;

        public ImportResultLogger(IQueryExecutorFactory<ImportResultHandler,ImportResultDbo> queryExecutorFactory)
        {
            this.queryExecutorFactory = queryExecutorFactory;
        }

        public Task WriteImportLogsAsync(ImportResult[] results, Guid userId)
        {
            var now = DateTime.Now;
            var sessionId = Guid.NewGuid();
            var importResults = results.Select(x => new ImportResultDbo
            {
                Id = Guid.NewGuid(),
                Date = now,
                UserId = userId,
                SessionId = sessionId,
                Title = x.ImportingTrack.Title,
                Artist = x.ImportingTrack.Artist,
                CandidateArtist = x.Candidate?.Artist ?? string.Empty,
                CandidateTitle = x.Candidate?.Title ?? string.Empty,
                NormalizedArtist = x.NormalizedTrack?.Artist ?? string.Empty,
                NormalizedTitle = x.NormalizedTrack?.Title ?? string.Empty,
                Message = x.Message ?? string.Empty,
                Status = x.ImportStatus
            }).ToArray();

            using var queryExecutor = queryExecutorFactory.Create();
            return queryExecutor.ExecuteAsync(x => x.CreateAsync(importResults));
        }
    }
}