using System;
using System.Threading.Tasks;
using Tolltech.SqlEF.Integration;

namespace Tolltech.SqlEF
{
    public class QueryExecutor<TSqlHandler, TSqlEntity> : IDisposable where TSqlHandler : SqlHandlerBase<TSqlEntity> where TSqlEntity : class
    {
        private readonly DataContextBase<TSqlEntity> dataContext;
        private readonly ISqlHandlerProvider sqlHandlerProvider;

        public QueryExecutor(DataContextBase<TSqlEntity> dataContext, ISqlHandlerProvider sqlHandlerProvider)
        {
            this.dataContext = dataContext;
            this.sqlHandlerProvider = sqlHandlerProvider;
        }

        public Task ExecuteAsync(Func<TSqlHandler, Task> query)
        {
            var handle = sqlHandlerProvider.Create<TSqlHandler, TSqlEntity>(dataContext);
            return query(handle);
        }

        public Task<TResult> ExecuteAsync<TResult>(Func<TSqlHandler, Task<TResult>> query)
        {
            var handle = sqlHandlerProvider.Create<TSqlHandler, TSqlEntity>(dataContext);
            return query(handle);
        }

        public void Dispose()
        {
            dataContext.Dispose();
        }
    }
}