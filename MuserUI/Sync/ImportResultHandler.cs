﻿using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tolltech.Muser.Models;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Sync
{
    public class ImportResultHandler : SqlHandlerBase<ImportResultDbo>
    {
        private readonly DataContextBase<ImportResultDbo> dataContext;

        public ImportResultHandler(DataContextBase<ImportResultDbo> dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        public Task CreateAsync([NotNull] [ItemNotNull] params ImportResultDbo[] importResults)
        {
            return dataContext.Table.AddRangeAsync(importResults);
        }

        [NotNull]
        [ItemNotNull]
        public Task<ImportResultDbo[]> SelectAsync(Guid sessionId, Guid userId, params ImportStatus[] statuses)
        {
            var query = dataContext.Table.Where(x => x.SessionId == sessionId && x.UserId == userId);

            if (statuses?.Length > 0)
            {
                query = query.Where(x => statuses.Contains(x.Status));
            }

            return query.ToArrayAsync();
        }

        [NotNull]
        [ItemNotNull]
        public Task<ImportResultDbo[]> SelectAsync(Guid[] ids)
        {
            return dataContext.Table
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
        }

        [NotNull]
        public Task UpdateAsync()
        {
            return dataContext.SaveChangesAsync();
        }

        [NotNull]
        public Task<int> CountAsync(Guid sessionId, Guid userId)
        {
            return dataContext.Table
                .Where(x => x.SessionId == sessionId && x.UserId == userId)
                .CountAsync();
        }
    }
}