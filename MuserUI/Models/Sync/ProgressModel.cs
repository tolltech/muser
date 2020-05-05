using System;

namespace Tolltech.MuserUI.Models.Sync
{
    public class ProgressModel
    {
        public Guid Id { get; set; }
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Left => Total - Processed;
        public bool Initial { get; set; }
        public int Skipped { get; set; }
        public TrackModel[] Errors { get; set; }
    }
}