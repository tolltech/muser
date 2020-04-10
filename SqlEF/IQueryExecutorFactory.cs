namespace Tolltech.ThisCore.Sql
{
    public interface IQueryExecutorFactory
    {
        QueryExecutor Create();
        QueryExecutor Create(string connectionStringKey);
    }
}