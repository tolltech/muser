using Microsoft.Extensions.DependencyInjection;
using Tolltech.MuserUI.Study;
using Tolltech.SqlEF;
using TolltechCore;

namespace Tolltech.MuserUI.UICore
{
    public static class IServiceCollectionExtensions
    {
        public static void ConfigureContainer(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IQueryExecutorFactory<KeyValueHandler, KeyValue>,
                    QueryExecutorFactory<KeyValueHandler, KeyValue>>();

            IoCResolver.Resolve((x, y) => serviceCollection.AddSingleton(x, y), "Tolltech");
        }
    }
}