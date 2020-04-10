using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;

namespace Tolltech.ThisCore.Sql
{
    public class DataContextFactory : IDataContextFactory
    {
        private readonly IDbConnection connection;
        private static readonly MappingSource mapping = new AttributeMappingSource();

        public DataContextFactory(IConnectionString connectionString)
        {
            this.connection = new SqlConnection(connectionString.Value);
        }

        public DataContext Create()
        {
            return new DataContext(connection, mapping);
        }
    }
}