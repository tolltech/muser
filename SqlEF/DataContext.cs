using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tolltech.SqlEF
{
    public abstract class DataContextBase<T> : DbContext
    {
        public DbSet<MediaDate> MediaDates { get; set; }

        protected abstract void ConfigureOptionsBuilder(DbContextOptionsBuilder optionsBuilder);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigureOptionsBuilder(optionsBuilder);
        }

        public IQueryable<T> GetTable<T>() where T : class
        {
            return Set<T>();
        }

        public Task CreateAsync<T>(params T[] data) where T : class
        {
            Set<T>().AddRange(data);
            return SaveChangesAsync();
        }

        public Task DeleteAsync<T>(params T[] data) where T : class
        {
            Set<T>().RemoveRange(data);
            return SaveChangesAsync();
        }

        public Task UpdateAsync()
        {
            return SaveChangesAsync();
        }
    }
}