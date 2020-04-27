using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tolltech.MuserUI.Sync
{
    [Table("playlists")]
    public class PlaylistDbo
    {
        [Column("Id", TypeName = "uuid"), Key, Required]
        public Guid Id { get; set; }

        [Column("UserId", TypeName = "uuid"), Required]
        public Guid UserId { get; set; }

        [Column("Content", TypeName = "varchar"), Required(AllowEmptyStrings = true)]
        public string Content { get; set; }

        [Column("Filename", TypeName = "varchar"), Required(AllowEmptyStrings = true)]
        public string Filename { get; set; }

        [Column("Extension", TypeName = "varchar"), Required(AllowEmptyStrings = true)]
        public string Extension { get; set; }

        [Column("Date", TypeName = "timestamp"), Required]
        public DateTime Date { get; set; }
    }
}