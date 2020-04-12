namespace Tolltech.SqlEF.Integration
{
    public interface IDataContextFactory
    {
        DataContextBase<TSqlEntity> Create<TSqlEntity>() where TSqlEntity : class;
    }
}