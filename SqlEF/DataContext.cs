using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using log4net;

namespace Tolltech.ThisCore.Sql
{
    public class DataContextImpl : IDataContext
    {
        private static readonly MappingSource mapping = new AttributeMappingSource();

        private readonly DataContext dataContext;
        private readonly IDbConnection connection;
        private static readonly ILog log = LogManager.GetLogger(typeof(DataContextImpl));

        public DataContextImpl(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
            dataContext = new DataContext(connection, mapping);
        }

        public void Dispose()
        {
            dataContext?.Dispose();
            connection?.Dispose();
        }

        public IQueryable GetTable(Type type)
        {
            return dataContext.GetTable(type);
        }

        public IQueryable<T> GetTable<T>() where T : class
        {
            return dataContext.GetTable<T>();
        }

        public void Create(Type type, object data)
        {
            Create(dataContext.GetTable(type), data);
        }

        public void Create<T>(T data) where T : class
        {
            Create(dataContext.GetTable<T>(), data);
        }

        public void CreateAll<T>(IEnumerable<T> datas) where T : class
        {
            CreateAll(dataContext.GetTable<T>(), datas);
        }

        private void CreateAll(ITable table, IEnumerable<object> datas)
        {
            SafeCall(() =>
            {
                table.InsertAllOnSubmit(datas);
                dataContext.SubmitChanges();
            });
        }

        private void Create(ITable table, object data)
        {
            SafeCall(() =>
            {
                table.InsertOnSubmit(data);
                dataContext.SubmitChanges();
            });
        }

        public void Update()
        {
            SafeCall(() => dataContext.SubmitChanges());
        }

        public void ResetIdentityCounter(string tableName)
        {
            var command = string.Format("DBCC CHECKIDENT (\"{0}\", RESEED, 0)", tableName);
            dataContext.ExecuteCommand(command);
        }

        public void Delete(Type type, object data)
        {
            Delete(dataContext.GetTable(type), data);
        }

        public void Delete<T>(T data) where T : class
        {
            Delete(dataContext.GetTable<T>(), data);
        }

        public void DeleteAll<T>(IEnumerable<T> datas) where T : class
        {
            DeleteAll(dataContext.GetTable<T>(), datas);
        }

        private void DeleteAll(ITable table, IEnumerable<object> datas)
        {
            SafeCall(() =>
            {
                foreach (var data in datas)
                {
                    var entityState = table.GetOriginalEntityState(data);
                    if (entityState == null)
                        table.Attach(data);
                }
                table.DeleteAllOnSubmit(datas);
                dataContext.SubmitChanges();
            });
        }

        private void Delete(ITable table, object data)
        {
            SafeCall(() =>
            {
                var entityState = table.GetOriginalEntityState(data);
                if (entityState == null)
                    table.Attach(data);
                table.DeleteOnSubmit(data);
                dataContext.SubmitChanges();
            });
        }

        private void SafeCall(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                throw;
            }
        }
    }
}