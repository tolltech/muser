using Ninject;

namespace Tolltech.ThisCore.Sql
{
    public class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly IConfiguration configuration;
        private readonly IKernel kernel;
        private readonly string defaultConectionString;

        public QueryExecutorFactory(IConfiguration configuration, IKernel kernel)
        {
            this.configuration = configuration;
            this.kernel = kernel;
            defaultConectionString = configuration.Get(ConfigKeys.DbConnectionStringKey);
        }

        public QueryExecutor Create()
        {
            return new QueryExecutor(new DataContextImpl(defaultConectionString), kernel);
        }

        public QueryExecutor Create(string connectionStringKey)
        {
            return new QueryExecutor(new DataContextImpl(configuration.Get(connectionStringKey)), kernel);
        }
    }
}