using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tolltech.Muser.Models;

namespace Tolltech.MuserUI.Sync
{
    [Table("importresults")]
    public class ImportResultDbo
    {
        [Column("id", TypeName = "uuid"), Key, Required]
        public Guid Id { get; set; }

        [Column("sessionid", TypeName = "uuid"), Required]
        public Guid SessionId { get; set; }

        [Column("userid", TypeName = "uuid"), Required]
        public Guid UserId { get; set; }

        [Column("status", TypeName = "int"), Required]
        public ImportStatus Status { get; set; }

        [Column("date", TypeName = "timestamp"), Required]
        public DateTime Date { get; set; }

        [Column("title", TypeName = "varchar"), Required]
        public string Title { get; set; }

        [Column("artist", TypeName = "varchar"), Required]
        public string Artist { get; set; }

        [Column("normalizedtitle", TypeName = "varchar"), Required]
        public string NormalizedTitle { get; set; }

        [Column("normalizedartist", TypeName = "varchar"), Required]
        public string NormalizedArtist { get; set; }

        [Column("candidatetitle", TypeName = "varchar"), Required]
        public string CandidateTitle { get; set; }

        [Column("candidateartist", TypeName = "varchar"), Required]
        public string CandidateArtist { get; set; }

        [Column("candidatetrackid", TypeName = "varchar")]
        public string CandidateTrackId { get; set; }

        [Column("candidatealbumid", TypeName = "varchar")]
        public string CandidateAlbumId { get; set; }

        [Column("message", TypeName = "varchar"), Required]
        public string Message { get; set; }

        [Column("approvedmanual", TypeName = "boolean"), Required]
        public bool ApprovedManual { get; set; }

        [Column("playlistid", TypeName = "varchar")]
        public string PlaylistId { get; set; }
    }
}