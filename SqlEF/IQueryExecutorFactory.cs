using Tolltech.SqlEF.Integration;

namespace Tolltech.SqlEF
{
    public interface IQueryExecutorFactory
    {
        QueryExecutor<TSqlHandler, TSqlEntity> Create<TSqlHandler, TSqlEntity>() where TSqlEntity : class where TSqlHandler : SqlHandlerBase<TSqlEntity>;
    }
}