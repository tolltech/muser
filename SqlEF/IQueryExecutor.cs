using System;

namespace Tolltech.ThisCore.Sql
{
    public interface IQueryExecutor : IDisposable
    {
        void Execute<THandle>(Action<THandle> query);
        TResult Execute<THandle, TResult>(Func<THandle, TResult> query);
    }
}