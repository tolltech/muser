using Tolltech.SqlEF.Integration;

namespace Tolltech.SqlEF
{
    public class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly IDataContextFactory dataContextFactory;
        private readonly ISqlHandlerProvider sqlHandlerProvider;

        public QueryExecutorFactory(IDataContextFactory dataContextFactory, ISqlHandlerProvider sqlHandlerProvider)
        {
            this.dataContextFactory = dataContextFactory;
            this.sqlHandlerProvider = sqlHandlerProvider;
        }

        public QueryExecutor Create()
        {
            return new QueryExecutor(dataContextFactory.Create(), sqlHandlerProvider);
        }

        public QueryExecutor Create(string connectionStringKey)
        {
            return new QueryExecutor(dataContextFactory.Create(), sqlHandlerProvider);
        }
    }
}