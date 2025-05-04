namespace AntaresPay.Persistence.Repositories;

public interface IRepository<Entity>
{
    Task<List<Entity>> GetAllAsync();
    Task<Entity> GetByIdAsync(int id);
    Task<int> AddAsync(Entity entity);
    Task<int> UpdateAsync(Entity entity);
    Task<int> DeleteAsync(int id);
}
