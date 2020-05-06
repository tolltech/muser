namespace Tolltech.Muser.Models
{
    public class ImportResult
    {
        public ImportResult(string artist, string title, string playlistId)
        {
            this.ImportingTrack = new ImportingTrack()
            {
                Artist = artist,
                Title = title,
            };
            PlaylistId = playlistId;
        }

        public ImportStatus ImportStatus { get; set; }

        public string Message { get; set; }

        public ImportingTrack ImportingTrack { get; }

        public ImportingTrack NormalizedTrack { get; set; }

        public ImportingTrack Candidate { get; set; }

        public string CandidateTrackId { get; set; }

        public string CandidateAlbumId { get; set; }

        public string PlaylistId { get; set; }
    }
}