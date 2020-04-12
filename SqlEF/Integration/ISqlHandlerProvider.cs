namespace Tolltech.SqlEF.Integration
{
    public interface ISqlHandlerProvider
    {
        TSqlHandler Create<TSqlHandler, TSqlEntity>(DataContextBase<TSqlEntity> dataContext) where TSqlHandler : SqlHandlerBase<TSqlEntity> where TSqlEntity : class;
    }
}