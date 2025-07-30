using DistributedStorage.Domain.Entities;

namespace DistributedStorage.Application.Abstractions.Repositories
{
    public interface IChunkRepository : IGenericRepository<Chunk>
    {
        Task<List<Chunk>> GetAllByFileIdAsync(Guid fileId, CancellationToken cancellationToken = default);
    }
}