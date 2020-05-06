using System;
using System.Collections.Generic;

namespace Tolltech.MuserUI.Models.Sync
{
    public class ProgressModel
    {
        public ProgressModel()
        {
            Errors = new List<(TrackModel Track, string Message)>();
        }

        public Guid Id { get; set; }
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Left => Total - Processed;
        public List<(TrackModel Track, string Message)> Errors { get; set; }
    }
}