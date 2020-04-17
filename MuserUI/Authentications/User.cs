using System;

namespace Tolltech.MuserUI.Authentications
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}