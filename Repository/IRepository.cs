namespace dotnet_rpg.Repository;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<List<Character>> GetAllAsync();
    void Add(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task SaveChangesAsync();
}