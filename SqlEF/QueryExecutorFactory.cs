﻿using Tolltech.SqlEF.Integration;

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

        public QueryExecutor<TSqlHandler, TSqlEntity> Create<TSqlHandler, TSqlEntity>() where TSqlEntity : class
            where TSqlHandler : SqlHandlerBase<TSqlEntity>
        {
            return new QueryExecutor<TSqlHandler, TSqlEntity>(dataContextFactory.Create<TSqlEntity>(),
                sqlHandlerProvider);
        }
    }
}