using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.Muser.Models;

namespace Tolltech.MuserUI.Sync
{
    public interface IImportResultLogger
    {
        [NotNull]
        Task WriteImportLogsAsync([ItemNotNull] [NotNull] ImportResult[] results, Guid userId, Guid? sessionId);
    }
}