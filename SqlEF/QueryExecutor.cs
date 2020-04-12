using System;
using System.Threading.Tasks;
using Tolltech.SqlEF.Integration;

namespace Tolltech.SqlEF
{
    public class QueryExecutor : IDisposable
    {
        private readonly DataContextBase dataContext;
        private readonly ISqlHandlerProvider sqlHandlerProvider;

        public QueryExecutor(DataContextBase dataContext, ISqlHandlerProvider sqlHandlerProvider)
        {
            this.dataContext = dataContext;
            this.sqlHandlerProvider = sqlHandlerProvider;
        }

        public Task ExecuteAsync<THandle>(Func<THandle, Task> query)
        {
            var handle = sqlHandlerProvider.Create<THandle>(dataContext);
            return query(handle);
        }

        public Task<TResult> ExecuteAsync<THandle, TResult>(Func<THandle, Task<TResult>> query)
        {
            var handle = sqlHandlerProvider.Create<THandle>(dataContext);
            return query(handle);
        }

        public void Dispose()
        {
            dataContext.Dispose();
        }
    }
}