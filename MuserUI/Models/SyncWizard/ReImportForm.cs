using System;

namespace Tolltech.MuserUI.Models.SyncWizard
{
    public class ReImportForm
    {
        public ReImportForm()
        {
            Tracks = Array.Empty<ReImportTrackForm>();
        }

        public Guid SessionId { get; set; }
        public ReImportTrackForm[] Tracks { get; set; }
        public string PlaylistId { get; set; }
    }
}