using System;
using Tolltech.MuserUI.Models.Sync;

namespace Tolltech.MuserUI.Models.SyncWizard
{
    public class ProgressWithUrlModel
    {
        public ProgressModel Progress { get; set; }
        public string YandexPlaylistUrl { get; set; }
        public Guid SessionId { get; set; }
    }
}