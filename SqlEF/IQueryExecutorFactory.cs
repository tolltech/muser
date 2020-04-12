namespace Tolltech.SqlEF
{
    public interface IQueryExecutorFactory
    {
        QueryExecutor Create();
        QueryExecutor Create(string connectionStringKey);
    }
}