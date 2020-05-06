using System;
using System.Linq;
using System.Threading.Tasks;
using Tolltech.Muser.Models;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Sync
{
    public class ImportResultLogger : IImportResultLogger
    {
        private readonly IQueryExecutorFactory queryExecutorFactory;

        public ImportResultLogger(IQueryExecutorFactory queryExecutorFactory)
        {
            this.queryExecutorFactory = queryExecutorFactory;
        }

        public async Task WriteImportLogsAsync(ImportResult[] results, Guid userId, Guid? sessionId)
        {
            var now = DateTime.Now;
            sessionId ??= Guid.NewGuid();
            var importResults = results.Select(x => new ImportResultDbo
            {
                Id = Guid.NewGuid(),
                Date = now,
                UserId = userId,
                SessionId = sessionId.Value,
                Title = x.ImportingTrack.Title,
                Artist = x.ImportingTrack.Artist,
                CandidateArtist = x.Candidate?.Artist ?? string.Empty,
                CandidateTitle = x.Candidate?.Title ?? string.Empty,
                NormalizedArtist = x.NormalizedTrack?.Artist ?? string.Empty,
                NormalizedTitle = x.NormalizedTrack?.Title ?? string.Empty,
                Message = x.Message ?? string.Empty,
                Status = x.ImportStatus,
                ApprovedManual = false,
                CandidateAlbumId = x.CandidateAlbumId,
                CandidateTrackId = x.CandidateTrackId,
                PlaylistId = x.PlaylistId
            }).ToArray();

            using var queryExecutor = queryExecutorFactory.Create<ImportResultHandler, ImportResultDbo>();
            await queryExecutor.ExecuteAsync(x => x.CreateAsync(importResults)).ConfigureAwait(false);
        }

        public async Task<ImportResultDbo[]> SelectAsync(Guid sessionId, Guid userId, params ImportStatus[] statuses)
        {
            using var queryExecutor = queryExecutorFactory.Create<ImportResultHandler, ImportResultDbo>();
            var result = await queryExecutor.ExecuteAsync(x => x.SelectAsync(sessionId, userId, statuses)).ConfigureAwait(false);
            return result;
        }

        public async Task UpdateManualApprovingAsync(Guid[] ids)
        {
            using var queryExecutor = queryExecutorFactory.Create<ImportResultHandler, ImportResultDbo>();
            var result = await queryExecutor.ExecuteAsync(x => x.SelectAsync(ids)).ConfigureAwait(false);
            foreach (var importResultDbo in result)
            {
                importResultDbo.ApprovedManual = true;
                importResultDbo.Status = ImportStatus.Ok;
            }

            await queryExecutor.ExecuteAsync(x => x.UpdateAsync()).ConfigureAwait(false);
        }
    }
}