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

        public DataContextBase Create()
        {
            return new PsqlDataContext(connectionString.Value);
        }
    }
}