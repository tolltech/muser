using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tolltech.MuserUI.Sync
{
    [Table("playlists")]
    public class PlaylistDbo
    {
        [Column("id", TypeName = "uuid"), Key, Required]
        public Guid Id { get; set; }

        [Column("userid", TypeName = "uuid"), Required]
        public Guid UserId { get; set; }

        [Column("content", TypeName = "varchar"), Required(AllowEmptyStrings = true)]
        public string Content { get; set; }

        [Column("filename", TypeName = "varchar"), Required(AllowEmptyStrings = true)]
        public string Filename { get; set; }

        [Column("extension", TypeName = "varchar"), Required(AllowEmptyStrings = true)]
        public string Extension { get; set; }

        [Column("date", TypeName = "timestamp"), Required]
        public DateTimeOffset Date { get; set; }
    }
}