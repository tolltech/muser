using System;
using Microsoft.Extensions.DependencyInjection;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Psql.Integration
{
    public class SqlHandlerProvider : ISqlHandlerProvider
    {
        private readonly IServiceProvider serviceProvider;

        public SqlHandlerProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public TSqlHandler Create<TSqlHandler>(DataContextBase dataContext)
        {
            return (TSqlHandler) ActivatorUtilities.CreateInstance(serviceProvider, typeof(TSqlHandler), dataContext);
        }
    }
}