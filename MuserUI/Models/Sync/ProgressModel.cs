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
        public Guid? SessionId { get; set; }
        public int Total { get; set; }
        public int Processed { get; set; }
        public bool Submitted { get; set; }
        public bool ImportLogsSaved { get; set; }
        public List<(TrackModel Track, string Message)> Errors { get; set; }
    }
}