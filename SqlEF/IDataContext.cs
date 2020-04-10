using System;
using System.Collections.Generic;
using System.Linq;

namespace Tolltech.ThisCore.Sql
{
    public interface IDataContext : IDisposable
    {
        IQueryable GetTable(Type type);
        IQueryable<T> GetTable<T>() where T : class;

        void Create(Type type, object data);
        void Create<T>(T data) where T : class;
        void CreateAll<T>(IEnumerable<T> datas) where T : class;

        void Delete(Type type, object data);
        void Delete<T>(T data) where T : class;
        void DeleteAll<T>(IEnumerable<T> datas) where T : class;

        void Update();

        void ResetIdentityCounter(string tableName);
    }
}
