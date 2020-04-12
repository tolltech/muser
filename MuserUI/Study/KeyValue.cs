using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Tolltech.MuserUI.Study
{
    [Table("KeyValues")]
    public class KeyValue
    {
        [JsonIgnore]
        [Column("id", TypeName = "uuid"), Required, Key]
        public Guid Id { get; set; }

        [Column("key", TypeName = "varchar(10000)"), Required]
        public string Key { get; set; }

        [Column("value", TypeName = "varchar(10000)"), Required]
        public string Value { get; set; }
    }
}