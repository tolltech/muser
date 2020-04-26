using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tolltech.Muser.Models;

namespace Tolltech.MuserUI.Sync
{
    [Table("ImportResults")]
    public class ImportResultDbo
    {
        [Column("Id", TypeName = "uuid"), Key, Required]
        public Guid Id { get; set; }

        [Column("SessionId", TypeName = "uuid"), Required]
        public Guid SessionId { get; set; }

        [Column("UserId", TypeName = "uuid"), Required]
        public Guid UserId { get; set; }

        [Column("Status", TypeName = "int"), Required]
        public ImportStatus Status { get; set; }

        [Column("Date", TypeName = "timestamp"), Required]
        public DateTime Date { get; set; }

        [Column("Title", TypeName = "varchar"), Required]
        public string Title { get; set; }

        [Column("Artist", TypeName = "varchar"), Required]
        public string Artist { get; set; }

        [Column("NormalizedTitle", TypeName = "varchar"), Required]
        public string NormalizedTitle { get; set; }

        [Column("NormalizedArtist", TypeName = "varchar"), Required]
        public string NormalizedArtist { get; set; }

        [Column("CandidateTitle", TypeName = "varchar"), Required]
        public string CandidateTitle { get; set; }

        [Column("CandidateArtist", TypeName = "varchar"), Required]
        public string CandidateArtist { get; set; }

        [Column("Message", TypeName = "varchar"), Required]
        public string Message { get; set; }
    }
}