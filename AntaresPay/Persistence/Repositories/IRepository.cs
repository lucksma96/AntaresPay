namespace AntaresPay.Persistence.Repositories;

public interface IRepository<Entity>
{
    Task<List<Entity>> GetAllAsync();
    Task<Entity> GetByIdAsync(int id);
    Task AddAsync(Entity entity);
    Task UpdateAsync(Entity entity);
    Task DeleteAsync(int id);
}
