namespace DistributedStorage.Application.Abstractions.Services
{
    public interface IStorageProvider
    {
        Task SaveChunkAsync(Guid chunkId, byte[] data, bool encrypt = false, CancellationToken cancellationToken = default);

        Task<byte[]> GetChunkAsync(Guid chunkId, bool decrypt = false, CancellationToken cancellationToken = default);

        Task DeleteChunkAsync(Guid chunkId, CancellationToken cancellationToken = default);
    }
}