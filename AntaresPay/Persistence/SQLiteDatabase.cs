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
            var path = Path.Combine(FileSystem.AppDataDirectory, "database.db");
            Connection = new SQLiteAsyncConnection(path);
            Connection.CreateTableAsync<OperationEntity>().ContinueWith((a) => Debug.WriteLine($"{nameof(OperationEntity)} table ok"));
        }
    }
}
