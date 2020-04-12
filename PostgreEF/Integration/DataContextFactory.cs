using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.PostgreEF.Integration
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly IConnectionString connectionString;

        public DataContextFactory(IConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataContextBase<TSqlEntity> Create<TSqlEntity>() where TSqlEntity : class
        {
            return new PsqlDataContext<TSqlEntity>(connectionString.Value);
        }
    }
}