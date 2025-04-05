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
            // TODO - create tables
        }
    }
}
