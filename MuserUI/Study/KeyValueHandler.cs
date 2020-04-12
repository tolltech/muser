using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Study
{
    public class KeyValueHandler : SqlHandlerBase<KeyValue>
    {
        private readonly DataContextBase<KeyValue> dataContext;

        public KeyValueHandler(DataContextBase<KeyValue> dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        [ItemCanBeNull]
        public Task<KeyValue> FindAsync([NotNull] string key)
        {
            return dataContext.Table.FirstOrDefaultAsync(x => x.Key == key);
        }

        [NotNull]
        public Task CreateAsync([NotNull] [ItemNotNull] params KeyValue[] keyValues)
        {
            throw new System.NotImplementedException();
        }

        [NotNull]
        [ItemNotNull]
        public Task<KeyValue[]> SelectAsync([ItemNotNull] [NotNull] string[] keys)
        {
            throw new System.NotImplementedException();
        }

        [NotNull]
        public Task UpdateAsync([ItemNotNull] [NotNull] params KeyValue[] existed)
        {
            throw new System.NotImplementedException();
        }
    }
}