using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.SqlEF;
using Tolltech.SqlEF.Integration;

namespace Tolltech.MuserUI.Sync
{
    public class ImportResultHandler : SqlHandlerBase<ImportResultDbo>
    {
        private readonly DataContextBase<ImportResultDbo> dataContext;

        public ImportResultHandler(DataContextBase<ImportResultDbo> dataContext)
        {
            this.dataContext = dataContext;
        }

        [NotNull]
        public Task CreateAsync([NotNull] [ItemNotNull] params ImportResultDbo[] importResults)
        {
            return dataContext.Table.AddRangeAsync(importResults);
        }
    }
}