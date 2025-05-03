using AntaresPay.Persistence.Entities;
using SQLite;
using System.Diagnostics;

namespace AntaresPay.Persistence
{
    public class SQLiteDatabase
    {
        public SQLiteAsyncConnection Connection { get; private set; }

        public SQLiteDatabase()
        {
            if (Connection == null)
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "antarespay.db3");
                Connection = new SQLiteAsyncConnection(path);
            }

            Connection.CreateTableAsync<OperationEntity>().ContinueWith((a) => Debug.WriteLine($"{nameof(OperationEntity)} table ok"));
        }
    }
}
