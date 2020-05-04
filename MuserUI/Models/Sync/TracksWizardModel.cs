using System;

namespace Tolltech.MuserUI.Models.Sync
{
    public class TracksWizardModel
    {
        public TracksWizardModel()
        {
            Tracks = Array.Empty<TrackModel>();
        }

        public TrackModel[] Tracks { get; set; }
    }
}