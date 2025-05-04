using AntaresPay.Persistence.Entities;

namespace AntaresPay.Persistence.Repositories;

public class OperationRepository(SQLiteDatabase database) : BaseRepository<OperationEntity>(database)
{
    public async Task<List<OperationEntity>> GetByUnitNameAsync(string value)
    {
        return await Database.Connection.Table<OperationEntity>().Where(x => x.UnitName == value).OrderByDescending(x => x.CreatedAt).ToListAsync();
    }
}
