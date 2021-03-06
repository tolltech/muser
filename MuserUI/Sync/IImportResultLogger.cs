﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.Muser.Models;

namespace Tolltech.MuserUI.Sync
{
    public interface IImportResultLogger
    {
        [NotNull]
        Task WriteImportLogsAsync([ItemNotNull] [NotNull] ImportResult[] results, Guid userId, Guid? sessionId);

        [NotNull]
        [ItemNotNull]
        Task<ImportResultDbo[]> SelectAsync(Guid sessionId, Guid userId, params ImportStatus[] statuses);

        [NotNull]
        Task UpdateManualApprovingAsync([NotNull] Guid[] ids);

        [NotNull] Task<int> CountAsync(Guid sessionId, Guid userId);
    }
}