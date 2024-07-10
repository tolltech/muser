using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tolltech.MuserUI.Sync
{
    [Table("tempsessions")]
    public class TempSessionDbo
    {
        [Column("id", TypeName = "uuid"), Key, Required]
        public Guid Id { get; set; }

        [Column("text", TypeName = "varchar"), ConcurrencyCheck, Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        [Column("date", TypeName = "timestamp"), Required]
        public DateTimeOffset Date { get; set; }

        [Column("userid", TypeName = "uuid"), ConcurrencyCheck]
        public Guid? UserId { get; set; }
    }
}