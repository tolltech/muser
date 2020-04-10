using System.Data.Linq;

namespace Tolltech.ThisCore.Sql
{
    public interface IDataContextFactory
    {
        DataContext Create();
    }
}