using System;

namespace Tolltech.MuserUI.Models.SyncWizard
{
    public class ReImportModel
    {
        public ReImportModel()
        {
            Tracks = Array.Empty<ReImportTrack>();
        }

        public ReImportTrack[] Tracks { get; set; }
        public Guid SessionId { get; set; }
        public string PlaylistId { get; set; }
        public string PlaylistUrl { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
    }
}