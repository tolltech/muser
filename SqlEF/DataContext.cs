using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tolltech.SqlEF
{
    public abstract class DataContextBase<T> : DbContext where T : class
    {
        public DbSet<T> Table { get; set; }

        protected abstract void ConfigureOptionsBuilder(DbContextOptionsBuilder optionsBuilder);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigureOptionsBuilder(optionsBuilder);
        }

        public Task CreateAsync(params T[] data)
        {
            Table.AddRange(data);
            return SaveChangesAsync();
        }

        public Task DeleteAsync(params T[] data)
        {
            Table.RemoveRange(data);
            return SaveChangesAsync();
        }

        public Task UpdateAsync()
        {
            return SaveChangesAsync();
        }
    }
}