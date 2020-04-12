namespace Tolltech.SqlEF.Integration
{
    public interface ISqlHandlerProvider
    {
        TSqlHandler Create<TSqlHandler>(DataContextBase dataContext);
    }
}