﻿namespace Tolltech.Muser.Models
{
    public class ImportResult
    {
        public ImportResult(string artist, string title)
        {
            this.ImportingTrack = new ImportingTrack()
            {
                Artist = artist,
                Title = title
            };
        }

        public ImportStatus ImportStatus { get; set; }

        public string Message { get; set; }

        public ImportingTrack ImportingTrack { get; }

        public ImportingTrack NormalizedTrack { get; set; }

        public ImportingTrack Candidate { get; set; }
    }
}