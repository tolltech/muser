using System;
using Ninject;
using Ninject.Parameters;

namespace Tolltech.ThisCore.Sql
{
    public class QueryExecutor : IQueryExecutor
    {
        private readonly IDataContext dataContext;
        private readonly IKernel kernel;

        public QueryExecutor(IDataContext dataContext, IKernel kernel)
        {
            this.dataContext = dataContext;
            this.kernel = kernel;
        }

        public void Execute<THandle>(Action<THandle> query)
        {
            //var type = kernel.Get<THandle>().GetType();
            //var handle = (THandle)Activator.CreateInstance(type, dataContext);
            var handle = kernel.Get<THandle>(new ConstructorArgument("dataContext", dataContext));
            query(handle);
        }

        public TResult Execute<THandle, TResult>(Func<THandle, TResult> query)
        {
            //var type = kernel.Get<THandle>().GetType();
            //var handle = (THandle)Activator.CreateInstance(type, dataContext);
            var handle = kernel.Get<THandle>(new ConstructorArgument("dataContext", dataContext));
            return query(handle);
        }

        public void Dispose()
        {
            dataContext.Dispose();
        }
    }
}