using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tolltech.MuserUI.Sync
{
    [Table("tempsessions")]
    public class TempSessionDbo
    {
        [Column("Id", TypeName = "uuid"), Key, Required]
        public Guid Id { get; set; }

        [Column("Text", TypeName = "varchar"), ConcurrencyCheck, Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        [Column("Date", TypeName = "timestamp"), Required]
        public DateTime Date { get; set; }

        [Column("UserId", TypeName = "uuid"), ConcurrencyCheck]
        public Guid? UserId { get; set; }
    }
}