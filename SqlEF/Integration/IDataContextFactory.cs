namespace Tolltech.SqlEF.Integration
{
    public interface IDataContextFactory
    {
        DataContextBase Create();
    }
}