using System;

namespace Tolltech.MuserUI.Models.SyncWizard
{
    public class ReImportTrack
    {
        public string InputTitle { get; set; }
        public string InputArtist { get; set; }
        public string TrackId { get; set; }
        public string AlbumId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public bool Selected { get; set; }
        public Guid Id { get; set; }
        public bool Disabled { get; set; }
    }
}