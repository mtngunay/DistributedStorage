using DistributedStorage.Domain.Common;

namespace DistributedStorage.Application.Abstractions.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<bool> ExistsAsync(Guid id);
    }
}