namespace Data.IRepository
{
    public interface ICurdRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<int> AddUpdateAsync(T entity);
        Task<int> AddUpdateScalerAsync(T entity);
        Task<int> DeleteAsync(int id);
    }
}
