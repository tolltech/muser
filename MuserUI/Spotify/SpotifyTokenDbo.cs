using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tolltech.MuserUI.Spotify
{                                
    [Table("spotify_tokens")]
    public class SpotifyTokenDbo
    {
        [Column("user_id", TypeName = "uuid"), Key, Required]
        public Guid UserId { get; set; }

        [Column("access_token", TypeName = "varchar"), ConcurrencyCheck, Required(AllowEmptyStrings = true)]
        public string AccessToken { get; set; }

        [Column("token_type", TypeName = "varchar"), ConcurrencyCheck, Required(AllowEmptyStrings = true)]
        public string TokenType { get; set; }

        [Column("scope", TypeName = "varchar"), ConcurrencyCheck, Required(AllowEmptyStrings = true)]
        public string Scope { get; set; }

        [Column("expires_utc", TypeName = "timestamptz"), ConcurrencyCheck]
        public DateTime ExpiresUtc { get; set; }

        [Column("refresh_token", TypeName = "varchar"), ConcurrencyCheck, Required(AllowEmptyStrings = true)]
        public string RefreshToken { get; set; }
    }
}