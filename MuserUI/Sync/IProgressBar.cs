using System;
using JetBrains.Annotations;
using Tolltech.MuserUI.Models.Sync;

namespace Tolltech.MuserUI.Sync
{
    public interface IProgressBar
    {
        [CanBeNull]
        ProgressModel FindProgressModel(Guid progressId);
        void UpdateProgressModel([NotNull]ProgressModel progress);
    }
}