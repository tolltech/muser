using System;
using System.Collections.Concurrent;
using Tolltech.MuserUI.Models.Sync;

namespace Tolltech.MuserUI.Sync
{
    public class ProgressBar : IProgressBar
    {
        private static readonly ConcurrentDictionary<Guid, ProgressModel> progressDict = new ConcurrentDictionary<Guid, ProgressModel>();

        public ProgressModel GetProgressModel(Guid progressId)
        {
            return progressDict.TryGetValue(progressId, out var result) ? result : null;
        }

        public void UpdateProgressModel(ProgressModel progress)
        {
            progressDict.AddOrUpdate(progress.Id, progress, (guid, model) => progress);
        }
    }
}