using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tolltech.MuserUI.Authentications
{
    [Table("users")]
    public class User
    {
        [Column("id", TypeName = "uuid"), Required, Key]
        public Guid Id { get; set; }

        [Column("email", TypeName = "varchar(10000)"), Required]
        public string Email { get; set; }

        [Column("password", TypeName = "varchar(10000)"), Required]
        public string Password { get; set; }
    }
}