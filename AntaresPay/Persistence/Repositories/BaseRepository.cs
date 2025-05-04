
using AntaresPay.Persistence.Entities;

namespace AntaresPay.Persistence.Repositories;

public abstract class BaseRepository<T>(SQLiteDatabase database) : IRepository<T>
    where T : BaseEntity, new()
{
    public readonly SQLiteDatabase Database = database;

    public async Task<int> AddAsync(T entity)
    {
        return await Database.Connection.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await Database.Connection.DeleteAsync<T>(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await Database.Connection.Table<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await Database.Connection.GetAsync<T>(id);
    }

    public async Task<int> UpdateAsync(T entity)
    {
        return await Database.Connection.UpdateAsync(entity);
    }
}
