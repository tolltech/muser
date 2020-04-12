using Microsoft.EntityFrameworkCore;
using Tolltech.SqlEF;

namespace Tolltech.PostgreEF.Integration
{
    public class PsqlDataContext<TSqlEntity> : DataContextBase<TSqlEntity> where TSqlEntity : class
    {
        private readonly string connectionString;

        public PsqlDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void ConfigureOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}