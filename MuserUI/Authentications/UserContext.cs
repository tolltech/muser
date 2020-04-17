using Microsoft.EntityFrameworkCore;

namespace Tolltech.MuserUI.Authentications
{
    public sealed class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}