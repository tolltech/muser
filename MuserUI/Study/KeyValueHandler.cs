using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Study
{
    public class KeyValueHandler
    {
        private readonly DataContextBase dataContext;

        public KeyValueHandler(DataContextBase dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        [ItemCanBeNull]
        public Task<KeyValue> FindAsync([NotNull] string key)
        {
            return dataContext.GetTable<KeyValue>().FirstOrDefaultAsync(x => x.Key == key);
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