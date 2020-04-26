using System.Collections.Generic;

namespace Tolltech.MuserUI.Models.Sync
{
    public class TracksModel
    {
        public TracksModel()
        {
            Tracks = new List<TrackModel>();
        }

        public List<TrackModel> Tracks { get; set; }
    }
}