using File = DistributedStorage.Domain.Entities.File;

namespace DistributedStorage.Application.Abstractions.Repositories
{
    public interface IFileRepository : IGenericRepository<File>
    {
        Task<File?> GetFileWithChunksAsync(Guid fileId, CancellationToken cancellationToken = default);
    }
}