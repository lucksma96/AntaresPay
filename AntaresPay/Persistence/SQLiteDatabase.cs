using AntaresPay.Persistence.Entities;
using SQLite;

namespace AntaresPay.Persistence
{
    public class SQLiteDatabase
    {
        public SQLiteConnection Connection { get; private set; }

        SQLiteDatabase()
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "database.db");
            Connection = new SQLiteConnection(path);
            Connection.CreateTable<Operation>();
        }
    }
}
