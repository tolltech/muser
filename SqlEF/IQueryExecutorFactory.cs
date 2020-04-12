using Tolltech.SqlEF.Integration;

namespace Tolltech.SqlEF
{
    public interface IQueryExecutorFactory<TSqlHandler, TSqlEntity> where TSqlEntity : class where TSqlHandler : SqlHandlerBase<TSqlEntity>
    {
        QueryExecutor<TSqlHandler, TSqlEntity> Create();
    }
}